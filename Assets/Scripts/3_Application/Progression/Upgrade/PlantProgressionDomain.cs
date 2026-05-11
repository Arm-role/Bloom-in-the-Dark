using System;
using UnityEngine;
using System.Collections.Generic;

public class PlantProgressionDomain
{
  private const float NormalExp = 1.0f;
  private const float BuffedExp = 2.0f;

  public event Action<IItemDefinition, int> OnLevelUp;

  private readonly Dictionary<GameTag, PlantExp> _progressions = new();
  private readonly IProgressionView _view;

  public PlantProgressionDomain(IProgressionView view)
  {
    _view = view;
  }

  public void OnPlantReady(IItemDefinition plant, bool buffed)
  {
    GameTag key = plant.Key;

    if (!_progressions.TryGetValue(key, out PlantExp data))
    {
      data = new PlantExp(maxExp: 3);
      _progressions[key] = data;
    }

    float amount = buffed ? BuffedExp : NormalExp;
    bool leveledUp = data.AddExp(amount);

    _view.SetProgression(data.Level, data.CurrentExp, data.MaxExp);

    if (leveledUp)
      OnLevelUp?.Invoke(plant, data.Level);
  }

  public PlantExp GetExp(IItemDefinition plant)
      => _progressions.TryGetValue(plant.Key, out PlantExp data) ? data : null;
}

public class PlantExp
{
  public float CurrentExp { get; private set; }
  public float MaxExp { get; private set; }
  public int Level { get; private set; }

  public PlantExp(float maxExp, int startLevel = 1)
  {
    Level = startLevel;
    MaxExp = maxExp;
    CurrentExp = 0f;
  }

  public bool AddExp(float amount)
  {
    CurrentExp = Mathf.Clamp(CurrentExp + amount, 0f, MaxExp);

    if (!Mathf.Approximately(CurrentExp, MaxExp)) return false;

    Level++;
    MaxExp = Mathf.Round(MaxExp * 1.5f);
    CurrentExp = 0f;
    return true;
  }
}