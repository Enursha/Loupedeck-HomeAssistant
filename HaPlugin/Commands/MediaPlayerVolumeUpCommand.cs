namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    public class MediaPlayerVolumeUpCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerVolumeUpCommand() : base(
            feature: BaseExtensionsMediaPlayer.VOLUME_STEP,
            service: "volume_up",
            isServiceDataRequired: false,
            name: "MediaPlayer",
            displayName: "Volume Up",
            description: "Increase the volume of a media player",
            groupName: "Media Player"
            )
        {
        }
    }
}
