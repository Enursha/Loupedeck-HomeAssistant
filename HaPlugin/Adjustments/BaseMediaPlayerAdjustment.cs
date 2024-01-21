namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;

    using Newtonsoft.Json.Linq;

    public abstract class BaseMediaPlayerAdjustment : ActionEditorAdjustment
    {
        protected Int32 feature;
        protected String service;
        protected Boolean isServiceDataRequired;
        protected JObject service_data;
        protected HaPlugin plugin;

        protected BaseMediaPlayerAdjustment() : base(false)
        {
            this.plugin = base.Plugin as HaPlugin;

            this.SetMediaPlayerGroupName();
            this.AddMediaPlayerControlsToActionEditor(this.plugin, this.feature);
        }
        //protected override Boolean OnLoad() => this.plugin.MediaPlayerOnLoad();
        protected override Boolean OnLoad() => BaseExtensionsMediaPlayer.MediaPlayerOnLoad(this);
        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters) => BaseExtensionsMediaPlayer.RunMediaPlayerCommand(actionParameters, this.isServiceDataRequired, this.service, this.service_data, this.plugin);
    }
}
