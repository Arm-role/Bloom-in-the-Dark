using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FloatingTextBootstrap : MonoBehaviour
{
  [SerializeField] private GameObject prefab;

  [SerializeField]
  private List<FloatingTextStyleConfig> styles;

  private FloatingTextService _service;
  public FloatingTextService Service => _service;

  private void Start()
  {
    var pool = new AdressablePoolingService("[Damage_Root]");

    var s = new List<IFloatingTextStyleConfig>(styles);

    _service = new FloatingTextService(
        pool,
        prefab,
        s
    );
  }
  public async Task SpawnDamageText(Vector3 hitbox, int amount)
  {
    await _service.Spawn(
              FloatingTextType.Damage,
              hitbox,
              amount
          );
  }
  public async Task SpawnHealText(Vector3 hitbox, int amount)
  {
    await _service.Spawn(
              FloatingTextType.Heal,
              hitbox,
              amount
          );
  }
  public async Task SpawnEnergyText(Vector3 hitbox, int amount)
  {
    await _service.Spawn(
              FloatingTextType.Energy,
              hitbox,
              amount
          );
  }
}
