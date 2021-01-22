﻿using System;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static System.Convert;
using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public partial class Warn : BaseCommandModule
    {
        [Command("warn")]
        public async Task WarnCommand(CommandContext ctx,
            DiscordMember target,

            [RemainingText]
            String arguments = "usedefault")
        {
            if (arguments.StartsWith('-'))
            {
                await ParseWarnCommand(ctx, target, arguments);
                return;
            }
            await ExecuteWarn(ctx, target, arguments, false, false);
        }

        private async Task ParseWarnCommand(CommandContext ctx, DiscordMember target, String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                    cmdArguments += " --reason usedefault";

                await Parser.Default.ParseArguments<WarnOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteWarn(ctx, target, String.Join(' ', o.Reason), o.Silent, o.DmMember);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"],
                        ctx, target),
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

        private async Task ExecuteWarn(CommandContext ctx,
            DiscordMember target,
            String reason,
            Boolean silent, 
            Boolean dmMember)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.warn"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //if silent - delete the warn command
            if (silent)
                await ctx.Message.DeleteAsync();

            //actually do something with the usedefault value
            String WarnReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, target),
                _ => GetFormattedString(reason, ctx, target)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "Warn",
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", target.Mention, true)
                .AddField("Reason", WarnReason, true);

            try
            {
                target.AddModlogEntry(ModlogEntryType.warn, WarnReason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.success"],
                        ctx, target),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"],
                        ctx, target),
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
                if(dmMember)
                {
                    embedBuilder.Description = GetReason(InsanityBot.LanguageConfig["insanitybot.moderation.warn.reason"], WarnReason);
                    await (await target.CreateDmChannelAsync()).SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }
    }


    public class WarnOptions : ModerationOptionBase
    {
        
    }
}
