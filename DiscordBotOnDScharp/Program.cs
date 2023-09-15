using DiscordBotOnDScharp.Config;
using DiscordBotOnDScharp.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using DiscordBotOnDScharp.Services;
using static System.Net.WebRequestMethods;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using DiscordBotOnDScharp.SlashCommands;

namespace DiscordBotOnDScharp
{
    public sealed class Program
    {
        public static BadWordsService BadWords { get; set; }


        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {
            //1. Get the details of your config.json file by deserialising it
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            //2. Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            //3. Apply this config to our DiscordClient
            Client = new DiscordClient(discordConfig);

            //4. Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //5. Set up the Task Handler Ready event
            Client.Ready += OnClientReady;
            Client.MessageCreated += MessageCreatedHandler;

            //6. Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();

            //7. Register your commands
            Commands.RegisterCommands<Basic>();

            slashCommandsConfig.RegisterCommands<Slash>(738815836722299063);


            Commands.CommandErrored += CommandError;



            BadWords = new BadWordsService();
            await BadWords.LoadBadWordsAsync("badwords.txt");
            //8. Connect to get the Bot online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task CommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is ChecksFailedException exception)
            {
                string timeleft = String.Empty;
                foreach(var check in exception.FailedChecks)
                {
                    var cooldown = (CooldownAttribute)check;
                    timeleft = cooldown.GetRemainingCooldown(e.Context).ToString(@"hh\:mm\:ss");

                    var embedmsg = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Команда в откате",
                        Description = $"Осталось: {timeleft}"
                    };

                    await e.Context.Channel.SendMessageAsync(embedmsg);

                }
            }
        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            string message = e.Message.Content;
            if (e.Message.Author.Id == 675666415897935873)
            {
                var embedmsg = new DiscordEmbedBuilder
                {
                    ImageUrl = @"http://i.kym-cdn.com/entries/icons/facebook/000/037/848/cover2.jpg"
                };
                var botsmg = await e.Message.RespondAsync(embed: embedmsg);
                await Task.Delay(3000);
                await e.Channel.DeleteMessageAsync(botsmg);
            }
            if (BadWords.ContainsBadWord(message))
            {
                var embedmsg = new DiscordEmbedBuilder
                {
                    ImageUrl = @"https://sun9-50.userapi.com/impg/0wl1mY7ZCMa54KjiyZh1r5kBxboK6gceqAgZww/bAGTxUZIIzM.jpg?size=1920x1080&quality=96&sign=476412589a4796e8aa98f4cd7060a6b4&type=album"
                };

                await e.Message.RespondAsync(embed: embedmsg);
            }
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
