namespace Loupedeck.HomeAssistant.Commands
{
    public class MediaPlayerPreviousTrackCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerPreviousTrackCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.PREVIOUS_TRACK;
            this.service = "media_previous_track";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayerPreviousTrack";
            this.DisplayName = "Previous Track";
            this.Description = "Play the previous track";
        }
    }
}
