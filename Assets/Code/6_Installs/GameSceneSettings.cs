using UnityEngine;

[CreateAssetMenu(fileName = "GameSceneSettings", menuName = "Game/Scene Settings")]
public class GameSceneSettings : ScriptableObject
{
  [Header("DRAG CONFIGURATION")]
  public float holdThreshold = 0.5f;
  public float holdMoveTolerance = 0.5f;

  [Header("PLAYER CONFIGURATION")]
  public float MoveSpeed = 5f;
  public float DetectDistance = 2f;
  public int MaxHP = 100;
  public int MaxEnergy = 100;

  public LayerMask DetectionLayer;

  [Header("INVENTORY CONFIGURATION")]
  public int HotbarSize = 9;
  public int InventorySize = 27;

  [Header("LIBRARY REFERENCES")]
  public ItemLibrary ItemsLibrary;
  public ParticleLibrary ParticleLibrary;
  public GameObjectLibrary GameObjectLibrary;
  public TileLibrary TileLibrary;
  public CharacterAnimationLibrary AnimationLibrary;
  public InteractionCostConfig InteractionCostConfig;

  [Header("PREVIEW REFERENCES")]
  public MousePreviewData MousePreviewData;

  [Header("CONFIG REFERENCES")]
  public CharacterAnimationConfig CharacterAnimationConfig;
}
