﻿using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Manul.Modules
{
    public class RandomModule : ModuleBase<SocketCommandContext>
    {
        private const int DefaultMaxValue = 6;
        private const int DefaultMinValue = 1;
        private const int DefaultAmount = 1;
        private const int FailureRate = 5;
        private const int MaxAmount = 20;
        private static int _randomCounter;
        private static readonly string[] HandThings =
        {
            "колбаса", "Саныч", "колбоса", "прическа Даниэля", "клок шерсти",
            "арта", "твой диплом", "1 и 4", "пакетик чая «Досада»", "а ничего! Ахахах)))", "пирог от Лисички",
            "админские права на 0.001 сек.", "удостоверение оператора ЭВМ", "пакетик чая «Отрада»",
            "подкрутка", "ящик питеня", "блинчик с клубничным вареньем", "премиум аккаунт в Miodenus Project"
        };
        private readonly Random _random = new ();
        
        public RandomModule(DiscordSocketClient client) => client.ButtonExecuted += ChooseHandButtonHandler;

        [Command("rand"), Alias("r", "random", "р", "рандом", "ранд", "кубики", "кубик", "кости")]
        [Summary("кидаю за тебя кубики и смотрю, что выпадет)")]
        public async Task RandAsync([Summary("нижняя граница диапазона")] string minValue = "", 
                [Summary("верхняя граница диапазона")] string maxValue = "",
                [Summary("количество чисел")] string amount = "")
        {
            var result = new StringBuilder();
            var builder = new EmbedBuilder { Color = Config.EmbedColor };
            var intMinValue = DefaultMinValue;
            var intMaxValue = DefaultMaxValue;
            var intAmount = DefaultAmount;

            if (minValue != string.Empty && !int.TryParse(minValue, out intMinValue))
            {
                builder.Description = "**Вместо минимального значения я вижу какой-то мусор.**";
            }
            else if (maxValue != string.Empty && !int.TryParse(maxValue, out intMaxValue))
            {
                builder.Description = "**Вместо максимального значения я вижу какой-то мусор.**";
            }
            else if (amount != string.Empty && !int.TryParse(amount, out intAmount))
            {
                builder.Description = "**Вместо количества чисел я вижу некий HOPEWELL, что переводится как помойка.**";
            }
            else if (intMinValue == intMaxValue)
            {
                builder.Description = "**Минимальное значение равно макcимальному... На самом деле мне всё с тобой ясно, в том числе и результат, который ты ожидаешь увидеть... Но не увидишь)))**";
            }
            else if (intMinValue > intMaxValue)
            {
                builder.Description = "**Минимальное значение не может быть больше макcимального... Я запрещаю тебе такое запрашивать!**";
            }
            else if (intAmount > MaxAmount)
            {
                builder.Description = $"**Слишком много чисел ты просишь... Максимум {MaxAmount} я готов дать.**";
            }
            else if (intAmount <= 0)
            {
                builder.Description = $"**Сколько чисел ты хочешь? {intAmount}? Ты в порядке {Context.User.Mention}?)))**";
            }
            else if (_random.Next(100) + 1 <= FailureRate)
            {
                builder.Description = "**Тебе сегодня повезло... Ведь я не буду отвечать)))**";
            }
            else
            {
                intMaxValue++;    // Чтоб было включительно.
                
                for (var i = 0; i < intAmount; i++)
                {
                    result.Append(i == intAmount - 1 ? $"{_random.Next(intMinValue, intMaxValue)}"
                            : $"{_random.Next(intMinValue, intMaxValue)}, ");
                }

                if (Context.User.Username == "submarinecap" && _random.Next(100) + 1 <= 50)
                {
                    builder.Description = "**Тебе выпало: 1113**";
                }
                else if (Context.User.Username == "null me" && _randomCounter == 0)
                {
                    builder.Description = "**Тебе выпало: 1**";
                    _randomCounter++;
                }
                else if (Context.User.Username == "null me" && _randomCounter == 1)
                {
                    builder.Description = "**Тебе выпало: 4**";
                    _randomCounter++;
                }
                else
                {
                    builder.Description = $"**Тебе выпало: {result}**";
                }
            }
            
            await Context.Message.ReplyAsync(string.Empty, false, builder.Build());
        }
        
        [Command("hand"), Alias("рука", "угадай", "угадать", "ру", "ручка", "лапа", "спрячь", "спрятай", "guess")]
        [Summary("угадай в какой лапе)))")]
        public async Task RandHandAsync([Summary("Что спрятать")] string thing = "")
        {
            var builder = new EmbedBuilder { Color = Config.EmbedColor };

            if (string.IsNullOrEmpty(thing) || string.IsNullOrWhiteSpace(thing))
            {
                builder.Description = $"**Итак, {Context.User.Mention}, я спрятал кое-что)\nУгадай где))**";
                thing = HandThings[_random.Next(HandThings.Length)];
            }
            else
            {
                builder.Description = $"**Итак, {Context.User.Mention}, я спрятал {thing})\nУгадай где))**";
            }
            
            var leftButton = new ButtonBuilder();
            var rightButton = new ButtonBuilder();

            leftButton.Label = "Левая лапа";
            leftButton.CustomId = "left" + thing;
            leftButton.Style = ButtonStyle.Primary;
            rightButton.Label = "Правая лапа";
            rightButton.CustomId = "right" + thing;
            rightButton.Style = ButtonStyle.Success;

            await Context.Message.ReplyAsync(string.Empty, false, builder.Build(),
                    components:  new ComponentBuilder().WithButton(leftButton).WithButton(rightButton).Build());
        }
        
        private async Task ChooseHandButtonHandler(SocketMessageComponent component)
        {
            var builder = new EmbedBuilder { Color = Config.EmbedColor };
            var result = _random.Next(2);
            var leftButton = new ButtonBuilder();
            var rightButton = new ButtonBuilder();

            leftButton.Label = "Левая лапа";
            leftButton.CustomId = "left" + component.Data.CustomId.Substring(4);
            leftButton.Style = ButtonStyle.Primary;
            leftButton.IsDisabled = true;
            rightButton.Label = "Правая лапа";
            rightButton.CustomId = "right" + component.Data.CustomId.Substring(5);
            rightButton.Style = ButtonStyle.Success;
            rightButton.IsDisabled = true;

            if (component.Data.CustomId.StartsWith("left"))
            {
                builder.Description = $"**{component.User.Mention}, думаешь, что в левой?))**\n**{(result == 0 ? $"Ладно, угадал))\n Твой приз: {component.Data.CustomId.Substring(4)}" : $"Ахахах, не угадал))\nТеперь {component.Data.CustomId.Substring(4)} - мой приз))")}**";
            }
            else if (component.Data.CustomId.StartsWith("right"))
            {
                builder.Description = $"**{component.User.Mention}, думаешь, что в правой?))**\n**{(result == 1 ? $"Ладно, угадал))\n Твой приз: {component.Data.CustomId.Substring(5)}" : $"Ахахах, не угадал))\nТеперь {component.Data.CustomId.Substring(5)} - мой приз))")}**";
            }
            else
            {
                builder.Description = "**Даб, даб, не понял...**";
            }
            
            await component.UpdateAsync(x => { x.Components = new ComponentBuilder().WithButton(leftButton).WithButton(rightButton).Build(); });
            await component.FollowupAsync(string.Empty, new [] { builder.Build() });
        }
    }
}