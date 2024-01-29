namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;
    using Loupedeck.HomeAssistant.Json;
    using Newtonsoft.Json.Linq;

    public class BaseExtensionsMediaPlayer
    {
        public Int32 feature;
        public String service;
        public Boolean isServiceDataRequired;
        public JObject service_data;
        public HaPlugin plugin;
        public ActionEditorAction actionEditorAction;

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

        public BaseExtensionsMediaPlayer()
        {
        }

        public void SetMediaPlayerGroupName()
        {
            this.actionEditorAction.GroupName = "Media Player";
        }

        public Boolean MediaPlayerOnLoad(HaPlugin plugin)
        {
            this.plugin = plugin;
            plugin.StatesReady += this.StatesReady;
            return true;
        }

        public void StatesReady(Object sender, EventArgs e)
        {
            PluginLog.Verbose($".OnLoad() => StatesReady");

            var mediaPlayerParameter = new PluginActionParameter("test", "ting", "player");

        }

        public void AddMediaPlayerControlsToActionEditor(ActionEditorAction actionEditorAction)
        {
            this.actionEditorAction = actionEditorAction;
            actionEditorAction.ActionEditor.AddControlEx(
                new ActionEditorTextbox("label", "Button Label")
                .SetPlaceholder("Label on icon image")
            );
            actionEditorAction.ActionEditor.AddControlEx(
               new ActionEditorListbox(name: "media_player", labelText: "Media Player", description: "Select the media player").SetRequired()
            );
            actionEditorAction.ActionEditor.ListboxItemsRequested += this.HandleMediaPlayerListboxItemsRequested;
            actionEditorAction.ActionEditor.ControlValueChanged += this.HandleMediaPlayerControlValueChanged;
        }
        public void HandleMediaPlayerListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (this.IsMediaPlayerCommandSupported(state, this.feature))
                    {
                        e.AddItem(state.Entity_Id, state.FriendlyName, null);
                    }
                }
            }
            else
            {
                this.plugin.Log.Error($"Unexpected control name '{e.ControlName}'");
            }
        }

        public void HandleMediaPlayerControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                this.actionEditorAction.ActionEditor.ListboxItemsChanged("media_player");
            }
        }

        public Boolean IsMediaPlayerCommandSupported(HaState state, Int32 feature)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(), out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && this.IsMediaPlayerFeatureSupported(entityValue, feature);
        }

        public Boolean IsMediaPlayerFeatureSupported(Int32 entityValue, Int32 feature) => (entityValue & feature) == feature;

        public Boolean CallServiceWithData(ActionEditorActionParameters actionParameter, Boolean isServiceDataRequired, String service, JObject serviceData, HaPlugin plugin)
        {
            if (actionParameter == null)
            {
                return false;
            }

            var mediaPlayer = actionParameter.GetString("media_player");

            var data = isServiceDataRequired
                ? new JObject
                {
                { "domain", "media_player" },
                { "service", service },
                { "service_data", serviceData },
                { "target", new JObject { { "entity_id", mediaPlayer } } }
                }
                : new JObject
                {
                { "domain", "media_player" },
                { "service", service },
                { "target", new JObject { { "entity_id", mediaPlayer } } }
                };

            plugin.CallService(data);
            return true;
        }
    }
}
