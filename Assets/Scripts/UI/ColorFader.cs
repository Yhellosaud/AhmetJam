using TMPro;
using UnityEngine;

public class ColorFader : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.01f;
    private TextMeshProUGUI text;
    private float alpha;
    private bool isReadyToFade;


    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        isReadyToFade = false;

        GameManager.OnGameStart += () => isReadyToFade = true;

        text.color = new Color(0f, 0f, 0f, 1f);

        alpha = 1f;
    }
    void OnDisable()
    {
        GameManager.OnGameStart -= () => isReadyToFade = true;
    }

    private void Update()
    {
        if (!isReadyToFade) return;

        if (alpha < 0.01f)
        {
            isReadyToFade = false;
            text.color = new Color(0f, 0f, 0f, 0f);
            gameObject.SetActive(false);
        }
        else
        {
            alpha = Mathf.Lerp(alpha, 0f, transitionTime);
            text.color = new Color(0f, 0f, 0f, alpha);
        }
    }
}
