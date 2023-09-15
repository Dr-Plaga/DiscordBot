using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System.Net;

namespace DiscordBotOnDScharp.Commands
{
    public class Basic : BaseCommandModule
    {
        [Command("test")]
        [Cooldown(5,10,CooldownBucketType.Global)]
        public async Task TestCommand(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();

            var messageToRetrive = await interactivity.WaitForMessageAsync(message => message.Content == "Hallo");

            if (messageToRetrive.Result.Content == "Hallo")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} said Hello");
            }
        }
        [Command("test1")]
        public async Task Test1Command(CommandContext ctx)
        {
            var interactivity = Program.Client.GetInteractivity();

            var messageToReact = await interactivity.WaitForReactionAsync(message => message.Message.Id == 1148640791279960075);
            if (messageToReact.Result.Message.Id == 1148640791279960075)
            {
                await ctx.Channel.SendMessageAsync($":{messageToReact.Result.Emoji.Name}:");
            }

        }
        //[Command("Track")]
        //public async Task Track(CommandContext ctx, string nickname)
        //{
        //    string l = @"<div class=""valorant-rank-bg"" data-v-a2eef9ca data-v-ddc47ee8>";
        //    nickname = nickname.Replace(" ", "%20").Replace("#", "%23");
        //    WebRequest webRequest =  /*($@"https://tracker.gg/valorant/profile/riot/{nickname}/overview")*/;
        //    WebResponse response = null;
        //    try
        //    {
        //        webRequest.Headers.Add("User-Agent", "Gyan Rosling");
        //        response = await webRequest.GetResponseAsync();
        //    }
        //    catch (WebException ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    Stream stream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(stream);
        //    string text = await reader.ReadToEndAsync();
        //    text = text.Substring(text.IndexOf(l) + l.Length);
        //    text = text.Remove(text.IndexOf("</div>"));
        //    await ctx.Channel.SendMessageAsync(text);
        //}


        [Command("Vote")]
        public async Task Vote(CommandContext ctx, string option1, string option2, [RemainingText] string polltitle)
        {
            var interactivity = Program.Client.GetInteractivity();
            var pollTime = TimeSpan.FromSeconds(10);

            DiscordEmoji[] emojiOptions = { DiscordEmoji.FromName(Program.Client, ":one:"),
                                            DiscordEmoji.FromName(Program.Client, ":two:")};

            string optionsDescription = $"{emojiOptions[0]}     |    {option1}\n" +
                                        $"{emojiOptions[1]}     |    {option2}\n";


            var pollMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = polltitle,
                Description = optionsDescription,
            };

            var putReactOn = await ctx.Channel.SendMessageAsync(embed: pollMessage);

            foreach (var emoji in emojiOptions)
            {
                await putReactOn.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(putReactOn, pollTime);

            float count1 = 0;
            float count2 = 0;

            foreach (var emoji in totalReactions)
            {
                if (emoji.Emoji == emojiOptions[0])
                    count1 += emoji.Total;
                else if (emoji.Emoji == emojiOptions[1])
                    count2 += emoji.Total;
            }

            float totalVote = count1 + count2;

            double firstInPercent = Math.Round(count1 / totalVote * 100);
            double secondInPercent = Math.Round(count2 / totalVote * 100);


            string res = $"{emojiOptions[0]}: {count1} голосов -- {firstInPercent}%\n" +
                         $"{emojiOptions[1]}: {count2} голосов -- {secondInPercent}%";

            var embedmsg = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Результаты",
                Description = res,
            };
            await ctx.Channel.SendMessageAsync(embedmsg);
        }
    }
}
