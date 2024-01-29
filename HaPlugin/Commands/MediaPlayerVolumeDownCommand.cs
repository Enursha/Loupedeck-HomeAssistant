namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    public class MediaPlayerVolumeDownCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerVolumeDownCommand() : base(
            feature: BaseExtensionsMediaPlayer.VOLUME_STEP,
            service: "volume_down",
            isServiceDataRequired: false,
            name: "MediaPlayerVolumeDown",
            displayName: "Volume Down",
            description: "Decrease the volume of the media player",
            groupName: "Media Player"
            )
        {
        }
    }
}
