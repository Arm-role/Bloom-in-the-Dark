using System;

public class PlayerProgression
{
  public event Action<int> OnLevelChanged;
  public event Action<float, float> OnExpChanged;
  public event Action<int> OnLevelUp;

  private int _level = 1;
  private float _currentExp;
  private float _requiredExp;

  public int Level => _level;

  public PlayerProgression(float baseExp = 100f)
  {
    _requiredExp = baseExp;
  }
  public void Setup()
  {
    OnExpChanged?.Invoke(_currentExp, _requiredExp);
  }

  public void AddExp(float amount)
  {
    if (amount <= 0) return;

    _currentExp += amount;

    while (_currentExp >= _requiredExp)
    {
      _currentExp -= _requiredExp;
      LevelUp();
    }

    OnExpChanged?.Invoke(_currentExp, _requiredExp);
  }

  private void LevelUp()
  {
    _level++;

    // 🔥 scaling curve (ปรับได้)
    _requiredExp *= 1.25f;

    OnLevelChanged?.Invoke(_level);
    OnLevelUp?.Invoke(_level);
  }
}