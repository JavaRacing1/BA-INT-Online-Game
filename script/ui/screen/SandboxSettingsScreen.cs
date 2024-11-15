using Godot;

using INTOnlineCoop.Script.Level;
using INTOnlineCoop.Script.UI.Component;

namespace INTOnlineCoop.Script.UI.Screen
{
    /// <summary>
    /// Screen for configuring the sandbox mode
    /// </summary>
    public partial class SandboxSettingsScreen : Control
    {
        [Export] private GeneratorSettingsContainer _generatorSettings;

        private void OnBackButtonPressed()
        {
            _ = GetTree().ChangeSceneToFile("res://scene/ui/screen/MainMenu.tscn");
        }

        private void OnPlayButtonPressed()
        {
            if (_generatorSettings != null)
            {
                LevelGenerator levelGenerator = new();
                levelGenerator.SetTerrainShape(_generatorSettings.SelectedTerrainShape);
                levelGenerator.EnableDebugMode();
                Image image = levelGenerator.Generate(_generatorSettings.Seed);

                GameLevel level = GD.Load<PackedScene>("res://scene/level/GameLevel.tscn").Instantiate<GameLevel>();
                level.Init(image);
                GetTree().Root.AddChild(level);
                GetTree().CurrentScene = level;
                level.SpawnSandboxCharacter(_generatorSettings.SelectedTerrainShape);
                QueueFree();
            }
        }
    }
}
