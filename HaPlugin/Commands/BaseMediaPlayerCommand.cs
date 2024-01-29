namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public abstract class BaseMediaPlayerCommand : ActionEditorCommand
    {
        protected Int32 feature;
        protected String service;
        protected Boolean isServiceDataRequired;
        protected JObject service_data;
        protected HaPlugin plugin;
        protected BaseExtensionsMediaPlayer Extensions;
        protected ActionEditorActionParameters currentActionParameter;

        public BaseMediaPlayerCommand(Int32 feature, String service, Boolean isServiceDataRequired, String name, String displayName, String description, String groupName) : base()
        {
            this.feature = feature;
            this.service = service;
            this.isServiceDataRequired = isServiceDataRequired;
            this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
            this.GroupName = groupName;

            this.Extensions = new BaseExtensionsMediaPlayer();
            this.Extensions.AddMediaPlayerControlsToActionEditor(this);
        }
        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;
            return this.Extensions.MediaPlayerOnLoad(this.plugin);
        }
        protected override Boolean RunCommand(ActionEditorActionParameters actionParameter)
        {
            this.currentActionParameter = actionParameter;
            this.service_data = this.CreateServiceData();
            this.Extensions.CallServiceWithData(actionParameter, this.isServiceDataRequired, this.service, this.service_data, this.plugin);
            return true;
        }

        protected virtual JObject CreateServiceData() => null;
    }
}
