namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaPlayerMuteCommand : BaseMediaPlayerCommand
    {
        public MediaPlayerMuteCommand() : base()
        {
            this.feature = BaseMediaPlayerCommand.VOLUME_MUTE;
            this.service = "media_mute";
            this.isServiceDataRequired = false;

            this.Name = "MediaPlayerMute";
            this.DisplayName = "Mute";
            this.Description = "Mute a media player";
        }
    }
}
