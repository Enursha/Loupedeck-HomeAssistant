namespace Loupedeck.HomeAssistant.Commands
{
    public class MediaPlayerStopCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerStopCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.STOP;
            this.service = "media_stop";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayerStop";
            this.DisplayName = "Stop";
            this.Description = "Stop the media player";
        }
    }
}
