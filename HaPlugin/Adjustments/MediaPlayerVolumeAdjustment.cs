namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;
    using Newtonsoft.Json.Linq;

    public class MediaPlayerVolumeAdjustment : BaseMediaPlayerAdjustment
    {
        public MediaPlayerVolumeAdjustment() : base(
            feature: BaseExtensionsMediaPlayer.VOLUME_SET,
            service: "volume_set",
            isServiceDataRequired: true,
            name: "MediaPlayerVolumeAdjustmentCopy",
            displayName: "Volume Set",
            description: "Set the volume of a media player",
            groupName: "MediaPlayerGroup"
            )
        {
        }
        protected override JObject CreateServiceData(Int32 value)
        {
            this.plugin = base.Plugin as HaPlugin;

            var _mediaplayer = this.currentActionParameter.GetString("media_player");

            var entityState = this.plugin.States[_mediaplayer];
            Double.TryParse(entityState.Attributes["volume_level"]?.ToString(), out var entityValue);

            const Double stepSize = 0.01;

            var volume_level_raw = entityValue;
            volume_level_raw += value * stepSize;

            volume_level_raw = Math.Max(0, Math.Min(1, volume_level_raw));

            var new_volume_level = volume_level_raw.ToString("0.00");

            PluginLog.Verbose($"{_mediaplayer} - {value} - {entityValue} => {new_volume_level}");

            this.plugin.States[_mediaplayer].Attributes["volume_level"] = new_volume_level.ToString();

            return new JObject { { "volume_level", new_volume_level } };
        }

        protected override String CreateAdjustmentDisplayName()
        {
            this.plugin = base.Plugin as HaPlugin;
            var _mediaplayer = this.currentActionParameter.GetString("media_player");

            var entityState = this.plugin.States[_mediaplayer];
            Double.TryParse(entityState.Attributes["volume_level"]?.ToString(), out var entityValue);
            return $"{this.currentActionParameter.GetString("label")}\n{Convert.ToInt32(entityValue * 100)}";
        }
    }
}
