using Godot;

public partial class Card : PanelContainer
{
    [Export] private TextureRect cardSprite = null;
    [Export] private Label numberLabel = null;
    [Export] private Polygon2D buff1 = null;
    [Export] private Polygon2D buff2 = null;
    [Export] public Units.Type unitType = Units.Type.None;

    [Signal] public delegate void CardClickedEventHandler(Units.Type unit);

    private ColorRect overlay = null;

    public override void _Ready()
    {
        overlay = new ColorRect();
        overlay.Color = new Color(0, 0, 0, 0.5f);
        overlay.AnchorsPreset = (int)Control.LayoutPreset.FullRect;
        overlay.Visible = false;
        AddChild(overlay);

        Vector2[] triangleShape = new Vector2[]
        {
            new Vector2(0, 10),
            new Vector2(5, 0),
            new Vector2(10, 10)
        };

        buff1.Polygon = triangleShape;
        buff1.Color = new Color(0, 1, 0);
        buff1.Visible = false;

        buff2.Polygon = triangleShape;
        buff2.Color = new Color(0, 1, 0);
        buff2.Position = new Vector2(15, 0);
        buff2.Visible = false;

        GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.Pressed &&
            overlay.Visible == false) // cant click when greyed out
        {
            EmitSignal(SignalName.CardClicked, (int)unitType);
        }
    }

    public void SetNumber(int number) => numberLabel.Text = number.ToString();
    public void SetSprite(Texture2D texture) => cardSprite.Texture = texture;

    public void SetBuffCount(int count)
    {
        buff1.Visible = count >= 1;
        buff2.Visible = count >= 2;
    }

    public void SetGreyedOut(bool greyed) => overlay.Visible = greyed;
}