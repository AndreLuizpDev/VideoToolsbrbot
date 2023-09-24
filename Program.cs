using OpenAI;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using OpenAI.Audio;
using Xabe.FFmpeg;
using NAudio.Wave;
using System.Diagnostics;
using System.Configuration;

var botClient = new TelegramBotClient(ConfigurationManager.AppSettings["api-Bot"]);
var apiOpenAI = new OpenAIClient(ConfigurationManager.AppSettings["sk-apiKey"]);

var cancellationToken = new CancellationTokenSource();

botClient.StartReceiving(
    updateHandler: updateHandlerAsync,
    pollingErrorHandler: pollingErrorHandlerAsync,
    receiverOptions: new ReceiverOptions
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    },
    cancellationToken: cancellationToken.Token
);
Task pollingErrorHandlerAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ctoken)
{
    Console.WriteLine(exception.Message);
    return Task.CompletedTask;
}
async Task updateHandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken ctoken)
{
    if (update.Message is not { } message)
        return;

    var chatId = message.Chat.Id;
    var messageText = message.Text;

    string videosPath = Directory.GetCurrentDirectory();

    for (int i = 0; i < 3; i++)
    {
        videosPath = Directory.GetParent(videosPath).FullName;
    }

    videosPath = videosPath + @"\Videos\";

    Console.WriteLine($"A pasta atual do projeto é: {videosPath}");

    Console.WriteLine($"mensagem recebida: '{messageText}' do tipo '{message.Type}' no chat '{chatId}'");

    if (message.Video is null)
    {
        var sendErro = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Você precisa enviar um vídeo!",
            cancellationToken: ctoken
            );
        return;
    }

    var sendMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Vídeo recebido, processando...",
    cancellationToken: ctoken
    );

    var fileInfo = await botClient.GetFileAsync(message.Video!.FileId);
    var filePath = fileInfo.FilePath;

    var fileName = Path.GetFileName(filePath);

    Console.WriteLine($"{filePath} {messageText}");

    string destinationFilePath = videosPath + fileName;

    Console.WriteLine("Salvando vídeo...");

    await using Stream fileStream = System.IO.File.Create(destinationFilePath);
    await botClient.DownloadFileAsync(
        filePath: filePath,
        destination: fileStream,
        cancellationToken: ctoken);

    fileStream.Close();

    string inputFilePath = destinationFilePath;
    string outputFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + ".mp3";

    Console.WriteLine($"Entrada: {inputFilePath} Saída: {outputFilePath}");

    sendMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Gerando legenda...",
    cancellationToken: ctoken
    );

    var reader = new MediaFoundationReader(inputFilePath);
    WaveFileWriter.CreateWaveFile(outputFilePath, reader);

    var request = new AudioTranscriptionRequest(Path.GetFullPath(outputFilePath), responseFormat: AudioResponseFormat.Srt);
    var result = await apiOpenAI.AudioEndpoint.CreateTranscriptionAsync(request);
    Console.WriteLine(result);

    Console.WriteLine("Salvando legenda...");

    string subtitleFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + ".srt";

    System.IO.File.WriteAllText(subtitleFilePath, result);

    sendMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Legendando vídeo...",
    cancellationToken: ctoken
    );

    string legendaFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + "_legendado.mp4";

    Console.WriteLine($"{legendaFilePath}");

    Console.WriteLine($"Salvando arquivo: {Path.GetFileNameWithoutExtension(filePath)}_legendado.mp4 ...");

    Console.WriteLine($"inputFilePath: {inputFilePath} subtitleFilePath: {subtitleFilePath} legendaFilePath: {legendaFilePath}");

    var processInfo = new ProcessStartInfo
    {
        FileName = @"C:\ffmpeg-master-latest-win64-gpl\bin\ffmpeg.exe",
        Arguments = $"-i \"{inputFilePath}\" -vf \"subtitles='{subtitleFilePath}'\" -y \"{legendaFilePath}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    sendMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Enviando arquivo legendado...",
    cancellationToken: ctoken
    );

    fileStream.Close();

    using (var process = new Process())
    {
        process.StartInfo = processInfo;
        process.Start();

        int timeoutMilliseconds = 6000;
        process.WaitForExit(timeoutMilliseconds);
    };

    Console.WriteLine("Arquivo gerado com sucesso!");

    fileStream.Close();

    Thread.Sleep(3000);

    using Stream stream = System.IO.File.OpenRead(legendaFilePath);

    Console.WriteLine("Upload do arquivo rezlizado com sucesso.");

    var sendVideo = await botClient.SendVideoAsync(
        chatId: chatId,
        video: InputFile.FromStream(stream),
        cancellationToken: ctoken
    );

    string pasta = videosPath; // Substitua pelo caminho da pasta que você deseja limpar.

    try
    {
        // Obtém todos os arquivos na pasta.
        string[] arquivos = Directory.GetFiles(pasta);

        Console.WriteLine(arquivos);

        // Exclui cada arquivo encontrado.
        foreach (string arquivo in arquivos)
        {
            System.IO.File.Delete(arquivo);
            Console.WriteLine($"Arquivo excluído: {arquivo}");
        }

        Console.WriteLine("Todos os arquivos foram excluídos com sucesso.");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Ocorreu um erro: {e.Message}");
    }

}

var me = await botClient.GetMeAsync(cancellationToken.Token);

Console.WriteLine($"Escutando {me.Username}");
Console.ReadLine();

cancellationToken.Cancel();