using UnityEngine;

public class UpgradeMock : MonoBehaviour
{
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.U))
    {
      UpgradeManager.Instance.OpenUpgradePopup();
    }
  }
}