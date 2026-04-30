using Godot;

public partial class Unit : Sprite2D
{
    private Texture2D attackTexture = null;
    private float attackTimer = 0f;
    private float attackDelay = 0f;
    private bool hasAttacked = false;

    public void SetAttack(Texture2D texture, float delay)
    {
        attackTexture = texture;
        attackDelay = delay;
    }

    public override void _Process(double delta)
    {
        if (attackTexture == null || hasAttacked)
            return;

        attackTimer += (float)delta;
        if (attackTimer >= attackDelay)
        {
            Texture = attackTexture;
            hasAttacked = true;
        }
    }
}