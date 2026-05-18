using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("Follow")]
  [SerializeField] private Transform target;
  [SerializeField] private float smoothTime = 0.2f;
  [SerializeField] private Vector3 offset;

  [Header("Zoom")]
  [SerializeField] private Camera cam;
  [SerializeField] private float zoomSpeed = 4f;
  [SerializeField] private float defaultSize = 5f;
  [SerializeField] private float minSize = 3f;
  [SerializeField] private float maxSize = 10f;
  [SerializeField] private float zoomStep = 1f;

  private float _targetSize;
  private Vector3 velocity;
  private Vector3 shakeOffset;

  public CameraState State { get; private set; } = CameraState.Follow;

  private void Awake() => _targetSize = defaultSize;

  void LateUpdate()
  {
    //Debug.Log(State.ToString());

    Vector3 basePos = cam.transform.position;

    if (State != CameraState.Locked && target)
    {
      Vector3 desired = target.position + offset;
      desired.z = cam.transform.position.z;

      basePos = Vector3.SmoothDamp(
        cam.transform.position,
        desired,
        ref velocity,
        smoothTime
      );
    }

    cam.transform.position = basePos + shakeOffset;

    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (Mathf.Abs(scroll) > 0.01f)
      _targetSize = Mathf.Clamp(_targetSize - scroll * zoomStep, minSize, maxSize);

    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, _targetSize, zoomSpeed * Time.deltaTime);
  }

  #region State

  public void SetState(CameraState state)
  {
    State = state;
  }

  #endregion

  #region Zoom

  public void ZoomTo(float size)
  {
    _targetSize = Mathf.Clamp(size, minSize, maxSize);
  }

  public void ResetZoom()
  {
    _targetSize = defaultSize;
  }

  #endregion

  #region Shake

  public void Shake(float duration, float strength)
  {
    StopCoroutine(nameof(ShakeRoutine));
    StartCoroutine(ShakeRoutine(duration, strength));
  }

  private System.Collections.IEnumerator ShakeRoutine(float duration, float strength)
  {
    float time = 0f;

    while (time < duration)
    {
      shakeOffset = Random.insideUnitCircle * strength;
      time += Time.deltaTime;
      yield return null;
    }

    shakeOffset = Vector3.zero;
  }

  #endregion
}