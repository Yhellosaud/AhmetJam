using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ColorTweener : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.1f;
    [SerializeField] private int waitTimeMiliseconds = 750;
    private Image img;
    private float alpha;
    private bool isFullyOpaque, isFullyTransparent;

    private CancellationTokenSource source;

    public static event Action OnTransitionComplete;

    private void Awake()
    {
        img = GetComponent<Image>();
    }
    private void OnEnable()
    {
        img.color = new Color(0f, 0f, 0f, 0f);
        isFullyOpaque = false;
        isFullyTransparent = true;
        alpha = 0f;
        source = new();
    }
    void OnDisable()
    {
        source.Cancel();
    }
    private async void CountForToggleOff()
    {
        await Task.Delay(waitTimeMiliseconds);

        if (source.IsCancellationRequested) return;

        isFullyTransparent = false;
    }

    private void Update()
    {
        if (!isFullyOpaque)
        {
            if (alpha > .99f)
            {
                isFullyOpaque = true;
                img.color = new Color(0f, 0f, 0f, 1f);
                CountForToggleOff();
            }
            else
            {
                alpha = Mathf.Lerp(alpha, 1f, transitionTime);
                img.color = new Color(0f, 0f, 0f, alpha);
            }
        }
        else if (!isFullyTransparent)
        {
            if (alpha < 0.01f && isFullyOpaque)
            {
                isFullyTransparent = true;
                img.color = new Color(0f, 0f, 0f, 0f);
                OnTransitionComplete?.Invoke();
                gameObject.SetActive(false);
            }
            else
            {
                alpha = Mathf.Lerp(alpha, 0f, transitionTime);
                img.color = new Color(0f, 0f, 0f, alpha);
            }
        }
    }
}
