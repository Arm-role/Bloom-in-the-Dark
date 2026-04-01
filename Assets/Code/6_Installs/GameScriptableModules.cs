using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameScriptableModules", menuName = "Game/Scriptable Modules")]
public class GameScriptableModules : ScriptableObject
{
  [Header("DRAG CONFIGURATION")]
  public float holdThreshold = 0.5f;
  public float holdMoveTolerance = 0.5f;

  public LayerMask DetectionLayer;

  [Header("INVENTORY CONFIGURATION")]
  public int HotbarSize = 9;
  public int InventorySize = 27;

  [Header("LIBRARY REFERENCES")]
  public ItemDatabase ItemDatabase;
  public StatDatabase StatDatabase;
  public RequestDatabase RequestDatabase;
  public ParticleLibrary ParticleLibrary;
  public GameObjectLibrary GameObjectLibrary;
  public TileLibrary TileLibrary;
  public TagLibraryAsset TagLibrary;
  public CharacterAnimationLibrary AnimationLibrary;
  public InteractionCostConfig InteractionCostConfig;

  [Header("CONFIG REFERENCES")]
  public PhaseStatConfig PhaseStatConfig;
  public CharacterAnimationConfig CharacterAnimationConfig;
  public List<UpgradeThresholdConfig> UpgradeThresholdConfigs;
  public FloatingTextConfig FloatingTextConfig;
}
