namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;

    public class MediaPlayerHelper
    {
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

        public static void HandleControlValueChanged(ActionEditor actionEditor, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                actionEditor.ListboxItemsChanged("media_player");
            }
        }
        public static void HandleListboxItemsRequested(HaPlugin plugin, ActionEditor actionEditor, ActionEditorListboxItemsRequestedEventArgs e, Int32 feature)
        {
            if (e.ControlName.EqualsNoCase("media_player"))
            {
                foreach (KeyValuePair<String, Json.HaState> group in plugin.States)
                {
                    var state = group.Value;
                    if (MediaPlayerHelper.IsCommandSupported(state, feature))
                    {
                        e.AddItem(state.Entity_Id, state.FriendlyName, null);
                    }
                }
            }
            else
            {
                plugin.Log.Error($"Unexpected control name '{e.ControlName}'");
            }
        }

        public static Boolean IsCommandSupported(HaState state, Int32 feature)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(), out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && IsFeatureSupported(entityValue, feature);
        }

        public static Boolean IsFeatureSupported(Int32 entityValue, Int32 feature) => (entityValue & feature) == feature;

        public static Boolean RunCommand(ActionEditorActionParameters actionParameter, Boolean isServiceDataRequired, String service, JObject serviceData, HaPlugin plugin)
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
