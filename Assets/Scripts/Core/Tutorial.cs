using DG.Tweening;
using LSCore;
using LSCore.ConfigModule;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public Spawner spawner;
    public Transform pointer;
    private int currentFingerId;
    private LSInput.Simulator.Touch touch;
    
    private void Start()
    {
        if (FirstTime.IsNot("Tutorial", out var pass))
        {
            LSInput.IsManualControl = true;
            pointer.position = spawner.transform.position;
            pointer.DOMove(Vector3.zero, 1f).SetLoops(-1, LoopType.Restart).OnStart(TouchDown).OnStepComplete(TouchDown);
        }
    }

    private void Update()
    {
        if (LSInput.TouchCount > 0)
        {
            touch.position = Camera.main.WorldToScreenPoint(pointer.position);
        }
    }

    private void TouchDown()
    {
        touch?.Release();
        touch = LSInput.Simulator.TouchDown(Camera.main.WorldToScreenPoint(pointer.position));
    }
}