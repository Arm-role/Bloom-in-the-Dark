using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
  public TMP_Text title;
  public TMP_Text description;
  [SerializeField] private Button button;

  UpgradeData data;

  private void Start()
  {
    button.onClick.AddListener(OnClick);
  }
  public void Setup(UpgradeData upgrade)
  {
    data = upgrade;

    title.text = upgrade.upgradeName;
    description.text = upgrade.description;
  }

  public void OnClick()
  {
    UpgradeManager.Instance.SelectUpgrade(data);
  }
}
