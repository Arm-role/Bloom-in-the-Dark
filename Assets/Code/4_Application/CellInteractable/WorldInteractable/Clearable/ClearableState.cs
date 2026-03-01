using UnityEngine;

public abstract class ClearableState : MonoBehaviour
{
  [SerializeField] protected float maxHP = 10f;
  protected float hp;

  protected virtual void Awake()
  {
    hp = maxHP;
  }

  public bool IsCleared => hp <= 0f;

  public virtual bool ApplyDamage(float amount)
  {
    hp -= amount;
    if (hp <= 0f)
    {
      OnCleared();
      return true;
    }
    return false;
  }

  protected virtual void OnCleared()
  {
    Destroy(gameObject);
  }

  public abstract ETargetType TargetType { get; }
  public abstract EInteractionIntentType RequiredIntent { get; }
  public abstract bool CanBeClearedBy(IItemInstance item);
}