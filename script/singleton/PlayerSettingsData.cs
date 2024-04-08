using System;

using Godot;
using Godot.Collections;

namespace INTOnlineCoop.Script.Singleton
{
    /// <summary>
    /// Available display modes
    /// </summary>
    public enum DisplayMode
    {
        ///<summary>Exclusive fullscreen mode</summary>
        Fullscreen,
        ///<summary>Windowed mode</summary>
        Window
    }

    /// <summary>
    /// Singleton node for saving and accessing the settings of the player
    /// </summary>
    public partial class PlayerSettingsData : Node
    {
        private const string SettingsPath = "user://settings.dat";
        private string _displayMode = "Fullscreen";

        /// <summary>
        /// Returns the currently selected display mode
        /// </summary>
        /// <returns>Selected DisplayMode</returns>
        public DisplayMode SelectedDisplayMode => Enum.Parse<DisplayMode>(_displayMode);

        /// <summary>
        /// Returns true if particles are enabled
        /// </summary>
        /// <returns>true if particles are enabled</returns>
        public bool AreParticlesEnabled { get; private set; } = true;

        /// <summary>
        /// Returns the status of unsaved changes
        /// </summary>
        /// <value>true if a save to file is pending</value>
        public bool HasUnsavedChanges { get; private set; }

        /// <summary>
        /// Initializes the settings file
        /// </summary>
        public override void _Ready()
        {
            if (!FileAccess.FileExists(SettingsPath))
            {
                Save();
            }
            else
            {
                Load();
            }
            UpdateWindow();
        }

        /// <summary>
        /// Sets the display mode of the player
        /// </summary>
        /// <param name="displayMode">The new display mode of the player</param>
        public void SetDisplayMode(DisplayMode displayMode)
        {
            _displayMode = displayMode.ToString();
            HasUnsavedChanges = true;
        }

        /// <summary>
        /// Sets the particle activation status
        /// </summary>
        /// <param name="enabled">True if particles should be enabled</param>
        public void SetParticlesEnabled(bool enabled)
        {
            AreParticlesEnabled = enabled;
            HasUnsavedChanges = true;
        }

        /// <summary>
        /// Saves and applies the pending setting changes
        /// </summary>
        public void ApplyChanges()
        {
            Save();
            HasUnsavedChanges = false;
            UpdateWindow();
        }

        /// <summary>
        /// Resets the pending setting changes
        /// </summary>
        public void DiscardChanges()
        {
            Load();
            HasUnsavedChanges = false;
        }

        private void UpdateWindow()
        {
            DisplayServer.WindowMode newMode = SelectedDisplayMode switch
            {
                DisplayMode.Window => DisplayServer.WindowMode.Windowed,
                DisplayMode.Fullscreen => DisplayServer.WindowMode.ExclusiveFullscreen,
                _ => DisplayServer.WindowMode.ExclusiveFullscreen
            };
            DisplayServer.WindowSetMode(newMode);
        }

        private void Load()
        {
            if (!FileAccess.FileExists(SettingsPath))
            {
                GD.PrintErr("Tried to load a non-existing savefile!");
                return;
            }

            using FileAccess saveFile = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Read);
            if (saveFile == null)
            {
                Error fileError = FileAccess.GetOpenError();
                GD.PrintErr($"File Error: {fileError}");
                return;
            }

            string fileData = saveFile.GetAsText();
            Json json = new();
            Error parseResult = json.Parse(fileData);
            if (parseResult != Error.Ok)
            {
                GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {fileData} at line {json.GetErrorLine()}");
            }

            Dictionary<string, Variant> dataDict = new((Dictionary)json.Data);

            //Write variables
            _displayMode = (string)dataDict["DisplayMode"];
            AreParticlesEnabled = (bool)dataDict["ParticlesEnabled"];
        }

        private void Save()
        {
            Dictionary<string, Variant> dataDict = new() {
                {"DisplayMode", _displayMode},
                {"ParticlesEnabled", AreParticlesEnabled}
            };
            string jsonData = Json.Stringify(dataDict);

            using FileAccess saveFile = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Write);
            if (saveFile == null)
            {
                Error fileError = FileAccess.GetOpenError();
                GD.PrintErr($"File Error: {fileError}");
                return;
            }
            saveFile.StoreLine(jsonData);
        }
    }
}