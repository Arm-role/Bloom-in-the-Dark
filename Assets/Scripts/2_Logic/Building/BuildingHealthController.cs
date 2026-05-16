public sealed class BuildingHealthController
{
    public bool IsAlive => _health.IsAlive;
    public BuildingHealth Health => _health;

    private readonly BuildingHealth _health;

    public BuildingHealthController(float maxHP) => _health = new BuildingHealth(maxHP);
    public BuildingHealthController(BuildingHealth health) => _health = health;

    public bool TakeDamage(float amount)
    {
        if (!_health.IsAlive) return true;
        _health.TakeDamage(amount);
        return !_health.IsAlive;
    }

    public void Heal(float amount) => _health.Heal(amount);
    public void Fill() => _health.Fill();
}
