using Godot;

public partial class Weather : Node2D
{
    [Export] private NodePath rainPath = null;
    [Export] private NodePath snowPath = null;

    private GpuParticles2D rainParticles = null;
    private GpuParticles2D snowParticles = null;

    private float fadeSpeed = 2.0f;
    private Modifiers.Type currentWeather = Modifiers.Type.NoWeather;

    public override void _Ready()
    {
        rainParticles = GetNode<GpuParticles2D>(rainPath);
        snowParticles = GetNode<GpuParticles2D>(snowPath);

        rainParticles.Emitting = false;
        snowParticles.Emitting = false;
        rainParticles.Modulate = new Color(1, 1, 1, 0);
        snowParticles.Modulate = new Color(1, 1, 1, 0);
    }

    public override void _Process(double delta)
    {
        UpdateFade(rainParticles, currentWeather == Modifiers.Type.Raining, delta);
        UpdateFade(snowParticles, currentWeather == Modifiers.Type.Snowing, delta);
    }

    private void UpdateFade(GpuParticles2D particles, bool shouldBeVisible, double delta)
    {
        float alpha = particles.Modulate.A;

        if (shouldBeVisible)
        {
            if (alpha < 1f)
            {
                alpha = Mathf.Min(alpha + fadeSpeed * (float)delta, 1f);
                particles.Modulate = new Color(1, 1, 1, alpha);
                if (!particles.Emitting) particles.Emitting = true;
            }
        }
        else
        {
            if (alpha > 0f)
            {
                alpha = Mathf.Max(alpha - fadeSpeed * (float)delta, 0f);
                particles.Modulate = new Color(1, 1, 1, alpha);
                if (alpha == 0f) particles.Emitting = false;
            }
        }
    }

    public void UpdateWeather()
    {
        currentWeather = Modifiers.GetWeather();
    }
}