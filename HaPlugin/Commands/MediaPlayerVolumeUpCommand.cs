namespace Loupedeck.HomeAssistant.Commands
{
    public class MediaPlayerVolumeUpCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerVolumeUpCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.VOLUME_STEP;
            this.service = "volume_up";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayer";
            this.DisplayName = "Volume Up";
            this.Description = "Increase the volume of a media player";
        }
    }
}
