
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Text strawText, heartsText, timerText;
    public GameController controller;
    public GameObject losePanel, winPanel;

    void Start(){ UpdateHearts(controller.GetHearts()); UpdateStraw(controller.GetStraw()); }

    void OnEnable(){
        GameEvents.OnTimerTick += UpdateTimer;
        GameEvents.OnStrawCollected += OnStraw;
        GameEvents.OnHeartLost += UpdateHearts;
        GameEvents.OnInstantFail += ShowLose;
        GameEvents.OnTimeUp += ShowLose;
        GameEvents.OnGoalReached += ShowWin;
    }
    void OnDisable(){
        GameEvents.OnTimerTick -= UpdateTimer;
        GameEvents.OnStrawCollected -= OnStraw;
        GameEvents.OnHeartLost -= UpdateHearts;
        GameEvents.OnInstantFail -= ShowLose;
        GameEvents.OnTimeUp -= ShowLose;
        GameEvents.OnGoalReached -= ShowWin;
    }

    void OnStraw(){ UpdateStraw(controller.GetStraw()); }
    void UpdateStraw(int v){ if (strawText) strawText.text = "🌾 " + v + "/" + controller.targetStraw; }
    void UpdateHearts(int v){ if (heartsText) heartsText.text = "❤ " + Mathf.Max(0, v); }
    void UpdateTimer(float t){ if (timerText) timerText.text = Mathf.CeilToInt(t).ToString() + "s"; }

    void ShowLose(){ if (losePanel) losePanel.SetActive(true); }
    void ShowWin(){ if (winPanel) winPanel.SetActive(true); }
}
