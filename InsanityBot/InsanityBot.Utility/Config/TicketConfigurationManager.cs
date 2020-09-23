﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Config
{
    public class TicketConfigurationManager : IConfigSerializer<TicketConfiguration>, IConfigBuilder<TicketConfiguration, TicketConfigurationManager>
    {
        public TicketConfiguration Config { get; set; }

        public TicketConfigurationManager AddConfigEntry(String Identifier, Object DefaultValue)
        {
            Config.Configuration.Add(Identifier, DefaultValue);
            return this;
        }

        public TicketConfigurationManager RemoveConfigEntry(String Identifier)
        {
            Config.Configuration.Remove(Identifier);
            return this;
        }

        public TicketConfiguration Deserialize(String Filename)
        {
            using StreamReader reader = new StreamReader(File.OpenRead(Filename));
            return JsonConvert.DeserializeObject<TicketConfiguration>(reader.ReadToEnd());
        }

        public void Serialize(TicketConfiguration Config, String Filename)
        {
            using StreamWriter writer = new StreamWriter(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
