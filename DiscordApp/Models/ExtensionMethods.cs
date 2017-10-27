using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordApp.Models
{
    public static class ExtensionMethods
    {
        public static async Task ClearLastMessage(this DSharpPlus.CommandsNext.CommandContext iContext)
        {
            IReadOnlyCollection<DiscordMessage> messages = await iContext.Channel.GetMessagesAsync(1);
            await iContext.Channel.DeleteMessageAsync(messages.FirstOrDefault());
        }

        public static async Task ClearMessages(this DSharpPlus.CommandsNext.CommandContext iContext, int count = 25)
        {
            IReadOnlyCollection<DiscordMessage> messages = await iContext.Channel.GetMessagesAsync(count);
            await iContext.Channel.DeleteMessagesAsync(messages);
        }

        public static async Task RespondWithMessageAsync(this DSharpPlus.CommandsNext.CommandContext iContext, string message)
        {
            await iContext.TriggerTypingAsync();
            string[] words = message.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                Thread.Sleep(500);
            }
            //120 wpm
            await iContext.RespondAsync(message);
        }

        public static async Task EmbedTicket(this DSharpPlus.CommandsNext.CommandContext iContext, Ticket iTicket)
        {
            await iContext.TriggerTypingAsync();
            await iContext.RespondAsync("", embed:
            new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = String.Format("{0} - {1}", iTicket.Name, iTicket.TicketId)
                },
                Title = iTicket.SubmittedBy,
                Description = iTicket.Content,
                Timestamp = iTicket.Date,
                Color = TranslateStatusToColor(iTicket.Status)
            });
        }

        public static async Task EmbedTimeSheets(this DSharpPlus.CommandsNext.CommandContext iContext, BootCamper iBootCamper, List<TimeSheet> iSheets)
        {
            await iContext.TriggerTypingAsync();
            StringBuilder sb = new StringBuilder();
            string dow = string.Empty;
            string login = string.Empty;
            string lunchIn = string.Empty;
            string lunchOut = string.Empty;
            string logout = string.Empty;
            foreach (TimeSheet timeSheet in iSheets)
            {
                if (timeSheet.LogoutTime.DayOfWeek == DayOfWeek.Friday)
                {
                    sb.Append(Environment.NewLine);
                }
                dow = timeSheet.LoginTime.DayOfWeek.ToString();
                login = timeSheet.LoginTime.ToString("hh:mm:ss");
                lunchIn = timeSheet.LunchInTime == DateTime.MinValue ? "" : timeSheet.LunchInTime.ToString("hh:mm:ss");
                lunchOut = timeSheet.LunchOutTime == DateTime.MinValue ? "" : timeSheet.LunchOutTime.ToString("hh:mm:ss");
                logout = timeSheet.LogoutTime == DateTime.MinValue ? "" : timeSheet.LogoutTime.ToString("hh:mm:ss");
                sb.Append($"{ dow } - { timeSheet.LoginTime.Day }{ Environment.NewLine }");
                sb.Append($"{ login } { lunchIn } { lunchOut } { logout }{ Environment.NewLine }");
            }

            await iContext.RespondAsync("", embed:
            new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = String.Format("{0} {1}", iBootCamper.FirstName, iBootCamper.LastName)
                },
                Description = sb.ToString(),
                Color = TranslateStatusToColor("New")
            });
        }

        private static DiscordColor TranslateStatusToColor(string iStatus)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            switch (iStatus)
            {
                case "New":
                    r = 56;
                    g = 226;
                    b = 4;
                    break;

                case "In Progress":
                    r = 4;
                    g = 37;
                    b = 224;
                    break;

                case "Over Due":
                    r = 255;
                    g = 0;
                    b = 0;
                    break;
                    
                default:
                    break;
            }
            return new DiscordColor(r, g, b);
        }
    }
}
