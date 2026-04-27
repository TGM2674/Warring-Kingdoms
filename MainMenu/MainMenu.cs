using Godot;

public partial class MainMenu : Node2D
{
    [Export] private Button playButton = null;
    [Export] private Button exitButton = null;

    public override void _Ready()
    {
        playButton.Pressed += OnPlayPressed;
        exitButton.Pressed += OnExitPressed;
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://main.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}