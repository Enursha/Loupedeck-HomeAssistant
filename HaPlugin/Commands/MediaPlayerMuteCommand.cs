namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public class MediaPlayerMuteCommand : BaseMediaPlayerCommand
    {
        protected Boolean is_volume_muted;
        public MediaPlayerMuteCommand() : base(
            feature: BaseExtensionsMediaPlayer.VOLUME_MUTE,
            service: "volume_mute",
            isServiceDataRequired: true,
            name: "MediaPlayerMute",
            displayName: "Mute",
            description: "Mute a media player",
            groupName: "Media Player"
            )
        {
        }
        protected override JObject CreateServiceData()
        {
            this.plugin = base.Plugin as HaPlugin;
            var _mediaplayer = this.currentActionParameter.GetString("media_player");
            var entityState = this.plugin.States[_mediaplayer];
            Boolean.TryParse(entityState.Attributes["is_volume_muted"]?.ToString(), out var entityValue);

            if (entityValue != this.is_volume_muted)
            {
                this.is_volume_muted = !this.is_volume_muted;
            }
            else
            {
                this.is_volume_muted = !entityValue;
            }

            return new JObject { { "is_volume_muted", this.is_volume_muted } };
        }
    }
}
