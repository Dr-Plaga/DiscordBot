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
    }
}
