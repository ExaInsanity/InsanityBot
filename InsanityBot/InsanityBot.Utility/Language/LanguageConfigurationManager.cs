﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Language
{
    public class LanguageConfigurationManager : IConfigBuilder<LanguageConfiguration, LanguageConfigurationManager, String>,
        IConfigSerializer<LanguageConfiguration, String>
    {
        public LanguageConfiguration Config { get; set; }

        public LanguageConfigurationManager AddConfigEntry(String Identifier, String DefaultValue)
        {
            Config.Configuration.Add(Identifier, DefaultValue);
            return this;
        }

        public LanguageConfiguration Deserialize(String Filename)
        {
            StreamReader reader = new StreamReader(File.OpenRead(Filename));
            return JsonConvert.DeserializeObject<LanguageConfiguration>(reader.ReadToEnd());
        }

        public LanguageConfigurationManager RemoveConfigEntry(String Identifier)
        {
            Config.Configuration.Remove(Identifier);
            return this;
        }

        public void Serialize(LanguageConfiguration Config, String Filename)
        {
            using StreamWriter writer = new StreamWriter(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
