namespace Loupedeck.HomeAssistant.Commands
{
    public class MediaPlayerNextTrackCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerNextTrackCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.NEXT_TRACK;
            this.service = "media_next_track";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayerNextTrackCommand";
            this.DisplayName = "Next Track";
            this.Description = "Play the next track";
        }
    }
}
