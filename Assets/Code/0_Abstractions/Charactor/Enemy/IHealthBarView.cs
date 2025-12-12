public interface IHealthBarView
{
    string Name { get; }
    void Setup(float maxHP);
    void TakeDamage(float dmg);
    void Heal(float amount);
}