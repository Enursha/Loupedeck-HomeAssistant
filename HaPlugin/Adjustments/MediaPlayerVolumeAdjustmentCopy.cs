namespace Loupedeck.HomeAssistant.Adjustments
{

    public class MediaPlayerVolumeAdjustmentCopy : BaseMediaPlayerAdjustment
    {
        public MediaPlayerVolumeAdjustmentCopy() : base()        {
            this.plugin = base.Plugin as HaPlugin;
            this.feature = BaseExtensionsMediaPlayer.VOLUME_SET;
            this.service = "volume_set";
            this.isServiceDataRequired = false;
            this.Name = "MediaPlayerVolumeAdjustmentCopy";
            this.DisplayName = "Volume Set Copy";
            this.Description = "Set the volume of a media player";


            /*
                        var _mediaplayer = actionParameter.GetString("media_player");

                        var entityState = this.plugin.States[_mediaplayer];
                        Double.TryParse(entityState.Attributes["volume_level"]?.ToString(), out var entityValue);

                        const Double stepSize = 0.01;

                        var volume_level_raw = entityValue;
                        volume_level_raw += value * stepSize;

                        volume_level_raw = Math.Max(0, Math.Min(1, volume_level_raw));

                        var volume_level = volume_level_raw.ToString("0.00");

                        PluginLog.Verbose($"{_mediaplayer} - {value} - {entityValue} => {volume_level}");

                        this.plugin.States[_mediaplayer].Attributes["volume_level"] = volume_level.ToString();

                        var data = new JObject { { "volume_level", volume_level } };
            */
        }
    }
}
