using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AellaDiscordBot.Bots.Commands
{
    public class CommandRaids : BaseCommandModule
    {
        [Command("raidstart"), Aliases("rs"), RequireRoles(RoleCheckMode.Any, "Raiders")]
        public async Task RaidStart(CommandContext ctx, int limit = 8, int subs = 0)
        {
            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
            {
                await ctx.Channel.SendMessageAsync("You need to be a voice channel!").ConfigureAwait(false);
                return;
            }


        }

        [Command("raidend"), Aliases("re"), RequireRoles(RoleCheckMode.Any, "Raiders")]
        public async Task RaidEnd(CommandContext ctx)
        {
            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
            {
                await ctx.Channel.SendMessageAsync("You need to be a voice channel!").ConfigureAwait(false);
                return;
            }

        }

        [Command("e8s")]
        public async Task RaidMove(CommandContext ctx)
        {
            var channel1 = ctx.Guild.GetChannel(665777482955489285);
            var channel2 = ctx.Guild.GetChannel(623766866644434969);
            var interactivity = ctx.Client.GetInteractivity();

            try
            {
                var member1 = await ctx.Guild.GetMemberAsync(204114634477666304).ConfigureAwait(false);
                var member2 = await ctx.Guild.GetMemberAsync(337071607468130307).ConfigureAwait(false);
                var member3 = await ctx.Guild.GetMemberAsync(279781006716698634).ConfigureAwait(false);
                var member4 = await ctx.Guild.GetMemberAsync(178414494198923264).ConfigureAwait(false);

                await channel1.PlaceMemberAsync(member1).ConfigureAwait(false);
                await channel1.PlaceMemberAsync(member2).ConfigureAwait(false);
                await channel1.PlaceMemberAsync(member3).ConfigureAwait(false);
                await channel1.PlaceMemberAsync(member4).ConfigureAwait(false);

                await interactivity.WaitForUserTypingAsync(ctx.User, ctx.Channel, TimeSpan.FromMinutes(3)).ConfigureAwait(false);

                await channel2.PlaceMemberAsync(member1).ConfigureAwait(false);
                await channel2.PlaceMemberAsync(member2).ConfigureAwait(false);
                await channel2.PlaceMemberAsync(member3).ConfigureAwait(false);
                await channel2.PlaceMemberAsync(member4).ConfigureAwait(false);
            }
            catch
            {}
        }
    }
}
