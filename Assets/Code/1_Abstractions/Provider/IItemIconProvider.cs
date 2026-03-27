using System.Collections.Generic;
using UnityEngine;

public interface IItemIconProvider
{
  Sprite GetIcon(int itemId);
}