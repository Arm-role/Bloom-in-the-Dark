public class PlayerData
{
    public float MaxHealth;
    public float CurrentHealth;

    public float MaxEnergy;
    public float CurrentEnergy;

    public float MoveSpeed; 
    public PlayerData(
        float currentHealth,
        float maxHealt, 
        float currentEnergy, 
        float maxEnergy,
        float moveSpeed)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealt;
        
        CurrentEnergy = currentEnergy;
        MaxEnergy = maxEnergy;
        
        MoveSpeed = moveSpeed;
    }
}