using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordApp.Models;
using System.Collections.Specialized;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Reflection;

namespace DiscordApp
{
    public class Commands
    {
        public Commands()
        {

        }
        
        private Dictionary<string, BootCamper> bootcampers;
        public Dictionary<string, BootCamper> Bootcampers
        {
            get
            {
                if(bootcampers == null)
                {
                    bootcampers = new Dictionary<string, BootCamper>();
                }
                return bootcampers;
            }
        }

        DataAccess Data = new DataAccess();

        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext lCommandContext) // this command takes no arguments
        {
            await lCommandContext.ClearLastMessage();
            // let's trigger a typing indicator to let
            // users know we're working
            await lCommandContext.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(lCommandContext.Client, ":ping_pong:");

            // respond with ping
            await lCommandContext.RespondAsync($"{emoji} Pong! Ping: {lCommandContext.Client.Ping}ms");
        }

        [Command("poll"), Description("Run a poll with reactions.")]
        public async Task Poll(CommandContext lCommandContext, [Description("How long should the poll last.")] TimeSpan duration, [Description("What options should people have.")] params DiscordEmoji[] options)
        {
            // first retrieve the interactivity module from the client
            var interactivity = lCommandContext.Client.GetInteractivityModule();
            var poll_options = options.Select(xe => xe.ToString());

            // then let's present the poll
            var embed = new DiscordEmbedBuilder
            {
                Title = "Poll time!",
                Description = string.Join(" ", poll_options)
            };
            var msg = await lCommandContext.RespondAsync(embed: embed);

            // add the options as reactions
            for (var i = 0; i < options.Length; i++)
                await msg.CreateReactionAsync(options[i]);

            // collect and filter responses
            var poll_result = await interactivity.CollectReactionsAsync(msg, duration);
            var results = poll_result.Reactions.Where(xkvp => options.Contains(xkvp.Key))
                .Select(xkvp => $"{xkvp.Key}: {xkvp.Value}");

            // and finally post the results
            await lCommandContext.RespondAsync(string.Join("\n", results));
        }

        [Command("greet"), Description("Says hi to specified user."), Aliases("sayhi", "say_hi")]
        public async Task Greet(CommandContext lCommandContext, [Description("The user to say hi to.")] DiscordMember member)
        {
            await lCommandContext.ClearLastMessage();
            await lCommandContext.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(lCommandContext.Client, ":wave:");

            // and finally, let's respond and greet the user.
            await lCommandContext.RespondAsync($"{emoji} Hello, {member.Mention}!");
        }

        [Command("Login"), Description("Log the time you start your day"), Aliases("login", "log-in", "LOGIN")]
        public async Task Login(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();

            DateTime CurrentTime = DateTime.Now;
            if(DateTime.Now.Hour == 6 || DateTime.Now.Hour == 7)
            {
                CurrentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00);
            }

            string user = lCommandContext.User.Username;
            string Greeting = "";

            if ((DateTime.Now.Hour > 8 && DateTime.Now.Minute > 30) || DateTime.Now.Hour > 8)
            {
                Greeting = "Uh oh, you're late";
            }
            else if (DateTime.Now.Hour == 8)
            {
                Greeting = "Good morning!";
            }

            Data.Login(user, CurrentTime);
            BootCamper bc = Data.ViewBootCamperByUsername(user);
            Bootcampers.Add(user, bc);
            string Message = String.Format("{0} {1} you have been clocked in at {2}", Greeting, user, CurrentTime);
            await lCommandContext.TriggerTypingAsync();
            await lCommandContext.RespondAsync(Message);

        }

        [Command("Logout"), Description("Log the time you end your day"), Aliases("logout", "log-out", "LOGOUT")]
        public async Task Logout(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();

            DateTime CurrentTime = DateTime.Now;
            string user = lCommandContext.User.Username;
            Data.Logout(user, CurrentTime);
            string Message = String.Format("Thank you {0}, you have been clocked out at {1}", user, CurrentTime);
            await lCommandContext.TriggerTypingAsync();
            await lCommandContext.RespondAsync(Message);
        }

        [Command("ToLunch"), Description("Clock out for lunch"), Aliases("lunchin", "Lunchin", "LUNCHIN", "tolunch", "Tolunch", "TOLUNCH")]
        public async Task GoToLunch(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();

            DateTime CurrentTime = DateTime.Now;
            string user = lCommandContext.User.Username;
            string Greeting = "Thank you";

            Data.GoToLunch(user, CurrentTime);
            string Message = String.Format("{0} {1} you have been clocked out at {2}.  Have a good lunch!", Greeting, user, CurrentTime);
            await lCommandContext.TriggerTypingAsync();
            await lCommandContext.RespondAsync(Message);
        }

        [Command("FromLunch"), Description("Clock in from lunch"), Aliases("fromlunch", "FROMLUNCH")]
        public async Task BackFromLunch(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();

            //if (DateTime.Now.Hour >= 7 && DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.DayOfWeek == DayOfWeek.Thursday && DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour < 5)
            //{
            DateTime CurrentTime = DateTime.Now;
            if (DateTime.Now.Hour == 7)
            {
                CurrentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00);
            }
            string user = lCommandContext.User.Username;
            string Greeting = "Welcome Back";

            Data.BackFromLunch(user, CurrentTime);
            string Message = String.Format("{0} {1}! You have been clocked back in at {2}", Greeting, user, CurrentTime);
            await lCommandContext.TriggerTypingAsync();
            await lCommandContext.RespondAsync(Message);
        }

        [Command("Vocabulary"), Description("Helpful vocabulary words"), Aliases("vocabulary", "vocab", "Vocab")]
        public async Task Vocabulary(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();

            NameValueCollection Vocab = new NameValueCollection();
            Vocab["Instance"] = "";
            Vocab["Instantiate"] = "";
            Vocab["Initialize"] = "";
            Vocab["Declare"] = "";
            Vocab["Element"] = "";
            Vocab["Variable"] = "";
            Vocab["Parameter"] = "";
            Vocab["Argument"] = "";
            Vocab["Iterate"] = "";
            Vocab["Loop"] = "";
            Vocab["DataBase"] = "";
            Vocab["Connection"] = "";
            Vocab["Server"] = "";
            Vocab["Method"] = "A series of actions bundled into one statement, to encourage reuse of code.";
            Vocab["Algorithm"] = "Another fancy word for method.  A series of actions that are required to be performed to accomplish a task.";
            Vocab["Process"] = "";
            Vocab["Parse"] = "To break something down into its base components also may include changing the datatype";
            Vocab["Reference"] = "";
            Vocab["Program Flow"] = "";
            Vocab["Testing"] = "";
            Vocab["Scope"] = "";
        }

        [Command("meme"), Aliases("memes", "Meme")]
        public async Task Meme(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();
            var emoji = DiscordEmoji.FromName(lCommandContext.Client, ":no_entry:");

            var embed = new DiscordEmbedBuilder
            {
                Title = "That doesn't meme what you think it memes",
                Description = $"{emoji} Memes have been temporarily disabled.",
                Color = new DiscordColor(0XFF0000) //red
            };
            await lCommandContext.RespondAsync("", embed: embed);
        }
    }

    [Group("admin")] // let's mark this class as a command group
    [Description("Administrative commands.")] // give it a description for help purposes
    [Hidden] // let's hide this from the eyes of curious users
    [RequirePermissions(Permissions.ManageWebhooks)] // and restrict this to users who have appropriate permissions
    public class GrouppedCommands
    {
        DataAccess Data = new DataAccess();
        // all the commands will need to be executed as <prefix>admin <command> <arguments>

        [Command("AddBootCamper"), Description("Add all Bootcampers to database"), Aliases("AddUser", "adduser", "Adduser", "Addbootcamper", "AddBootcamper")]
        public async Task AddBootCampers(CommandContext lCommandContext, DiscordMember Member, string FirstName = "", string LastName = "")
        {
            await lCommandContext.ClearLastMessage();

            if (FirstName != "" && LastName != "")
            {
                Data.AddBootCampers(Member.Username, FirstName, LastName);
                string Message = String.Format("{0}, the Bootcamper {0} has been added to the database", lCommandContext.User.Username, Member.Username);

                await lCommandContext.TriggerTypingAsync();
                await lCommandContext.RespondAsync(Message);
            }
            else
            {
                await lCommandContext.TriggerTypingAsync();
                await lCommandContext.RespondAsync("Please try again only next time enter a first and last name for the bootcamper");
            }
        }

        [Command("UpdateTime"), Description("Syntax UpdateTime <member> <Hour> <Minute> <isLogin>"), Aliases("updatetime", "UpdateLogin", "Update", "update")]
        public async Task UpdateTimeIn(CommandContext lCommandContext, [Description("Mention the bootcamper to adjust time for")] DiscordMember member, [Description("Hour to be clocked in or out at")] int Hour, [Description("Minute to be clocked in or out at")] int Minute, [Description("true for login, false for logout")] bool isLogin)
        {
            if(Hour < 7)
            {
                Hour += 12;
            }

            //TODO: change logout to add +12 hours to Hour param
            await lCommandContext.ClearLastMessage();
            DateTime CurrentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour, Minute, 00);
            string User = member.Username;
            string LoginMessage = string.Format("Login time updated for {0}, new login time is {1}:{2}", User, Hour.ToString("D2"), Minute.ToString("D2"));
            string LogoutMessage = string.Format("Logout time updated for {0}, new logout time is {1}:{2}", User, Hour.ToString("D2"), Minute.ToString("D2"));
            if (isLogin == true)
            {
                Data.UpdateLogin(User, CurrentTime);
                await lCommandContext.TriggerTypingAsync();
                await lCommandContext.RespondAsync(LoginMessage);
            }
            else
            {
                Data.Logout(User, CurrentTime);
                await lCommandContext.TriggerTypingAsync();
                await lCommandContext.RespondAsync(LogoutMessage);
            }
        }

        #region Tickets
        [Command("SubmitTicket"), Description("Creates a ticket in the system."), Aliases("submitticket", "SUBMITTICKET", "submitTicket")]
        public async Task Ticket(CommandContext lCommandContext, string name, string content)
        {
            string responseMessage = "";
            bool success = Data.CreateTicket(new Ticket() { Name = name, Content = content, Status = "New", SubmittedBy = lCommandContext.User.Username });

            if (success)
            {
                responseMessage = "Your ticket has been created successfuly.";
            }
            else
            {
                responseMessage = "A data access issue has occurred.";
            }
            await lCommandContext.RespondWithMessageAsync(responseMessage);
        }

        [Command("ViewTickets"), Description("Views all open tickets, ordered by time."), Aliases("viewtickets", "viewTickets")]
        public async Task ViewTickets(CommandContext lCommandContext, string status = null)
        {
            await lCommandContext.ClearLastMessage();

            List<Ticket> allTickets = Data.ViewTickets(status);

            if (allTickets.Count > 0)
            {
                foreach (Ticket ticket in allTickets)
                {
                    await lCommandContext.EmbedTicket(ticket);
                }
            }
            else
            {
                await lCommandContext.TriggerTypingAsync();
                await lCommandContext.RespondWithMessageAsync("No tickets were found");
            }
        }

        [Command("UpdateTicket"), Description("Updates a ticket given the id, property and value."), Aliases("updateTicket", "updateticket")]
        public async Task UpdateTicket(CommandContext lCommandContext, Int64 iTicketId, string property, string value)
        {
            await lCommandContext.ClearLastMessage();
            bool success = false;
            string response = "";
            property = property.ToLower();
            Ticket ticket = Data.ViewTicketById(iTicketId);

            if (ticket.TicketId != 0)
            {
                bool propChanged = false;
                PropertyInfo[] info = ticket.GetType().GetProperties();
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i].Name.ToLower().Contains(property))
                    {
                        info[i].SetValue(ticket, value);
                        propChanged = true;
                    }
                }
                if (propChanged)
                {
                    success = Data.UpdateTicket(ticket);
                    if (success)
                    {
                        response = "Your ticket was updated successfuly";
                    }
                    else
                    {
                        response = "There was an issue updating your ticket";
                    }
                }
                else
                {
                    response = "Property not found";
                }
            }
            else
            {
                response = "Ticket not found.";
            }
            if (success)
            {
                Ticket savedTicket = Data.ViewTicketById(iTicketId);
                await lCommandContext.EmbedTicket(savedTicket);
            }
            await lCommandContext.TriggerTypingAsync();
            await lCommandContext.RespondWithMessageAsync(response);
        }
        #endregion

        [Command("Echo")]
        public async Task Echo(CommandContext iContext, string message)
        {
            await iContext.RespondWithMessageAsync(message);
        }
        
        [Command("ViewTimeSheet"), Aliases("vts", "viewtimesheet")]
        public async Task ViewBootCamperTimeSheet(CommandContext lCommandContext, DiscordMember member, int count = 5)
        {
            await lCommandContext.ClearLastMessage();
            BootCamper bc = Data.ViewBootCamperByUsername(member.Username);
            List<TimeSheet> timeSheets = Data.ViewBootCamperTimeSheet(member.Username, count);
            await lCommandContext.EmbedTimeSheets(bc, timeSheets);
        }

        [Command("CurrentlyLoggedIn"), Aliases("LoggedIn", "loggedin", "logged-in", "clockedin", "OnTheClock")]
        public async Task ViewAllLoggedIn(CommandContext lCommandContext)
        {
            await lCommandContext.ClearLastMessage();
            List<BootCamper> Campers = Data.ViewAllLoggedIn();
            StringBuilder sb = new StringBuilder();
            foreach (BootCamper camper in Campers)
            {
                int space = 16 - (camper.FirstName.Length + camper.LastName.Length + 1);
                sb.Append(String.Format("{0} {1} - Logged in at:{2}{3}", camper.FirstName, camper.LastName, camper.TimeLogs.FirstOrDefault().LoginTime.ToString("hh:mm:ss"), camper == Campers.Last() ? "" : Environment.NewLine));
            }
            await lCommandContext.TriggerTypingAsync();
            await lCommandContext.RespondAsync(sb.ToString());
        }

        [Command("Clear"), Aliases("cls", "CLS", "clear")]
        public async Task ClearMessages(CommandContext lCommandContext, int count = 25)
        {
            await lCommandContext.ClearLastMessage();

            if (count > 100) count = 100;

            await lCommandContext.ClearMessages(count);
        }

        // this command will be only executable by the bot's owner
        [Command("sudo"), Description("Executes a command as another user."), Hidden, RequireOwner]
        public async Task Sudo(CommandContext lCommandContext, [Description("Member to execute as.")] DiscordMember member, [RemainingText, Description("Command text to execute.")] string command)
        {
            // note the [RemainingText] attribute on the argument.
            // it will capture all the text passed to the command

            // let's trigger a typing indicator to let
            // users know we're working
            await lCommandContext.TriggerTypingAsync();

            // get the command service, we need this for
            // sudo purposes
            var cmds = lCommandContext.Client.GetCommandsNext();

            // and perform the sudo
            await cmds.SudoAsync(member, lCommandContext.Channel, command);
        }

        [Command("nick"), Description("Gives someone a new nickname."), RequirePermissions(Permissions.ManageNicknames)]
        public async Task ChangeNickname(CommandContext lCommandContext, [Description("Member to change the nickname for.")] DiscordMember member, [RemainingText, Description("The nickname to give to that user.")] string new_nickname)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await lCommandContext.TriggerTypingAsync();

            try
            {
                // let's change the nickname, and tell the 
                // audit logs who did it.
                await member.ModifyAsync(new_nickname, reason: $"Changed by {lCommandContext.User.Username} ({lCommandContext.User.Id}).");

                // let's make a simple response.
                var emoji = DiscordEmoji.FromName(lCommandContext.Client, ":+1:");

                // and respond with it.
                await lCommandContext.RespondAsync(emoji.ToString());
            }
            catch (Exception)
            {
                // oh no, something failed, let the invoker now
                var emoji = DiscordEmoji.FromName(lCommandContext.Client, ":-1:");
                await lCommandContext.RespondAsync(emoji.ToString());
            }
        }
    }
}



