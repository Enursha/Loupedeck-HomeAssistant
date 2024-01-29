namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;

    using Newtonsoft.Json.Linq;

    public class MediaPlayerSeekAdjustment : BaseMediaPlayerAdjustment
    {
        private TimeSpan timeSinceLastUpdate;

        public MediaPlayerSeekAdjustment() : base(
           feature: BaseExtensionsMediaPlayer.SEEK,
           service: "media_seek",
           isServiceDataRequired: true,
           name: "MediaPlayerSeekAdjustmentCopy",
           displayName: "Seek",
           description: "Seek in a media player",
           groupName: "MediaPlayerGroup"
           )
        {
        }

        protected override JObject CreateServiceData(Int32 value)
        {
            this.plugin = base.Plugin as HaPlugin;

            var _mediaplayer = this.currentActionParameter.GetString("media_player");

            var entityState = this.plugin.States[_mediaplayer];
            Int32.TryParse(entityState.Attributes["media_duration"]?.ToString(), out var media_duration);
            Int32.TryParse(entityState.Attributes["media_position"]?.ToString(), out var media_posistion);
            DateTime.TryParse(entityState.Attributes["media_position_updated_at"]?.ToString(), out var media_posistion_update_at);
            if (media_posistion_update_at == DateTime.MinValue)
            {
                DateTime.TryParseExact(entityState.Attributes["media_position_updated_at"]?.ToString(), "yyyy-MM-ddTHH:mm:ss.ffffffK", null, System.Globalization.DateTimeStyles.None, out var media_posistion_update_at2);
            this.timeSinceLastUpdate = DateTime.UtcNow - media_posistion_update_at2;
            }
            else
            {
                this.timeSinceLastUpdate = DateTime.UtcNow - media_posistion_update_at;
            }

            var timeSinceLastUpdateInSeconds = (Int32)this.timeSinceLastUpdate.TotalSeconds;
            var currentPosition = media_posistion + timeSinceLastUpdateInSeconds;

            var newPosition = Math.Max(0, Math.Min(currentPosition + value, media_duration));

            return new JObject { { "seek_position", newPosition } };
        }

        protected override String CreateAdjustmentDisplayName()
        {
            this.plugin = base.Plugin as HaPlugin;
            var _mediaplayer = this.currentActionParameter.GetString("media_player");

            var entityState = this.plugin.States[_mediaplayer];
            Double.TryParse(entityState.Attributes["media_position"]?.ToString(), out var entityValue);
            return $"{this.currentActionParameter.GetString("label")}\n{Convert.ToInt32(entityValue)}";
        }
    }
}
