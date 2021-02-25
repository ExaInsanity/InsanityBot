﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

#pragma warning disable CA2211
namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static CommandLineOptions CommandLineOptions;

        public static MainConfigurationManager ConfigManager;
        public static MainConfiguration Config;

        public static DiscordConfiguration ClientConfiguration;
        public static DiscordClient Client;

        public static DiscordGuild HomeGuild;

        public static CommandsNextExtension CommandsExtension;
        public static CommandsNextConfiguration CommandConfiguration;

        public static LanguageConfigurationManager LanguageManager;
        public static LanguageConfiguration LanguageConfig;
    }
}
#pragma warning restore CA2211
