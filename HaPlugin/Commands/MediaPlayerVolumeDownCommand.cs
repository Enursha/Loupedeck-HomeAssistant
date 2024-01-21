namespace Loupedeck.HomeAssistant.Commands
{
    public class MediaPlayerVolumeDownCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerVolumeDownCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.VOLUME_STEP;
            this.service = "volume_down";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayerVolumeDown";
            this.DisplayName = "Volume Down";
            this.Description = "Decrease the volume of the media player";
        }
    }
}
