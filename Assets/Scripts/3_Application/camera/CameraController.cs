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

  private Vector3 velocity;
  private Vector3 shakeOffset;

  public CameraState State { get; private set; } = CameraState.Follow;

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
    StopAllCoroutines();
    StartCoroutine(ZoomRoutine(size));
  }

  private System.Collections.IEnumerator ZoomRoutine(float targetSize)
  {
    while (!Mathf.Approximately(cam.orthographicSize, targetSize))
    {
      cam.orthographicSize = Mathf.Lerp(
        cam.orthographicSize,
        targetSize,
        zoomSpeed * Time.deltaTime
      );
      yield return null;
    }
  }

  public void ResetZoom()
  {
    ZoomTo(defaultSize);
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