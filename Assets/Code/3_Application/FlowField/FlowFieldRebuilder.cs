using UnityEngine;

[RequireComponent(typeof(Transform))]
public class FlowFieldRebuilder : MonoBehaviour
{
    public string FieldKey = "AttackPlayer";
    public float Interval = 0.18f;

    private float _timer = 0f;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= Interval)
        {
            _timer = 0f;
            FlowFieldRequestHub.Instance.RequestRebuild(FieldKey, transform.position);
        }
    }

    private void OnEnable()
    {
        TileDomainEvents.OnTileScanCompleted += ForceRebuild;
    }

    private void OnDisable()
    {
        TileDomainEvents.OnTileScanCompleted -= ForceRebuild;
    }

    private void ForceRebuild()
    {
        FlowFieldRequestHub.Instance.RequestRebuild(FieldKey, transform.position);
    }
}
