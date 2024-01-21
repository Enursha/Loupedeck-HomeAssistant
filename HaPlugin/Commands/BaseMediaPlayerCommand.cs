namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;
    public abstract class BaseMediaPlayerCommand : ActionEditorCommand
    {
        protected Int32 feature;
        protected String service;
        protected Boolean isServiceDataRequired;
        protected JObject service_data;
        protected HaPlugin plugin;

        public const Int32 PAUSE = 1;
        public const Int32 SEEK = 2;
        public const Int32 VOLUME_SET = 4;
        public const Int32 VOLUME_MUTE = 8;
        public const Int32 PREVIOUS_TRACK = 16;
        public const Int32 NEXT_TRACK = 32;
        public const Int32 TURN_ON = 128;
        public const Int32 TURN_OFF = 256;
        public const Int32 PLAY_MEDIA = 512;
        public const Int32 VOLUME_STEP = 1024;
        public const Int32 SELECT_SOURCE = 2048;
        public const Int32 STOP = 4096;
        public const Int32 CLEAR_PLAYLIST = 8192;
        public const Int32 PLAY = 16384;
        public const Int32 SHUFFLE_SET = 32768;
        public const Int32 SELECT_SOUND_MODE = 65536;
        public const Int32 BROWSE_MEDIA = 131072;
        public const Int32 REPEAT_SET = 262144;
        public const Int32 GROUPING = 524288;
        public const Int32 MEDIA_ANNOUNCE = 1048576;
        public const Int32 MEDIA_ENQUEUE = 2097152;


        protected BaseMediaPlayerCommand() : base()
        {
            this.GroupName = "Media Player";
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
                PluginLog.Verbose($"{this.Name}Command.OnLoad() => StatesReady");
            };
            return true;
        }

        protected void OnActionEditorControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                this.ActionEditor.ListboxItemsChanged("media_player");
            }
        }

        protected void OnActionEditorListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            this.plugin = base.Plugin as HaPlugin;

            if (e.ControlName.EqualsNoCase("media_player"))
            {
                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (this.IsCommandSupported(state, this.feature))
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
        protected Boolean IsCommandSupported(HaState state, Int32 feature)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(), out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && this.IsFeatureSupported(entityValue, feature);
        }

        protected Boolean IsFeatureSupported(Int32 entityValue, Int32 feature) => (entityValue & feature) == feature;

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameter)
        {
            if (actionParameter == null)
            {
                return false;
            }

            JObject data;

            var _mediaplayer = actionParameter.GetString("media_player");

            data = this.isServiceDataRequired
                ? new JObject
                {
                    { "domain", "media_player" },
                    { "service", this.service },
                    { "service_data", this.service_data },
                    { "target", new JObject { { "entity_id", _mediaplayer } } }
                }
                : new JObject
                {
                    { "domain", "media_player" },
                    { "service", this.service },
                    { "target", new JObject { { "entity_id", _mediaplayer } } }
                };

            this.plugin.CallService(data);
            return true;
        }
    }
}
