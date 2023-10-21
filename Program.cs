using OpenAI;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using OpenAI.Audio;
using NAudio.Wave;
using System.Diagnostics;
using System.Configuration;
using Newtonsoft.Json.Linq;

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
    var languageChat = message.From.LanguageCode.Substring(0, 2) ?? "en";
    string videosPath = Directory.GetCurrentDirectory();
    var trashVideo = "";
    var trashAudio = "";
    var trashSubtitle = "";
    var trashSubtitled = "";

    for (int i = 0; i < 3; i++)
    {
        videosPath = Directory.GetParent(videosPath).FullName;
    }

    videosPath = videosPath + @"\Videos\";

    if (!Directory.Exists(videosPath))
    {
        Console.WriteLine("The videos folder does not exist, creating...");
        Directory.CreateDirectory(videosPath);
    }
    else Console.WriteLine("The folder exists!");

    Console.WriteLine($"Received message: '{messageText}' of type '{message.Type}' in chat '{chatId}' {message.Chat.Username} Language: {languageChat}");

    if (message.Video is null)
    {
        var sendErro = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "You need to send a video!",
            cancellationToken: ctoken
            );
        return;
    }

    // Send a message indicating that the video is received and processing
    Message sentMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Video received, processing...",
    cancellationToken: ctoken
    );

    // Get file information for the received video
    var fileInfo = await botClient.GetFileAsync(message.Video!.FileId);
    var filePath = Path.GetFileName(fileInfo.FilePath);

    // Define the destination file path to save the video
    string destinationFilePath = videosPath + filePath;

    Console.WriteLine($"Saving the video...");

    // Create a stream to save the video file
    await using Stream fileStream = System.IO.File.Create(destinationFilePath);
    await botClient.DownloadFileAsync(
        filePath: fileInfo.FilePath,
        destination: fileStream,
        cancellationToken: ctoken);

    trashVideo = destinationFilePath;

    fileStream.Close();

    // Set the input and output file paths for further processing
    string inputFilePath = destinationFilePath;
    string outputFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + ".mp3";

    trashAudio = outputFilePath;

    // Generating subtitles in the audio language
    sentMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Generating subtitles...",
    cancellationToken: ctoken
    );

    // Create a reader for the audio file
    var reader = new MediaFoundationReader(inputFilePath);
    WaveFileWriter.CreateWaveFile(outputFilePath, reader);

    Console.WriteLine($"Selected language: {languageChat}");

    // Define a request for audio transcription
    var request = new AudioTranscriptionRequest(Path.GetFullPath(outputFilePath), language: languageChat, responseFormat: AudioResponseFormat.Srt);

    // Generate the subtitles using whisper OpenAI
    var result = await apiOpenAI.AudioEndpoint.CreateTranscriptionAsync(request);

    // Define the path to save the subtitle file
    string subtitleFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + ".srt";

    trashSubtitle = subtitleFilePath;

    System.IO.File.WriteAllText(subtitleFilePath, result);

    sentMessage = await botClient.SendTextMessageAsync(
    chatId: chatId,
    text: "Adding subtitles to the video...",
    cancellationToken: ctoken
    );

    string subtitledFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + "_subtitled.mp4";

    trashSubtitled = subtitledFilePath;

    inputFilePath = inputFilePath.Replace('\\', '/').Replace("C:", "");
    subtitleFilePath = subtitleFilePath.Replace('\\', '/').Replace("C:", "");
    subtitledFilePath = subtitledFilePath.Replace('\\', '/').Replace("C:", "");

    var fontname = ""; //The fontname as used by Windows. Case-sensitive.
    int fontsize = 0;
    var primaryColour = "&H03fcff"; //A long integer BGR (blue-green-red)  value. ie. the byte order in the hexadecimal equivelent of this number is BBGGRR
    var outlineColour = "&H40000000"; //A long integer BGR (blue-green-red)  value. ie. the byte order in the hexadecimal equivelent of this number is BBGGRR
    var backColour = "&H80000000"; //This is the colour of the subtitle outline or shadow, if these are used. A long integer BGR (blue-green-red)  value. ie. the byte order in the hexadecimal equivelent of this number is BBGGRR.
    int bold = 1; //This defines whether text is bold (true or 1) or not (false or 0).
    int italic = 1; //This defines whether text is italic (true or 1) or not (false or 0).
    int borderStyle = 3; //1=Outline + drop shadow, 3=Opaque box.
    int outline = 1; //If BorderStyle is 1,  then this specifies the width of the outline around the text, in pixels. Values may be 0, 1, 2, 3 or 4.
    double spacing = 0.8; //Extra space between characters. [pixels]
    double angle = 0.8; //The origin of the rotation is defined by the alignment. Can be a floating point number. [degrees]
    int alignment = 2; //(1-3 sub, 4-6 mid, 7-9 top)

    var forceStyleSubtitle = $"'PrimaryColour={primaryColour},Italic={italic},Spacing={spacing}'";

    // Configure the process
    var processInfo = new ProcessStartInfo
    {
        FileName = @"ffmpeg",
        Arguments = $"-i \"{inputFilePath}\" -vf \"subtitles={subtitleFilePath}:force_style={forceStyleSubtitle}\" -y \"{subtitledFilePath}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };

    Console.WriteLine("Current date and time 0 ************: " + DateTime.Now);

    // Initialize the process
    using (var process = new Process())
    {
        process.StartInfo = processInfo;

        FileInfo videoFileInfo = new FileInfo(inputFilePath);
        long totalVideoBytes = videoFileInfo.Length;
        long bytesRead = 0;

        Console.WriteLine($"totalVideoBytes: {totalVideoBytes}");
        ;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        while (bytesRead < totalVideoBytes)
        {
            // Get the size of the video file after each iteration.
            videoFileInfo.Refresh();

            // Update the amount of bytes read.
            bytesRead = videoFileInfo.Length;

            // Calculate the percentage of the video read.
            double percentageRead = (double)bytesRead / totalVideoBytes * 100;

            Console.WriteLine($"bytesRead: {bytesRead}");
            Console.WriteLine($"totalVideoBytes: {totalVideoBytes}");
            Console.WriteLine($"percentageRead: {percentageRead}");

            Console.WriteLine($"Percentage of video read: {percentageRead:F2}%");

            Thread.Sleep(500); // Aguarde um segundo antes de verificar novamente.
        }

        int timeoutMilliseconds = 60000;

        Console.WriteLine("Waiting for the process to exit!");
        if (process.WaitForExit(timeoutMilliseconds))
        {
            Console.WriteLine("Process exit with success!");
            process.Close();
        }
        else
        {
            Console.WriteLine("The process reached the timeout and is still running.");
            process.Kill(); // Terminate the process in case of a timeout
        }

    }

    using Stream stream = System.IO.File.OpenRead(subtitledFilePath);

    try
    {
        var sendVideo = await botClient.SendVideoAsync(
        chatId: chatId,
            video: InputFile.FromStream(stream),
            cancellationToken: ctoken
        );
    }
    catch (IOException e)
    {
        Console.WriteLine($"An error occurred: {e.Message}");
    }
    finally
    {
        if (stream != null)
        {
            stream.Close(); // fs.Dispose();
            Console.WriteLine("Space released");
        }
    }

    try
    {
        Console.WriteLine($"Deleting: {trashAudio}");
        System.IO.File.Delete(trashAudio);
        Console.WriteLine($"Deleting: {trashSubtitle}");
        System.IO.File.Delete(trashSubtitle);
        Console.WriteLine($"Deleting: {trashSubtitled}");
        System.IO.File.Delete(trashSubtitled);
        Console.WriteLine($"Deleting: {trashVideo}");
        System.IO.File.Delete(trashVideo);

        Console.WriteLine("All files have been successfully deleted.");
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error occurred: {e.Message}");
    }

}

var me = await botClient.GetMeAsync(cancellationToken.Token);

Console.WriteLine($"Listening to {me.Username}");
Console.ReadLine();

cancellationToken.Cancel();