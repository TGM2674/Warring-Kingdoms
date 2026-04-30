using Godot;

public partial class Terrain : Node
{
    [Export] private Sprite2D terrainSprite = null;
    [Export] private Marker2D unitMarker = null;
    [Export] public int playerIndex = 1;
    [Export] private bool flipX = false;

    private PackedScene highGroundScene = null;
    private PackedScene swampScene = null;
    private PackedScene noTerrainScene = null;

    private Vector2 defaultMarkerPosition = Vector2.Zero;
    private float highGroundOffset = 115f;

    public override void _Ready()
    {
        highGroundScene = GD.Load<PackedScene>("res://Environment/Terrain/HighGround.tscn");
        swampScene = GD.Load<PackedScene>("res://Environment/Terrain/Swamp.tscn");
        noTerrainScene = GD.Load<PackedScene>("res://Environment/Terrain/NoTerrain.tscn");

        defaultMarkerPosition = unitMarker.Position;
    }

    public void UpdateTerrain()
    {
        Modifiers.Type terrain = Modifiers.GetTerrain(playerIndex);

        // Reset marker to default position
        unitMarker.Position = defaultMarkerPosition;

        // Swap sprite
        PackedScene sceneToLoad = terrain switch
        {
            Modifiers.Type.HighGround => highGroundScene,
            Modifiers.Type.Swamp => swampScene,
            _ => noTerrainScene
        };

        Sprite2D newSprite = sceneToLoad.Instantiate<Sprite2D>();
        newSprite.FlipH = flipX;
	    newSprite.Position = defaultMarkerPosition;

        if (terrainSprite != null)
            terrainSprite.QueueFree();

        AddChild(newSprite);
        terrainSprite = newSprite;

        // Adjust marker height for HighGround
        if (terrain == Modifiers.Type.HighGround)
            unitMarker.Position = new Vector2(defaultMarkerPosition.X, defaultMarkerPosition.Y - highGroundOffset);
    }
}