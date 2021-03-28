﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Data
{
    public class PermissionMapping
    {
        public Dictionary<Int64, String[]> Mappings { get; set; }

        [JsonIgnore]
        public static readonly Dictionary<Int64, String> MappingTranslation = new()
        {
            { 0, "None" },
            { 1, "CreateInvite" },
            { 2, "KickMembers" },
            { 4, "BanMembers" },
            { 8, "Administrator" },
            { 16, "ManageChannels" },
            { 32, "ManageServer" },
            { 64, "AddReactions" },
            { 128, "ViewAuditLog" },
            { 256, "PriorityVoice" },
            { 512, "StreamVoice" },
            { 1024, "ViewChannels" },
            { 2048, "SendMessages" },
            { 4096, "SendTTS" },
            { 8192, "ManageMessages" },
            { 16384, "EmbedLinks" },
            { 32768, "AttachFiles" },
            { 65536, "ViewMessageHistory" },
            { 131072, "MentionEveryone" },
            { 262144, "UseExternalEmotes" },
            { 524288, "ViewServerInsights" },
            { 1048576, "ConnectVoice" },
            { 2097152, "SpeakVoice" },
            { 4194304, "MuteVoice" },
            { 8388608, "DeafenVoice" },
            { 16777216, "MoveVoice" },
            { 33554432, "VoiceActivity" },
            { 67108864, "ChangeNickname" },
            { 134217728, "ManageNicknames" },
            { 268435356, "ManageRoles" },
            { 536870912, "ManageWebhooks" },
            { 1073741824, "ManageEmotes" },
            { 2147483648, "ExecuteSlashCommand" }
        };

        public static PermissionMapping Deserialize(String modName)
        {
            StreamReader reader = modName switch
            {
                "vanilla" => new("./config/permissions/vanilla.mappings.json"),
                _ => new($"./mod-data/permissions/{modName}.mappings.json")
            };

            return JsonConvert.DeserializeObject<PermissionMapping>(reader.ReadToEnd());
        }

        public static PermissionMapping operator + (PermissionMapping left, PermissionMapping right)
        {
            PermissionMapping retValue = left;

            foreach(var v in right.Mappings)
            {
                if (!left.Mappings.ContainsKey(v.Key))
                    left.Mappings.Add(v.Key, v.Value);
                else
                    foreach (var v1 in v.Value)
                        left.Mappings[v.Key] = left.Mappings[v.Key].Append(v1).ToArray();
            }

            return retValue;
        }

        public static Dictionary<String, String[]> Export(PermissionMapping mapping)
        {
            Dictionary<String, String[]> result = new();
            foreach(var v in mapping.Mappings)
                result.Add(MappingTranslation[v.Key], v.Value);

            return result;
        }
    }
}
