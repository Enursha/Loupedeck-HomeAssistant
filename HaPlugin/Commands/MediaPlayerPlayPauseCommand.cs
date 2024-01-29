namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    using Loupedeck;

    public class MediaPlayerPlayPauseCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerPlayPauseCommand() : base(
            feature: BaseExtensionsMediaPlayer.PAUSE,
            service: "media_play_pause",
            isServiceDataRequired: false,
            name: "MediaPlayerPlayPause",
            displayName: "Play/Pause",
            description: "Play or pause a media player",
            groupName: "Media Player"
            )
        {
        }
    }
}
