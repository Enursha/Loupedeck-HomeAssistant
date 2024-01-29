namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using Loupedeck;
    public class MediaPlayerNextTrackCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerNextTrackCommand() : base(
            feature: BaseExtensionsMediaPlayer.NEXT_TRACK,
            service: "media_next_track",
            isServiceDataRequired: false,
            name: "MediaPlayerNextTrackCommand",
            displayName: "Next Track",
            description: "Play the next track",
            groupName: "Media Player"
            )
        {
        }
    }
}
