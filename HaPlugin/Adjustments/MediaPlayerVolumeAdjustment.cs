namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;

    public class MediaPlayerVolumeAdjustment : PluginDynamicAdjustment
    {
        private HaPlugin plugin;

        public MediaPlayerVolumeAdjustment() : base(true)
        {
            this.GroupName = "Volume";
            this.ResetDisplayName = "Mute";
        }

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"{this.GroupName}Command.OnLoad() => StatesReady");

                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (this.IsVolume(state))
                    {
                        this.AddParameter(state.Entity_Id, state.FriendlyName, "Volume");
                    }
                }

                PluginLog.Info($"[group: {this.GroupName}] [count: {this.GetParameters().Length}]");
            };

            this.plugin.StateChanged += (sender, e) => this.ActionImageChanged(e.Entity_Id);

            return true;
        }

        public const Int32 SUPPORT_VOLUME_SET = 4;
        //public const Int32 SUPPORT_VOLUME_MUTE = 8;

        private Boolean IsVolume(HaState state)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(),out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && this.IsFeatureSupported(entityValue, SUPPORT_VOLUME_SET);
        }
 

        private Boolean IsFeatureSupported(Int32 entityState, Int32 feature) => (entityState & feature) == feature;

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.FriendlyName}";
        }

        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.FriendlyName}";
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            Int32.TryParse(entityState?.Attributes["volume_level"]?.ToString(), out var entityValue);

            return entityValue.ToString();
        }

        protected override void ApplyAdjustment(String entity_id, Int32 value)
        {
            if (entity_id.IsNullOrEmpty())
            { return; }

            var entityState = this.plugin.States[entity_id];
            Double.TryParse(entityState.Attributes["volume_level"]?.ToString(), out var entityValue);

            const Double stepSize = 0.01;

            var volume_level_raw = entityValue;
            volume_level_raw += value * stepSize;

            volume_level_raw = Math.Max(0, Math.Min(1, volume_level_raw));

            var volume_level = volume_level_raw.ToString("0.00");

            PluginLog.Verbose($"{entity_id} - {value} - {entityValue} => {volume_level}");

            this.plugin.States[entity_id].Attributes["volume_level"] = volume_level.ToString();

            var data = new JObject {
                { "domain", "media_player" },
                { "service", "volume_set" },
                { "service_data", new JObject { { "volume_level", volume_level } } },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.plugin.CallService(data);
            this.AdjustmentValueChanged();
        }

        private Boolean is_volume_muted = false;
        
        protected override void RunCommand(String entity_id)
        {
            JObject data;
            String plex_volume;

            // Home Assistant can't get the volume state from Plex, so muting is done by Home Assistant by setting volume to 0
            // to unmute the default is to restore to the previous volume level, this can get out of sync if the volume is changed manually
            // I'm setting it to half volume on unmute instead, it's not ideal but it's better than blasting the sound out at full volume.
            if (entity_id.StartsWith("media_player.plex_plexamp"))
            {
                plex_volume = this.is_volume_muted ? "0.5" : "0";
                data = new JObject {
                            { "domain", "media_player" },
                            { "service", "volume_set" },
                            { "service_data", new JObject { { "volume_level", plex_volume } } },
                            { "target", new JObject { { "entity_id", entity_id } } }
                };
                this.is_volume_muted = !Boolean.Parse(this.is_volume_muted.ToString());
            }
            else
            {
                this.is_volume_muted = !Boolean.Parse(this.plugin.States[entity_id].Attributes["is_volume_muted"].ToString());

                data = new JObject {
                        { "domain", "media_player" },
                        { "service", "volume_mute" },
                        { "service_data", new JObject { { "is_volume_muted", this.is_volume_muted } } },
                        { "target", new JObject { { "entity_id", entity_id } } }
                };
            }
            this.plugin.CallService(data);
            this.AdjustmentValueChanged();
        }
    }
}
