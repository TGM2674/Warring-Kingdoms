using Godot;

public partial class DayNight : Node
{
    [Export] private Sprite2D sun = null;
    [Export] private Sprite2D moon = null;
    [Export] private ColorRect sky = null;
    [Export] private Marker2D planetOn = null;
    [Export] private Marker2D planetOff = null;

    private float ellipseX = 0f;
    private float ellipseY = 0f;
    private Vector2 center = Vector2.Zero;

    private float sunAngle = 0f;
    private float moonAngle = 0f;

    private float transitionDuration = 1f;
    private float transitionTimer = 0f;
    private bool isTransitioning = false;

    private Color dayColor = new Color(0.53f, 0.81f, 0.98f);
    private Color nightColor = new Color(0.05f, 0.05f, 0.2f);
    private Color skyStartColor;
    private Color skyTargetColor;

    public override void _Ready()
    {
        center = Vector2.Zero;
        ellipseX = Mathf.Abs(planetOn.Position.X);
        ellipseY = Mathf.Abs(planetOn.Position.Y);

        sunAngle = Mathf.Atan2(planetOn.Position.Y, planetOn.Position.X);
        moonAngle = Mathf.Atan2(planetOff.Position.Y, planetOff.Position.X);

        sun.Position = GetEllipsePoint(sunAngle);
        moon.Position = GetEllipsePoint(moonAngle);

        sky.Color = dayColor;
    }

    public override void _Process(double delta)
    {
        if (!isTransitioning)
            return;

        transitionTimer += (float)delta;
        float t = Mathf.Clamp(transitionTimer / transitionDuration, 0f, 1f);
        float smooth = t * t * (3f - 2f * t);

        float angleAdvance = Mathf.Pi * smooth;
        sun.Position = GetEllipsePoint(sunAngle + angleAdvance);
        moon.Position = GetEllipsePoint(moonAngle + angleAdvance);

        sky.Color = skyStartColor.Lerp(skyTargetColor, smooth);

        if (t >= 1f)
        {
            sunAngle += Mathf.Pi;
            moonAngle += Mathf.Pi;

            sun.Position = GetEllipsePoint(sunAngle);
            moon.Position = GetEllipsePoint(moonAngle);

            isTransitioning = false;
            transitionTimer = 0f;
        }
    }

    public void Transition()
    {
        if (isTransitioning)
            return;

        bool isDay = Modifiers.IsDay();
        skyStartColor = isDay ? dayColor : nightColor;
        skyTargetColor = isDay ? nightColor : dayColor;

        isTransitioning = true;
        transitionTimer = 0f;
    }

    public bool IsTransitioning() => isTransitioning;

    private Vector2 GetEllipsePoint(float angle)
    {
        return new Vector2(
            center.X + ellipseX * Mathf.Cos(angle),
            center.Y + ellipseY * Mathf.Sin(angle)
        );
    }
}