﻿using System;

using InsanityBot.Utility.Reference;

namespace InsanityBot.Utility
{
	public interface IConfigBuilder<ConfigType, ConfigBuilder, U> : IConfigBuilder
		where ConfigType : IConfiguration<U>
		where ConfigBuilder : IConfigBuilder
	{
		public ConfigType Config { get; set; }
		public ConfigBuilder AddConfigEntry(String Identifier, U DefaultValue);
		public ConfigBuilder RemoveConfigEntry(String Identifier);
	}
}
