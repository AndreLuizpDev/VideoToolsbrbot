using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using OpenAI;
using OpenAI.Audio;
using NAudio.Wave;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Telegram.Bot.Types.Enums;

namespace VideoToolsbrbot
{
    public class MainVideoToolsbrbot : EasyBot
    {
        static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BotToken"])) { 
                Console.WriteLine("Token null"); return;
            }

            string botToken = ConfigurationManager.AppSettings["BotToken"] ?? "";

            var bot = new MainVideoToolsbrbot(botToken);
            bot.Run();
        }

        public MainVideoToolsbrbot(string botToken) : base(botToken) { }

        public override async Task OnPrivateChat(Chat chat, User user, UpdateInfo update)
        {
            var apiOpenAI = new OpenAIClient(ConfigurationManager.AppSettings["sk-apiKey"]);
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            var languageChat = "en";
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

            if (update.Message != null && update.Message.From != null && !string.IsNullOrEmpty(update.Message.From.LanguageCode))
            {
                languageChat = update.Message.From.LanguageCode.Substring(0, 2);
            }

            Console.WriteLine($"Received message: '{messageText}' of type '{update.Message.Type}' in chat '{chatId}' {update.Message.Chat.Username} Language: {languageChat}");

            if (update.Message.Video is null)
            {
                var sendErro = await Telegram.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You need to send a video!"
                    );
                return;
            }

            if (update.Message.Video is not null)
            {
                // Get file information for the received video
                var fileInfo = await Telegram.GetFileAsync(update.Message.Video!.FileId);
                var filePath = Path.GetFileName(fileInfo.FilePath);

                // Define the destination file path to save the video
                string destinationFilePath = videosPath + filePath;

                Console.WriteLine($"Saving the video...");

                // Create a stream to save the video file
                await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                await Telegram.DownloadFileAsync(
                    filePath: fileInfo.FilePath,
                    destination: fileStream);

                trashVideo = destinationFilePath;

                fileStream.Close();

                // Set the input and output file paths for further processing
                string inputFilePath = destinationFilePath;
                string outputFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + ".mp3";

                trashAudio = outputFilePath;

                var sentMessage = await Telegram.SendTextMessageAsync(
                    chat,
                    "Configurações Selecionadas:"
                    );

                var configId = sentMessage.MessageId;
                var configMessage = sentMessage.Text;

                //#### QUESTION
                var choice = await Telegram.SendTextMessageAsync(chat, "What is the fontsize?", replyMarkup: new InlineKeyboardMarkup(new[]
                { new InlineKeyboardButton[] {
                    new("Default") { CallbackData = "10" }, new("12") { CallbackData = "12" }, new("15") { CallbackData = "15" }, new("18") { CallbackData = "18" } },
                  new InlineKeyboardButton[] {
                    new("20") { CallbackData = "20" }, new("22") { CallbackData = "22" }, new("25") { CallbackData = "25" }
                }}));
                var fontsize = await ButtonClicked(update, choice);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>fontsize:</b> " + fontsize;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                //#### QUESTION
                var choice2 = await Telegram.SendTextMessageAsync(chat, "What is the primaryColour?", replyMarkup: new InlineKeyboardMarkup(new[]
                { new InlineKeyboardButton[] {
                    new("Default") { CallbackData = "0" }, new("White") { CallbackData = "&HFFFFFF" }, new("Red") { CallbackData = "&H0000FF" }, new("Green") { CallbackData = "&H00FF00" }},
                  new InlineKeyboardButton[] {
                     new("Blue") { CallbackData = "&HFF0000" }, new("Magenta") { CallbackData = "&HFF00FF" }, new("Cyan") { CallbackData = "&HFFFF00" }, new("Yellow") { CallbackData = "&H00FFFF" }, new("Black") { CallbackData = "&H000000" }
                }}));
                var primaryColour = await ButtonClicked(update, choice2);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>primaryColour:</b> " + primaryColour;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                //#### QUESTION
                var choice4 = await Telegram.SendTextMessageAsync(chat, "What is the backColour?", replyMarkup: new InlineKeyboardMarkup(new[]
                { new InlineKeyboardButton[] {
                    new("Default") { CallbackData = "0" }, new("White") { CallbackData = "&HFFFFFF" }, new("Red") { CallbackData = "&H0000FF" }, new("Green") { CallbackData = "&H00FF00" }},
                  new InlineKeyboardButton[] {
                     new("Blue") { CallbackData = "&HFF0000" }, new("Magenta") { CallbackData = "&HFF00FF" }, new("Cyan") { CallbackData = "&HFFFF00" }, new("Yellow") { CallbackData = "&H00FFFF" }, new("Black") { CallbackData = "&H000000" }
                }}));
                var backColour = await ButtonClicked(update, choice4);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>backColour:</b> " + backColour;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                //#### QUESTION
                var choice7 = await Telegram.SendTextMessageAsync(chat, "What is the borderStyle?", replyMarkup: new InlineKeyboardMarkup(new[]
                { new InlineKeyboardButton[] {
                    new("Default") { CallbackData = "0" }, new("Outline + drop shadow") { CallbackData = "1" }},
                  new InlineKeyboardButton[] {
                      new("Opaque box") { CallbackData = "3"  }
                }}));
                var borderStyle = await ButtonClicked(update, choice7);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>borderStyle:</b> " + borderStyle;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                //#### QUESTION
                var choice3 = await Telegram.SendTextMessageAsync(chat, "What is the outlineColour?", replyMarkup: new InlineKeyboardMarkup(new[]
                { new InlineKeyboardButton[] {
                    new("Default") { CallbackData = "0" }, new("Transparent") { CallbackData = "&H40000000" }, new("White") { CallbackData = "&HFFFFFF" }, new("Red") { CallbackData = "&H0000FF" }, new("Green") { CallbackData = "&H00FF00" }},
                  new InlineKeyboardButton[] {
                     new("Blue") { CallbackData = "&HFF0000" }, new("Magenta") { CallbackData = "&HFF00FF" }, new("Cyan") { CallbackData = "&HFFFF00" }, new("Yellow") { CallbackData = "&H00FFFF" }, new("Black") { CallbackData = "&H000000" }
                }}));
                var outlineColour = await ButtonClicked(update, choice3);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>outlineColour:</b> " + outlineColour;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                //#### QUESTION
                var choice5 = await Telegram.SendTextMessageAsync(chat, "Is bold?", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[]
                {
                    new("Yes") { CallbackData = "1" }, new("No") { CallbackData = "0" }
                }));
                var bold = await ButtonClicked(update, choice5);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>bold:</b> " + bold;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                //#### QUESTION
                var choice6 = await Telegram.SendTextMessageAsync(chat, "Is italic?", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[]
                {
                    new("Yes") { CallbackData = "1" }, new("No") { CallbackData = "0" }
                }));
                var italic = await ButtonClicked(update, choice6);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                configMessage = configMessage + '\n' + "<b>italic:</b> " + italic;

                await Telegram.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: configId,
                    text: configMessage,
                     parseMode: ParseMode.Html
                    );

                var fontname = ""; //The fontname as used by Windows. Case-sensitive.
                int outline = 1; //If BorderStyle is 1,  then this specifies the width of the outline around the text, in pixels. Values may be 0, 1, 2, 3 or 4.
                double spacing = 0.8; //Extra space between characters. [pixels]
                double angle = 0.8; //The origin of the rotation is defined by the alignment. Can be a floating point number. [degrees]
                int alignment = 2; //(1-3 sub, 4-6 mid, 7-9 top)

                var forceStyleSubtitle = $"'" +
                            $"fontsize={fontsize}," +
                            $"primaryColour={primaryColour}," +
                            $"outlineColour={outlineColour}," +
                            $"borderStyle={borderStyle}," +
                            $"backColour={backColour}," +
                            $"bold={bold}," +
                            $"italic={italic}'";

                //       await Telegram.SendTextMessageAsync(chat, 
                //$"{forceStyleSubtitle}");

                // Generating subtitles in the audio language
                await Telegram.SendTextMessageAsync(
                chatId: chatId,
                text: "Generating subtitles..."
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

                await Telegram.SendTextMessageAsync(
                chatId: chatId,
                text: "Adding subtitles to the video...");

                string subtitledFilePath = videosPath + Path.GetFileNameWithoutExtension(filePath) + "_subtitled.mp4";

                trashSubtitled = subtitledFilePath;

                inputFilePath = inputFilePath.Replace('\\', '/').Replace("C:", "");
                subtitleFilePath = subtitleFilePath.Replace('\\', '/').Replace("C:", "");
                subtitledFilePath = subtitledFilePath.Replace('\\', '/').Replace("C:", "");

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
                    await Telegram.SendVideoAsync(
                        chatId: chatId,
                        video: InputFile.FromStream(stream)
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

                return;
            }
        }
        public override async Task OnGroupChat(Chat chat, UpdateInfo update)
        {
            Console.WriteLine($"In group chat {chat.Name()}");
            do
            {
                switch (update.UpdateKind)
                {
                    case UpdateKind.NewMessage:
                        Console.WriteLine($"{update.Message.From.Name()} wrote: {update.Message.Text}");
                        if (update.Message.Text == "/button@" + BotName)
                            await Telegram.SendTextMessageAsync(chat, "You summoned me!", replyMarkup: new InlineKeyboardMarkup("I grant your wish"));
                        break;
                    case UpdateKind.EditedMessage:
                        Console.WriteLine($"{update.Message.From.Name()} edited: {update.Message.Text}");
                        break;
                    case UpdateKind.CallbackQuery:
                        Console.WriteLine($"{update.Message.From.Name()} clicked the button with data '{update.CallbackData}' on the msg: {update.Message.Text}");
                        ReplyCallback(update, "Wish granted !");
                        break;
                }
                // in this approach, we choose to continue execution in a loop, obtaining new updates/messages for this chat as they come
            } while (await NextEvent(update) != 0);
        }
    }
}