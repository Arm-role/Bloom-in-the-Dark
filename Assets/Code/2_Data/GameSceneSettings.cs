using UnityEngine;

[CreateAssetMenu(fileName = "GameSceneSettings", menuName = "Game/Scene Settings")]
public class GameSceneSettings : ScriptableObject
{
    [Header("DRAG CONFIGURATION")]
    public float holdThreshold = 0.5f;
    public float holdMoveTolerance = 0.5f;

    [Header("PLAYER CONFIGURATION")]
    public float MoveSpeed = 5f;

    [Header("INVENTORY CONFIGURATION")]
    public int HotbarSize = 9;
    public int InventorySize = 27;

    [Header("LIBRARY REFERENCES")]
    public ItemLibrary ItemsLibrary;
    public ParticleLibrary ParticleLibrary;
    public GameObjectLibrary GameObjectLibrary;
    public RuleTileLibrary RuleTileLibrary;

}