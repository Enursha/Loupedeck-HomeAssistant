namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    public class MediaPlayerPreviousTrackCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerPreviousTrackCommand() : base(
            feature: BaseExtensionsMediaPlayer.PREVIOUS_TRACK,
            service: "media_previous_track",
            isServiceDataRequired: false,
            name: "MediaPlayerPreviousTrack",
            displayName: "Previous Track",
            description: "Play the previous track",
            groupName: "Media Player"
            )
        {
        }
    }
}
