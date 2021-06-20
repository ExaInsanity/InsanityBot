﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static System.Convert;
using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Modlog
{
	public partial class Modlog
	{
		[Command("verballog")]
		public async Task VerbalLogCommand(CommandContext ctx,
			DiscordMember user)
		{
			if (!ctx.Member.HasPermission("insanitybot.moderation.verballog"))
			{
				await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
				return;
			}
			try
			{
				UserModlog modlog = user.GetUserModlog();

				DiscordEmbedBuilder modlogEmbed = new()
				{
					Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.embed_title"],
						ctx, user),
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						Text = "InsanityBot 2020-2021"
					}
				};

				if (modlog.VerbalLog.Count == 0)
				{
					modlogEmbed.Color = DiscordColor.SpringGreen;
					modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.empty_modlog"],
						ctx, user);
					_ = ctx.Channel.SendMessageAsync(embed: modlogEmbed.Build());
				}
				else
				{
					if (!ToBoolean(InsanityBot.Config["insanitybot.commands.modlog.allow_verballog_scrolling"]))
					{
						modlogEmbed.Color = DiscordColor.Red;
						modlogEmbed.Description = user.CreateVerballogDescription();

						await ctx.Channel.SendMessageAsync(embed: modlogEmbed.Build());
					}
					else
					{
						modlogEmbed.Color = DiscordColor.Red;
						String embedDescription = user.CreateModlogDescription();

						IEnumerable<DSharpPlus.Interactivity.Page> pages = InsanityBot.Interactivity.GeneratePagesInEmbed(embedDescription, SplitType.Line, modlogEmbed);

						await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
					}
				}
			}
			catch (Exception e)
			{
				InsanityBot.Client.Logger.LogError(new EventId(1171, "VerbalLog"), $"Could not retrieve verbal logs: {e}: {e.Message}");

				DiscordEmbedBuilder failedModlog = new()
				{
					Color = DiscordColor.Red,
					Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.failed"], ctx, user),
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						Text = "InsanityBot 2020-2021"
					}
				};
				await ctx.Channel.SendMessageAsync(embed: failedModlog.Build());
			}
		}

		[Command("verballog")]
		public async Task VerbalLogCommand(CommandContext ctx)
		{
			await this.VerbalLogCommand(ctx, ctx.Member);
		}
	}
}
