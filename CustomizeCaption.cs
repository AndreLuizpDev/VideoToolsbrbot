using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VideoToolsbrbot;

namespace VideoToolsbrbot
{
    public class CustomizeCaption : EasyBot
    {
        private int configId;
        private string configMessage;
        private long chatId;
        private UpdateInfo update;
        public readonly TelegramBotClient Telegram;

        public CustomizeCaption(string botToken, long chatId, TelegramBotClient telegram, UpdateInfo update) : base(botToken)
        {
            this.chatId = chatId;
            this.Telegram = telegram;
            this.update = update;
        }

        public async Task<string> GenerateCaption()
        {
            var sentMessage = await Telegram.SendTextMessageAsync(
                chatId,
                "Configurações Selecionadas:"
            );

            var configId = sentMessage.MessageId;
            var configMessage = sentMessage.Text;

            //#### QUESTION
            var choice = await Telegram.SendTextMessageAsync(chatId, "What is the fontsize?", replyMarkup: new InlineKeyboardMarkup(new[]
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
            var choice2 = await Telegram.SendTextMessageAsync(chatId, "What is the primaryColour?", replyMarkup: new InlineKeyboardMarkup(new[]
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
            var choice4 = await Telegram.SendTextMessageAsync(chatId, "What is the backColour?", replyMarkup: new InlineKeyboardMarkup(new[]
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
            var choice7 = await Telegram.SendTextMessageAsync(chatId, "What is the borderStyle?", replyMarkup: new InlineKeyboardMarkup(new[]
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
            var choice3 = await Telegram.SendTextMessageAsync(chatId, "What is the outlineColour?", replyMarkup: new InlineKeyboardMarkup(new[]
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
            var choice5 = await Telegram.SendTextMessageAsync(chatId, "Is bold?", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[]
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
            var choice6 = await Telegram.SendTextMessageAsync(chatId, "Is italic?", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[]
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

            // The last line you want to concatenate
            var forceStyleSubtitle = $"'" +
                $"fontsize={fontsize}," +
                $"primaryColour={primaryColour}," +
                $"outlineColour={outlineColour}," +
                $"borderStyle={borderStyle}," +
                $"backColour={backColour}," +
                $"bold={bold}," +
                $"italic={italic}'";

            return forceStyleSubtitle;
        }
    }

}
