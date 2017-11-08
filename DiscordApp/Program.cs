using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using System.IO;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.EventArgs;
using DSharpPlus.Net;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using System.Threading;

namespace DiscordApp
{
    class Program
    {
        string _CommandLog = @"C:\Users\alexander.leon\Documents\Visual Studio 2017\Projects\BootCampDiscordApp3.0\DiscordApp\CommandLog";
        public DiscordClient Client { get; set; }
        public CommandsNextModule _Commands { get; set; }

        static void Main(string[] args)
        {
            //Entry method cannot be asynchronous, pass execution to asynchronous code
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            //Load configuration files
            var json = "";
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            //Load values from json file to client configuratioin
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            var cfg = new DiscordConfiguration
            {
                Token = cfgjson.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            //Instantiate a new client
            this.Client = new DiscordClient(cfg);

            this.Client.SetWebSocketClient<DSharpPlus.Net.WebSocket.WebSocket4NetClient>();

            //hook events for commandline display
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientErrored += this.Client_ClientError;

            var ccfg = new CommandsNextConfiguration
            {
                // String prefix defined in config.json
                StringPrefix = cfgjson.CommandPrefix,
                // Enable responding in direct messages
                EnableDms = true,
                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true                
            };

            //Hook up commands
            this._Commands = this.Client.UseCommandsNext(ccfg);

            //Hook command events for display
            this._Commands.CommandExecuted += this.Commands_CommandExecuted;
            this._Commands.CommandErrored += this.Commands_CommandErrored;

            //Add converter for custom type and name
            var mathopcvt = new MathOperationConverter();
            CommandsNextUtilities.RegisterConverter(mathopcvt);
            CommandsNextUtilities.RegisterUserFriendlyTypeName<MathOperation>("operation");

            //register commands
            this._Commands.RegisterCommands<Commands>();
            this._Commands.RegisterCommands<GrouppedCommands>();

            // Connect and log in
            await this.Client.ConnectAsync();
            

            //Prevent premature quitting
            await Task.Delay(-1);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            // Let's log the fat that this event occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BootcampBot", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BootcampBot", $"Guild available: {e.Guild.Name}", DateTime.Now);            
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "BootcampBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "BootcampBot", $"{e.Context.User.Username} Successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            StreamWriter lWriter = new StreamWriter(_CommandLog, true);
            lWriter.Write("{0} {1} {2} {3}", "BootcampBot", $"{e.Context.User.Username} Successfully executed '{e.Command.QualifiedName}'", DateTime.Now, Environment.NewLine);
            lWriter.Close();
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            //Log the name of the guild sent to client
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "BootcampBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}:  {e.Exception.Message ?? "<no message>"}", DateTime.Now);
            var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");


            if (e.Exception is CommandNotFoundException)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Try again",
                    Description = $"{emoji} That command doesn't exist.",
                    Color = new DiscordColor(0XFF0000) //red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
            else if(e.Exception is System.ArgumentException)
            {
                Program prog = new Program();
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("Whoa, hang on a second something went wrong.  I'll be right back!");
                await prog.Client.DisconnectAsync();
                Thread.Sleep(1000);
                await prog.Client.ReconnectAsync();
                await e.Context.TriggerTypingAsync();
                await e.Context.RespondAsync("Alright i'm back");
            }

            StreamWriter lWriter = new StreamWriter(_CommandLog, true);
            lWriter.Write("{0} {1} {2}", "BootcampBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}:  {e.Exception.Message ?? "<no message>"}", DateTime.Now, Environment.NewLine);

            lWriter.Close();
            // is error due to lack of permission?            
            if (e.Exception is ChecksFailedException ex)
            {
                //yes, the user lacks required permission, let them know

                var Error = new DiscordEmbedBuilder
                {
                    Title = "Access Denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0XFF0000) //red
                };
                await e.Context.RespondAsync("", embed: Error);
            }
        }
    }
        public struct ConfigJson
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("prefix")]
            public string CommandPrefix { get; private set; }
        }
    }

