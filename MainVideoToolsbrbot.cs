﻿using System;
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
using VideoToolsbrbot;

namespace VideoToolsbrbot
{
    public class MainVideoToolsbrbot : EasyBot
    {
        static void Main(string[] args)
        {
            string botToken = ConfigurationManager.AppSettings["BotToken"];

            var bot = new MainVideoToolsbrbot(botToken);
            bot.Run();
        }

        private string botToken;

        public MainVideoToolsbrbot(string botToken) : base(botToken)
        {
            this.botToken = botToken;
        }

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

                string forceStyleSubtitle = "";

                //#### QUESTION
                var choice5 = await Telegram.SendTextMessageAsync(chatId, "Want Customize Caption?", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[]
                {
                    new("Yes") { CallbackData = "1" }, new("No") { CallbackData = "0" }
                }));
                var wantCustomizeCaption = await ButtonClicked(update, choice5);
                ReplyCallback(update);
                await Telegram.DeleteMessageAsync(chatId, update.Message.MessageId);

                if (wantCustomizeCaption == "1")
                {
                    CustomizeCaption captionGenerator = new CustomizeCaption(botToken, chatId, Telegram, update);
                    forceStyleSubtitle = ":force_style=" + await captionGenerator.GenerateCaption();
                }

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
                    Arguments = $"-i \"{inputFilePath}\" -vf \"subtitles={subtitleFilePath}{forceStyleSubtitle}\" -y \"{subtitledFilePath}\"",
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