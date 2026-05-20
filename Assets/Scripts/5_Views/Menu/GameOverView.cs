using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
  [SerializeField] private Button _retryButton;
  [SerializeField] private Button _mainMenuButton;
  [SerializeField] private string _retrySceneName;
  [SerializeField] private string _mainMenuSceneName;

  private void Awake()
  {
    _retryButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(_retrySceneName));
    _mainMenuButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(_mainMenuSceneName));
  }
}
