using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotOnDScharp.SlashCommands
{
    
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("Clear", "Удаляет указанное количество сообщений")]
        public async Task Сlear(InteractionContext ctx, [Option("amount", "Количество сообщений для удаления")] long count)
        {
            try
            {
                var msgList = await ctx.Channel.GetMessagesAsync((int)count);
                if (count > msgList.Count)
                    throw new Exception("Число удаляемых сообщений не может превышать число существующих сообщений!");
                await ctx.Channel.DeleteMessagesAsync(msgList);

                var embMessage = new DiscordEmbedBuilder()
                {
                    Title = $"Успешно удалено {count} сообщений/я!",
                    Description = "Это сообщение автоматически удалится через пару секунд",
                    Color = DiscordColor.White,
                };
                
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("").AddEmbed(embMessage));
                await Task.Delay(TimeSpan.FromSeconds(2));
                
            }
            catch (Exception ex)
            {
                var message = new DiscordEmbedBuilder()
                {
                    Title = $"Что-то пошло не так",
                    Description = $"В ходе операции удаления произошла ошибка: {ex.Message}",
                    Color = DiscordColor.Red,
                };

                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("").AddEmbed(message));
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            finally
            {
                await ctx.DeleteResponseAsync();
            }
        }
        
        
        [SlashCommand("Vote", "Запускает голосование из двух вариантов")]
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
