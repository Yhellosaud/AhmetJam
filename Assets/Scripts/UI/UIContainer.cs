using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIContainer : MonoBehaviour
{
    [SerializeField] private GameObject tapToStartObj;
    [SerializeField] private TextMeshProUGUI timeTmp, levelTmp;
    [SerializeField] private GameObject transitionObj, failObj, passObj;
    [SerializeField] private Button retryBtn, passBtn, nextLevelBtn;
    [SerializeField] private GameManager gameManager;

    private bool isFirst = true;

    private void Awake()
    {
        retryBtn.onClick.AddListener(() => gameManager.Restart());
        retryBtn.onClick.AddListener(() => MakeTransition(false));
        passBtn.onClick.AddListener(() => gameManager.Pass());
        passBtn.onClick.AddListener(() => ToggleOnPassScreen());
        nextLevelBtn.onClick.AddListener(() => gameManager.Restart());
        // nextLevelBtn.onClick.AddListener(() => MakeTransition(false));
        nextLevelBtn.onClick.AddListener(() => ToggleOffPassScreen());

        MakeTransition(false);
    }

    private void OnEnable()
    {
        InputListener.OnTapToStart += MakeTransition;
        Timer.OnTimeFinished += ToggleOnFailScreen;
    }
    private void OnDisable()
    {
        InputListener.OnTapToStart -= MakeTransition;
        Timer.OnTimeFinished += ToggleOnFailScreen;
    }

    public void SetTimerText(float time)
    {
        timeTmp.text = time.ToString("n2");
    }
    public void MakeTransition(bool type)
    {
        failObj.SetActive(false);
        tapToStartObj.SetActive(!type);
        if (!isFirst)
        {
            ToggleTimeText(false);
            transitionObj.SetActive(true);
        }
        if (type) levelTmp.gameObject.SetActive(false);

        isFirst = false;
    }
    public void ToggleOnFailScreen()
    {
        failObj.SetActive(true);
    }
    public void ToggleOnPassScreen()
    {
        passObj.SetActive(true);
        passBtn.gameObject.SetActive(false);
        ToggleTimeText(false);
    }
    public void ToggleOffPassScreen()
    {
        passObj.SetActive(false);
        tapToStartObj.SetActive(true);
        levelTmp.gameObject.SetActive(false);
    }
    public void ToggleTimeText(bool type)
    {
        timeTmp.gameObject.SetActive(type);
    }
    public void SetLevelText(int level)
    {
        levelTmp.gameObject.SetActive(true);
        levelTmp.text = "Level " + level;
        passBtn.gameObject.SetActive(true);
    }
}
