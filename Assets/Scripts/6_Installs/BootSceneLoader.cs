using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLoader : MonoBehaviour
{
  [SerializeField] private string _firstSceneName;

  private void Start()
  {
    if (AppInstaller.IsReady)
      LoadFirstScene();
    else
      AppInstaller.OnServiceReady += HandleServiceReady;
  }

  private void OnDestroy()
  {
    AppInstaller.OnServiceReady -= HandleServiceReady;
  }

  private void HandleServiceReady(DIContainerBase _)
  {
    AppInstaller.OnServiceReady -= HandleServiceReady;
    LoadFirstScene();
  }

  private void LoadFirstScene()
  {
    if (string.IsNullOrEmpty(_firstSceneName))
    {
      Debug.LogWarning("[BootSceneLoader] _firstSceneName is empty");
      return;
    }
    SceneManager.LoadScene(_firstSceneName);
  }
}
