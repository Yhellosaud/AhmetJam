using System;
using UnityEngine;

public class InputListener : MonoBehaviour
{
    private bool started;
    public static event Action<bool> OnTapToStart;

    private void Update()
    {
        if (started) return;

        if (Input.GetMouseButtonDown(0))
        {
            started = true;
            OnTapToStart?.Invoke(true);
        }
    }
    public void ManualStart()
    {
        started = true;
        OnTapToStart?.Invoke(true);
    }
    public void Reset()
    {
        started = false;
    }
}
