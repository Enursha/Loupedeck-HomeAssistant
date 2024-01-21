namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;

    public class MediaPlayerVolumeAdjustment : ActionEditorAdjustment
    {
        private HaPlugin plugin;

        public MediaPlayerVolumeAdjustment() : base(false)
        {
            this.Name = "MediaPlayerVolumeAdjustment";
            this.DisplayName = "Volume Set";
            this.Description = "Set the volume of a media player";
            this.GroupName = "MediaPlayerGroup";

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox("label", "Button Label")
                .SetPlaceholder("Label on icon image")
            );
            this.ActionEditor.AddControlEx(
                new ActionEditorListbox(name: "media_player", labelText: "Media Player", description: "Select the media player").SetRequired()
            );
            this.ActionEditor.ListboxItemsRequested += this.OnActionEditorListboxItemsRequested;
            this.ActionEditor.ControlValueChanged += this.OnActionEditorControlValueChanged;
        }
        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"{this.GroupName} {this.Name} Adjustment.OnLoad() => StatesReady");
            };
            return true;
        }

        private void OnActionEditorControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                this.ActionEditor.ListboxItemsChanged("media_player");
            }
        }

        private void OnActionEditorListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            this.plugin = base.Plugin as HaPlugin;

            if (e.ControlName.EqualsNoCase("media_player"))
            {
                    foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                    {
                        var state = group.Value;
                        if (this.IsVolume(state))
                        {
                            e.AddItem(state.Entity_Id, state.FriendlyName, null);
                        }
                    }
            }
            else
            {
                this.Plugin.Log.Error($"Unexpected control name '{e.ControlName}'");
            }
        }

        public const Int32 SUPPORT_VOLUME_SET = 4;

        private Boolean IsVolume(HaState state)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(),out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && this.IsFeatureSupported(entityValue, SUPPORT_VOLUME_SET);
        }
 

        private Boolean IsFeatureSupported(Int32 entityState, Int32 feature) => (entityState & feature) == feature;


        protected override Boolean ApplyAdjustment(ActionEditorActionParameters actionParameter, Int32 value)
        {
            this.plugin = base.Plugin as HaPlugin;

            if (actionParameter == null)
            { return false; }

            var _mediaplayer = actionParameter.GetString("media_player");

            var entityState = this.plugin.States[_mediaplayer];
            Double.TryParse(entityState.Attributes["volume_level"]?.ToString(), out var entityValue);

            const Double stepSize = 0.01;

            var volume_level_raw = entityValue;
            volume_level_raw += value * stepSize;

            volume_level_raw = Math.Max(0, Math.Min(1, volume_level_raw));

            var volume_level = volume_level_raw.ToString("0.00");

            PluginLog.Verbose($"{_mediaplayer} - {value} - {entityValue} => {volume_level}");

            this.plugin.States[_mediaplayer].Attributes["volume_level"] = volume_level.ToString();

            var data = new JObject {
                { "domain", "media_player" },
                { "service", "volume_set" },
                { "service_data", new JObject { { "volume_level", volume_level } } },
                { "target", new JObject { { "entity_id", _mediaplayer } } }
            };

            this.plugin.CallService(data);
            this.AdjustmentValueChanged();
            return true;
        }

        protected override String GetAdjustmentDisplayName(ActionEditorActionParameters actionParameter)
        {
            this.plugin = base.Plugin as HaPlugin;
            var _mediaplayer = actionParameter.GetString("media_player");

            var entityState = this.plugin.States[_mediaplayer];
            Double.TryParse(entityState.Attributes["volume_level"]?.ToString(), out var entityValue);
            return $"{actionParameter.GetString("label")}\n{Convert.ToInt32(entityValue * 100)}";
        }
    }
}
