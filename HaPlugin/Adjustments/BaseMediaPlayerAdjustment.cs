namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;

    using Newtonsoft.Json.Linq;

    public abstract class BaseMediaPlayerAdjustment: ActionEditorAdjustment
    {
        protected Int32 feature;
        protected String service;
        protected Boolean isServiceDataRequired;
        protected JObject service_data;
        protected HaPlugin plugin;
        protected BaseExtensionsMediaPlayer Extensions;
        protected ActionEditorActionParameters currentActionParameter;

        public BaseMediaPlayerAdjustment(Int32 feature, String service, Boolean isServiceDataRequired, String name, String displayName, String description, String groupName) : base(false)
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
        protected override Boolean ApplyAdjustment(ActionEditorActionParameters actionParameter, Int32 value)
        {
            this.currentActionParameter = actionParameter;
            this.service_data = this.CreateServiceData(value);
            this.Extensions.CallServiceWithData(actionParameter, this.isServiceDataRequired, this.service, this.service_data, this.plugin);
            this.AdjustmentValueChanged();
            return true;
        }

        protected virtual JObject CreateServiceData(Int32 value)
        {
            return null;
        }

        protected override String GetAdjustmentDisplayName(ActionEditorActionParameters actionParameter)
        {
            this.currentActionParameter = actionParameter;
            return this.CreateAdjustmentDisplayName();
        }

        protected virtual String CreateAdjustmentDisplayName()
        {
            return null;
        }
    }
}
