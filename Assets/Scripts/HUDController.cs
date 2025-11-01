using UnityEngine;
using TMPro; // لمتابعة النصوص في التايمر والقش

public class HUDController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text strawText;
    public TMP_Text timerText;

    [Header("Hearts (Images)")]
    public GameObject[] heartImages; // مصفوفة من صور القلوب (UI Images)

    [Header("Refs")]
    public GameController controller;

    [Header("Panels")]
    public GameObject losePanel, winPanel;

    bool endedShown = false;

    void Start()
    {
        if (losePanel) losePanel.SetActive(false);
        if (winPanel)  winPanel.SetActive(false);

        if (controller != null)
        {
            UpdateHearts(controller.GetHearts());  // بدءاً من عدد القلوب
            UpdateStraw(controller.GetStraw());
            UpdateTimer(controller.roundSeconds);
        }
        else
        {
            UpdateHearts(0); // إذا كان GameController غير موجود
            UpdateStraw(0);
            UpdateTimer(0f);
        }
    }

    void OnEnable()
    {
        GameEvents.OnTimerTick      += UpdateTimer;
        GameEvents.OnStrawCollected += OnStraw;
        GameEvents.OnHeartLost      += OnHeartLost;
        GameEvents.OnInstantFail    += ShowLose;
        GameEvents.OnTimeUp         += ShowLose;
        GameEvents.OnGoalReached    += ShowWin;
    }

    void OnDisable()
    {
        GameEvents.OnTimerTick      -= UpdateTimer;
        GameEvents.OnStrawCollected -= OnStraw;
        GameEvents.OnHeartLost      -= OnHeartLost;
        GameEvents.OnInstantFail    -= ShowLose;
        GameEvents.OnTimeUp         -= ShowLose;
        GameEvents.OnGoalReached    -= ShowWin;
    }

    void OnStraw()
    {
        if (controller != null) UpdateStraw(controller.GetStraw());
    }

    void UpdateStraw(int v)
    {
        if (!strawText || controller == null) return;
        strawText.text = "🌾 " + v + "/" + controller.targetStraw;
    }

    // التفاعل مع فقدان القلوب
    void OnHeartLost(int _lost)
    {
        if (controller != null) UpdateHearts(controller.GetHearts());
    }

    // تحديث صور القلوب
    void UpdateHearts(int hearts)
    {
        if (heartImages == null || heartImages.Length == 0) return;

        // تأكد من عدد القلوب الذي سيتم إخفائه
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < hearts)
                heartImages[i].SetActive(true); // عرض القلب
            else
                heartImages[i].SetActive(false); // إخفاء القلب
        }
    }

    // التحديث الدوري للتايمر
    public void UpdateTimer(float secondsLeft)
    {
        if (!timerText) return;
        int total = Mathf.CeilToInt(Mathf.Max(0f, secondsLeft));
        int mm = total / 60;
        int ss = total % 60;
        timerText.text = string.Format("{0:00}:{1:00}", mm, ss);
    }

    void ShowLose()
    {
        if (endedShown) return;
        endedShown = true;
        if (winPanel)  winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(true);
    }

    void ShowWin()
    {
        if (endedShown) return;
        endedShown = true;
        if (losePanel) losePanel.SetActive(false);
        if (winPanel)  winPanel.SetActive(true);
    }
}
