namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Loupedeck.HomeAssistant.Json;
    using Newtonsoft.Json.Linq;

    public static class BaseExtensionsMediaPlayer
    {
        public static Int32 feature;
        public static String service;
        public static Boolean isServiceDataRequired;
        public static JObject service_data;
        public static HaPlugin plugin;

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


        public static void SetMediaPlayerGroupName<T>(this PluginActionBase<T> actionBase) where T : PluginActionBase<T> => actionBase.GroupName = "Media Player";

        //public static Boolean MediaPlayerOnLoad()
        public static Boolean MediaPlayerOnLoad(ActionEditorAction action)
        {
            plugin = action.Plugin as HaPlugin;
            plugin.StatesReady += StatesReady;
            return true;
        }

        public static void StatesReady(Object sender, EventArgs e)
        {
            PluginLog.Verbose($".OnLoad() => StatesReady");

           //var mediaPlayerParameter = new PluginActionParameter("test", "ting", "player");

        }

        public static void AddMediaPlayerControlsToActionEditor(
            this ActionEditorAction actionEditorAction,
            HaPlugin plugin,
            Int32 feature
            )
        {
            actionEditorAction.ActionEditor.AddControlEx(
                new ActionEditorTextbox("label", "Button Label")
                .SetPlaceholder("Label on icon image")
            );
            actionEditorAction.ActionEditor.AddControlEx(
               new ActionEditorListbox(name: "media_player", labelText: "Media Player", description: "Select the media player").SetRequired()
            );
            var controlHandler = new MediaPlayerControlHandler(actionEditorAction, plugin, feature);
            actionEditorAction.ActionEditor.ListboxItemsRequested += controlHandler.HandleMediaPlayerListboxItemsRequested;
            actionEditorAction.ActionEditor.ControlValueChanged += controlHandler.HandleMediaPlayerControlValueChanged;
        }

        public static Boolean IsMediaPlayerCommandSupported(this HaState state, Int32 feature)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(), out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && IsMediaPlayerFeatureSupported(entityValue, feature);
        }

        public static Boolean IsMediaPlayerFeatureSupported(Int32 entityValue, Int32 feature) => (entityValue & feature) == feature;

        public static Boolean RunMediaPlayerCommand( ActionEditorActionParameters actionParameter, Boolean isServiceDataRequired, String service, JObject serviceData, HaPlugin plugin)
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
    public class MediaPlayerControlHandler
    {
        private readonly ActionEditorAction _actionEditorAction;
        private readonly HaPlugin _plugin;
        private readonly Int32 _feature;

        public MediaPlayerControlHandler(ActionEditorAction actionEditorAction, HaPlugin plugin, Int32 feature)
        {
            this._plugin = plugin;
            this._feature = feature;
            this._actionEditorAction = actionEditorAction;
        }

        public void HandleMediaPlayerListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {

            if (e.ControlName.EqualsNoCase("media_player"))
            {
                foreach (KeyValuePair<String, Json.HaState> group in this._plugin.States)
                {
                    var state = group.Value;
                    if (BaseExtensionsMediaPlayer.IsMediaPlayerCommandSupported(state, this._feature))
                    {
                        e.AddItem(state.Entity_Id, state.FriendlyName, null);
                    }
                }
            }
            else
            {
                this._plugin.Log.Error($"Unexpected control name '{e.ControlName}'");
            }
        }

        public void HandleMediaPlayerControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                this._actionEditorAction.ActionEditor.ListboxItemsChanged("media_player");
            }
        }
    }
}
