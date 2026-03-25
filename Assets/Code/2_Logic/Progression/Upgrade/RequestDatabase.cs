using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/RequestDatabase")]
public class RequestDatabase : ScriptableObject
{
  public List<UpgradeRequestDefinition> requests;
}