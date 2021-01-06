using AellaDiscordBot.Bots.Commons;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AellaDiscordBot.Bots.Commands
{
    public class CommandGacha : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong! Test").ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(DiscordEmoji.FromName(ctx.Client, ":highquality:"));
        }

        [Command("gacha"), Description("Pull 1 time from the Gacha pool by reacting to the present! (1000 :cookie: req)")]
        public async Task Gacha(CommandContext ctx)
        {
            if (DBConnector.RemoveCurrency(ctx.User.Id.ToString(), 1000))
            {
                var interactivity = ctx.Client.GetInteractivity();

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Lunae TCG",
                    Description = $"**{ctx.User.Username}**, Add a gift reaction to open your Lunae TCG 1x Lootbox! (Valid for 2mins)",
                    Url = ctx.Client.CurrentUser.AvatarUrl,
                    ImageUrl = "https://i.pinimg.com/originals/fd/2c/1a/fd2c1a96b654e220d09525f006482477.gif",
                    Color = DiscordColor.Green,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "Valid for 2mins, failure to open this lootbox within 2 mins of the msg being sent the lootbox is voided."
                    }
                };

                var msg = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":gift:")).ConfigureAwait(false);

                var message = await interactivity
                    .WaitForReactionAsync(x =>
                        x.Channel == ctx.Channel
                        && x.User == ctx.User
                        && x.Emoji == DiscordEmoji.FromName(ctx.Client, ":gift:"))
                    .ConfigureAwait(false);

                await ctx.TriggerTypingAsync().ConfigureAwait(false);
                await msg.DeleteAsync().ConfigureAwait(false);

                Card card = GachaTable.Instance.GetRandomCard();

                DiscordColor color = DiscordColor.Black;
                string hqIcons = "";
                var hqIcon = DiscordEmoji.FromName(ctx.Client, ":highquality:");

                switch (card.Rarity)
                {
                    case Rarity.UQ:
                        color = DiscordColor.Gold;
                        hqIcons = $"{hqIcon}{hqIcon}{hqIcon}{hqIcon}{hqIcon}";
                        break;
                    case Rarity.HHQ:
                        color = DiscordColor.Orange;
                        hqIcons = $"{hqIcon}{hqIcon}{hqIcon}{hqIcon}";
                        break;
                    case Rarity.HQ:
                        color = DiscordColor.Purple;
                        hqIcons = $"{hqIcon}{hqIcon}{hqIcon}";
                        break;
                    case Rarity.NQP:
                        color = DiscordColor.Green;
                        hqIcons = $"{hqIcon}{hqIcon}";
                        DBConnector.AddCurrency(ctx.User.Id.ToString(), 150);
                        break;
                    case Rarity.NQ:
                        color = DiscordColor.Gray;
                        hqIcons = $"{hqIcon}";
                        DBConnector.AddCurrency(ctx.User.Id.ToString(), 100);
                        break;
                }

                var cardEmbed = new DiscordEmbedBuilder
                {
                    Title = $"{DiscordEmoji.FromName(ctx.Client, $":{card.Rarity}:")} {card.Name}",
                    Description = $"**{ctx.User.Username}**, you pulled a {DiscordEmoji.FromName(ctx.Client, $":{card.Rarity.ToString()}:")} {hqIcons}",
                    ImageUrl = $"{card.Filename}",
                    Color = color
                };

                await ctx.Channel.SendMessageAsync(embed: cardEmbed).ConfigureAwait(false);

                DBConnector.AddCard(ctx.User.Id.ToString(), card.Id);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username}, You require 1000 {DiscordEmoji.FromName(ctx.Client, ":cookie:")} to open a Lunae TCG Crate!").ConfigureAwait(false);
            }
        }

        [Command("gacha5"), Description("Pull 5 times from the Gacha pool by reacting to the present! (5000 :cookie: req, 1 bonus pull inculded)")]
        public async Task Gacha5(CommandContext ctx)
        {
            if (DBConnector.RemoveCurrency(ctx.User.Id.ToString(), 4000))
            {
                var interactivity = ctx.Client.GetInteractivity();

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Lunae TCG",
                    Description = $"**{ctx.User.Username}**, Add a gift reaction to open your Lunae TCG 6x Lootbox! (Valid for 2mins)",
                    Url = ctx.Client.CurrentUser.AvatarUrl,
                    ImageUrl = "https://i.pinimg.com/originals/fd/2c/1a/fd2c1a96b654e220d09525f006482477.gif",
                    Color = DiscordColor.Green,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "Valid for 2mins, failure to open this lootbox within 2 mins of the msg being sent the lootbox is voided."
                    }
                };

                var msg = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":gift:")).ConfigureAwait(false);

                var message = await interactivity
                    .WaitForReactionAsync(x =>
                        x.Channel == ctx.Channel
                        && x.User == ctx.User
                        && x.Emoji == DiscordEmoji.FromName(ctx.Client, ":gift:"))
                    .ConfigureAwait(false);

                await ctx.TriggerTypingAsync().ConfigureAwait(false);
                await msg.DeleteAsync().ConfigureAwait(false);

                Page[] pages = new Page[5];

                //Pull 5 cards
                for (int i = 0; i < 5; i++)
                {
                    Card card = GachaTable.Instance.GetRandomCard();

                    DiscordColor color = DiscordColor.Black;
                    string hqIcons = "";
                    var hqIcon = DiscordEmoji.FromName(ctx.Client, ":highquality:");

                    switch (card.Rarity)
                    {
                        case Rarity.UQ:
                            color = DiscordColor.Gold;
                            hqIcons = $"{hqIcon}{hqIcon}{hqIcon}{hqIcon}{hqIcon}";
                            break;
                        case Rarity.HHQ:
                            color = DiscordColor.Orange;
                            hqIcons = $"{hqIcon}{hqIcon}{hqIcon}{hqIcon}";
                            break;
                        case Rarity.HQ:
                            color = DiscordColor.Purple;
                            hqIcons = $"{hqIcon}{hqIcon}{hqIcon}";
                            break;
                        case Rarity.NQP:
                            color = DiscordColor.Green;
                            hqIcons = $"{hqIcon}{hqIcon}";
                            DBConnector.AddCurrency(ctx.User.Id.ToString(), 150);
                            break;
                        case Rarity.NQ:
                            color = DiscordColor.Gray;
                            hqIcons = $"{hqIcon}";
                            DBConnector.AddCurrency(ctx.User.Id.ToString(), 100);
                            break;
                    }

                    var cardEmbed = new DiscordEmbedBuilder
                    {
                        Title = $"Card {i + 1}: {DiscordEmoji.FromName(ctx.Client, $":{card.Rarity}:")} {card.Name}",
                        Description = $"**{ctx.User.Username}**, you pulled a {DiscordEmoji.FromName(ctx.Client, $":{card.Rarity}:")} {hqIcons}",
                        ImageUrl = $"{card.Filename}",
                        Color = color
                    };

                    DBConnector.AddCard(ctx.User.Id.ToString(), card.Id);

                    pages[i] = new Page("", cardEmbed);
                }

                PaginationEmojis paginationEmojis = GetPaginationEmojis(ctx.Client);

                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, paginationEmojis, PaginationBehaviour.WrapAround, PaginationDeletion.DeleteEmojis, TimeSpan.FromMinutes(2)).ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username}, You require 4000 {DiscordEmoji.FromName(ctx.Client, ":cookie:")} to open a Lunae TCG Crate!").ConfigureAwait(false);
            }
        }

        [Command("collections"), Description("Shows what cards you have collected")]
        public async Task Collection(CommandContext ctx, DiscordUser user = null)
        {
            if (user == null) user = ctx.User;

            List<Card> cardsCollected = DBConnector.GetUserCards(user.Id.ToString());
            var interactivity = ctx.Client.GetInteractivity();

            List<string> uqCards = cardsCollected.Where(c => c.Rarity == Rarity.UQ).Select(c => $"{c.Id}: {c.Name} ({c.Amount})").ToList();
            List<string> hhqCards = cardsCollected.Where(c => c.Rarity == Rarity.HHQ).Select(c => $"{c.Id}: {c.Name} ({c.Amount})").ToList();
            List<string> hqCards = cardsCollected.Where(c => c.Rarity == Rarity.HQ).Select(c => $"{c.Id}: {c.Name} ({c.Amount})").ToList();
            List<string> nqpCards = cardsCollected.Where(c => c.Rarity == Rarity.NQP).Select(c => $"{c.Id}: {c.Name} ({c.Amount})").ToList();
            List<string> nqCards = cardsCollected.Where(c => c.Rarity == Rarity.NQ).Select(c => $"{c.Id}: {c.Name} ({c.Amount})").ToList();

            int totalCards = 0;

            cardsCollected.ForEach(c => totalCards += c.Amount);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{user.Username} Cards Collected",
                Description = $"{user.Username} has collected ({cardsCollected.Count}/{GachaTable.Instance.Cards.Count}) ({totalCards} cards)",
                Color = DiscordColor.Gold
            };

            List<DiscordEmbedBuilder> embeds = new List<DiscordEmbedBuilder>();

            embeds.AddRange(GeneratePageText(uqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":uq:")} UQ Cards ({uqCards.Count}/{GachaTable.Instance.Cards.Where(c => c.Rarity == Rarity.UQ).ToList().Count})**", embed));
            embeds.AddRange(GeneratePageText(hhqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":hhq:")} HHQ Cards ({hhqCards.Count}/{GachaTable.Instance.Cards.Where(c => c.Rarity == Rarity.HHQ).ToList().Count})**", embed));
            embeds.AddRange(GeneratePageText(hqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":hq:")} HQ Cards ({hqCards.Count}/{GachaTable.Instance.Cards.Where(c => c.Rarity == Rarity.HQ).ToList().Count})**", embed));
            embeds.AddRange(GeneratePageText(nqpCards, $"**{DiscordEmoji.FromName(ctx.Client, ":nqp:")} NQP Cards ({nqpCards.Count}/{GachaTable.Instance.Cards.Where(c => c.Rarity == Rarity.NQP).ToList().Count})**", embed));
            embeds.AddRange(GeneratePageText(nqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":nq:")} NQ Cards ({nqCards.Count}/{GachaTable.Instance.Cards.Where(c => c.Rarity == Rarity.NQ).ToList().Count})**", embed));

            Page[] pages = new Page[embeds.Count];

            for (int i = 0; i < embeds.Count; i++)
            {
                pages[i] = new Page("", embeds[i]);
            }

            PaginationEmojis paginationEmojis = GetPaginationEmojis(ctx.Client);

            await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, paginationEmojis, PaginationBehaviour.WrapAround, PaginationDeletion.DeleteEmojis, TimeSpan.FromMinutes(1)).ConfigureAwait(false);
        }

        [Command("cards"), Description("Lists out all avaliable cards")]
        public async Task Cards(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            List<Card> cards = GachaTable.Instance.Cards;

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Cards Database ({cards.Count} cards)",
                Color = DiscordColor.Gold
            };

            try
            {
                List<string> uqCards = cards.Where(c => c.Rarity == Rarity.UQ).Select(c => $"{c.Id}: {c.Name}").ToList();
                List<string> hhqCards = cards.Where(c => c.Rarity == Rarity.HHQ).Select(c => $"{c.Id}: {c.Name}").ToList();
                List<string> hqCards = cards.Where(c => c.Rarity == Rarity.HQ).Select(c => $"{c.Id}: {c.Name}").ToList();
                List<string> nqpCards = cards.Where(c => c.Rarity == Rarity.NQP).Select(c => $"{c.Id}: {c.Name}").ToList();
                List<string> nqCards = cards.Where(c => c.Rarity == Rarity.NQ).Select(c => $"{c.Id}: {c.Name}").ToList();

                List<DiscordEmbedBuilder> embeds = new List<DiscordEmbedBuilder>();

                embeds.AddRange(GeneratePageText(uqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":uq:")} UQ Cards ({uqCards.Count})**", embed));
                embeds.AddRange(GeneratePageText(hhqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":hhq:")} HHQ Cards ({hhqCards.Count})**", embed));
                embeds.AddRange(GeneratePageText(hqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":hq:")} HQ Cards ({hqCards.Count})**", embed));
                embeds.AddRange(GeneratePageText(nqpCards, $"**{DiscordEmoji.FromName(ctx.Client, ":nqp:")} NQP Cards ({nqpCards.Count})**", embed));
                embeds.AddRange(GeneratePageText(nqCards, $"**{DiscordEmoji.FromName(ctx.Client, ":nq:")} NQ Cards ({nqCards.Count})**", embed));

                PaginationEmojis paginationEmojis = GetPaginationEmojis(ctx.Client);

                Page[] pages = new Page[embeds.Count];

                for (int i = 0; i < embeds.Count; i++)
                {
                    pages[i] = new Page("", embeds[i]);
                }

                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, paginationEmojis, PaginationBehaviour.WrapAround, PaginationDeletion.DeleteEmojis, TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        private List<DiscordEmbedBuilder> GeneratePageText(List<string> cards, string rarity, DiscordEmbed parentEmbed)
        {
            List<DiscordEmbedBuilder> embeds = new List<DiscordEmbedBuilder>();

            for (int i = 0; i < cards.Count; i += 13)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder(parentEmbed);

                embed.AddField($"{rarity}", string.Join("\n", cards.GetRange(i, Math.Min(13, cards.Count - i))), false);

                embeds.Add(embed);
            }

            return embeds;
        }

        private PaginationEmojis GetPaginationEmojis(DiscordClient client)
        {
            return new PaginationEmojis
            {
                Left = DiscordEmoji.FromName(client, ":arrow_left:"),
                Right = DiscordEmoji.FromName(client, ":arrow_right:")
            };
        }

        [Command("card"), Description("View a specific card")]
        public async Task Card(CommandContext ctx, int id = 0)
        {
            Card card = GachaTable.FindCard(id);

            DiscordColor color = DiscordColor.Black;

            switch (card.Rarity)
            {
                case Rarity.UQ:
                    color = DiscordColor.Gold;
                    break;
                case Rarity.HHQ:
                    color = DiscordColor.Orange;
                    break;
                case Rarity.HQ:
                    color = DiscordColor.Purple;
                    break;
                case Rarity.NQP:
                    color = DiscordColor.Green;
                    break;
                case Rarity.NQ:
                    color = DiscordColor.Gray;
                    break;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{DiscordEmoji.FromName(ctx.Client, $":{card.Rarity}:")} {card.Name}",
                ImageUrl = card.Filename,
                Color = color
            };

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}
