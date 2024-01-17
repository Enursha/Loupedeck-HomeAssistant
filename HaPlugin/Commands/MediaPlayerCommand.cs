namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using System.Collections.Generic;
    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;

    public class MediaPlayerMuteCommand : ActionEditorCommand
    {
        private HaPlugin plugin;

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"{this.GroupName}Command.OnLoad() => StatesReady");
            };
            return true;
        }
        public MediaPlayerMuteCommand() : base()
        {
            this.Name = "MediaPlayer";
            this.GroupName = "Media Player";
            this.DisplayName = "Mute";
            this.Description = "Mute a media player";

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
                    if (this.IsMute(state))
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

        public const Int32 SUPPORT_VOLUME_MUTE = 8;

        private Boolean IsMute(HaState state)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(), out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && this.IsFeatureSupported(entityValue, SUPPORT_VOLUME_MUTE);
        }

        private Boolean IsFeatureSupported(Int32 entityState, Int32 feature) => (entityState & feature) == feature;

        private Boolean is_volume_muted = false;

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameter)
        {
            if (actionParameter == null)
            { return false; }

            JObject data;
            String plex_volume;

            var _mediaplayer = actionParameter.GetString("media_player");

            // Home Assistant can't get the volume state from Plex, so muting is done by Home Assistant by setting volume to 0
            // to unmute the default is to restore to the previous volume level, this can get out of sync if the volume is changed manually
            // I'm setting it to half volume on unmute instead, it's not ideal but it's better than blasting the sound out at full volume.
            if (_mediaplayer.StartsWith("media_player.plex_plexamp"))
            {
                plex_volume = this.is_volume_muted ? "0.5" : "0";
                data = new JObject {
                            { "domain", "media_player" },
                            { "service", "volume_set" },
                            { "service_data", new JObject { { "volume_level", plex_volume } } },
                            { "target", new JObject { { "entity_id", _mediaplayer } } }
                };
                this.is_volume_muted = !Boolean.Parse(this.is_volume_muted.ToString());
            }
            else
            {
                this.is_volume_muted = !Boolean.Parse(this.plugin.States[_mediaplayer].Attributes["is_volume_muted"].ToString());

                data = new JObject {
                        { "domain", "media_player" },
                        { "service", "volume_mute" },
                        { "service_data", new JObject { { "is_volume_muted", this.is_volume_muted } } },
                        { "target", new JObject { { "entity_id", _mediaplayer} } }
                };
            }
            this.plugin.CallService(data);
            return true;
        }
    }



    public class MediaPlayerPlayPauseCommand : ActionEditorCommand
    {
        private HaPlugin plugin;

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"{this.GroupName}Command.OnLoad() => StatesReady");
            };
            return true;
        }
        public MediaPlayerPlayPauseCommand() : base()
        {
            this.Name = "MediaPlayer";
            this.GroupName = "Media Player";
            this.DisplayName = "Play/Pause";
            this.Description = "Play or pause a media player";

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
                    if (this.IsPlayPause(state))
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

        public const Int32 SUPPORT_PLAY = 1;
        public const Int32 SUPPORT_PAUSE = 2;

        private Boolean IsPlayPause(HaState state)
        {
            Int32.TryParse(state?.Attributes["supported_features"]?.ToString(), out var entityValue);
            return state.Entity_Id.StartsWith("media_player.") && this.IsFeatureSupported(entityValue, SUPPORT_PLAY);
        }
        private Boolean IsFeatureSupported(Int32 entityState, Int32 feature) => (entityState & feature) == feature;
    }
}