using System.Collections.Generic;
using UnityEngine;

public class FlowFieldRequestHub : MonoBehaviour
{
    public static FlowFieldRequestHub Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // simple forwarder to Director
    public void RequestRebuild(string key, Vector3 target)
    {
        if (FlowFieldDirector.Instance == null)
        {
            Debug.LogWarning("FlowFieldDirector missing");
            return;
        }
        FlowFieldDirector.Instance.ScheduleBuild(key, target);
    }
}