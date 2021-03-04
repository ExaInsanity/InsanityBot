﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Locking
{
    public class Lock : BaseCommandModule
    {
        public async Task LockCommand(CommandContext ctx)
        {
            await LockCommand(ctx, ctx.Channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);
        }

        public async Task LockCommand(CommandContext ctx, DiscordChannel channel)
        {
            await LockCommand(ctx, channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);
        }

        public async Task LockCommand(CommandContext ctx, String args)
        {
            try
            {
                String cmdArguments = args;

                if (!args.Contains("-r") && !args.Contains("--reason"))
                    cmdArguments += " --reason usedefault";

                await Parser.Default.ParseArguments<LockOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await LockCommand(ctx, InsanityBot.HomeGuild.GetChannel(o.ChannelId), String.Join(' ', o.Reason), o.Silent);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.lock.failure"],
                        ctx),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.RespondAsync(embed: failed.Build());
            }
        }

        public async Task LockCommand(CommandContext ctx, DiscordChannel channel, String args)
        {
            await LockCommand(ctx, args + $" -c {channel.Id}");
        }

        private async Task LockCommand(CommandContext ctx, DiscordChannel channel, String reason = "usedefault", Boolean silent = false)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.lock"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String LockReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx),
                _ => GetFormattedString(reason, ctx)
            };

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "LOCK",
                Color = DiscordColor.Blue,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Channel", channel.Mention, true)
                .AddField("Reason", LockReason, true);

            try
            {
                channel.SerializeChannelData();
                ChannelData data = channel.GetCachedChannelData();

                await channel.AddOverwriteAsync(InsanityBot.HomeGuild.EveryoneRole, deny: Permissions.SendMessages, reason: "InsanityBot - locking channel");

                foreach(var v in data.LockedRoles)
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), deny: Permissions.SendMessages, reason: "InsanityBot - locking channel, removing access for listed roles");

                foreach (var v in data.WhitelistedRoles)
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), allow: Permissions.SendMessages, reason: "InsanityBot - locking channel, re-adding access for whitelisted roles");

                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.lock.success"], ctx),
                    Color = DiscordColor.Blue,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.lock.failure"], ctx),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                if(!silent)
                    await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }

    public class LockOptions : ModerationOptionBase
    {
        [Option('c', "channel", Default = 0, Required = false)]
        public UInt64 ChannelId { get; set; }
    }
}
