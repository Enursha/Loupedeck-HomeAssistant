namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaPlayerPlayPauseCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerPlayPauseCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.PAUSE;
            this.service = "media_play_pause";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayerPlayPause";
            this.DisplayName = "Play/Pause";
            this.Description = "Play or pause a media player";
        }
    }
}
