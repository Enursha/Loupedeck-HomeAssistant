namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    public class MediaPlayerStopCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerStopCommand() : base(
            feature: BaseExtensionsMediaPlayer.VOLUME_MUTE,
            service: "media_stop",
            isServiceDataRequired: false,
            name: "MediaPlayerStop",
            displayName: "Stop",
            description: "Stop a media player",
            groupName: "Media Player"
            )
        {
        }
    }
}
