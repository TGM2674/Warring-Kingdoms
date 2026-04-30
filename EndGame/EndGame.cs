using Godot;

public partial class EndGame : Control
{
    [Export] private Label resultLabel = null;
    [Export] private Button playAgainButton = null;
    [Export] private Button exitButton = null;
    [Export] private string resultText = "Victory!";

    public override void _Ready()
    {
        resultLabel.Text = resultText;
        playAgainButton.Pressed += OnPlayAgainPressed;
        exitButton.Pressed += OnExitPressed;
    }

    private void OnPlayAgainPressed()
    {
        GetTree().ChangeSceneToFile("res://main.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}