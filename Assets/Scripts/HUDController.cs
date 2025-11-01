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
    bool heartsInitialized = false;

    void Start()
    {
        if (losePanel) losePanel.SetActive(false);
        if (winPanel)  winPanel.SetActive(false);
        InitializeUI();
    }
    
    void InitializeUI()
    {
        // Try to find controller if not assigned
        if (controller == null)
        {
            controller = FindObjectOfType<GameController>();
        }
        
        if (controller != null)
        {
            // Explicitly show all hearts at start - use the public hearts field
            int initialHearts = controller.hearts; // Access public field directly
            UpdateHearts(initialHearts);
            UpdateStraw(controller.GetStraw());
            UpdateTimer(controller.roundSeconds);
        }
        else
        {
            // Default to 3 hearts if controller missing
            Debug.LogWarning("HUDController: GameController not found! Using default values.");
            UpdateHearts(3);
            UpdateStraw(0);
            UpdateTimer(60f);
        }
    }
    
    void LateUpdate()
    {
        // Ensure hearts are shown on first frame if they weren't initialized properly
        if (!heartsInitialized && heartImages != null && heartImages.Length > 0 && controller != null)
        {
            int currentHearts = controller.GetHearts();
            UpdateHearts(currentHearts); // Re-initialize hearts
            heartsInitialized = true; // Only run once
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
        // Update immediately - GameController should have already processed since events
        // are typically handled in subscription order, but if not, we'll use coroutine as fallback
        // Try immediate update first
        if (controller != null)
        {
            UpdateStraw(controller.GetStraw());
        }
        else
        {
            // Try to find controller if not assigned
            controller = FindObjectOfType<GameController>();
            if (controller != null)
            {
                UpdateStraw(controller.GetStraw());
            }
        }
        
        // Also schedule a delayed update as safety net to catch any timing issues
        StartCoroutine(UpdateStrawDelayed());
    }
    
    System.Collections.IEnumerator UpdateStrawDelayed()
    {
        // Wait one frame to catch any cases where immediate update missed the change
        yield return null;
        
        // Get the updated straw count
        if (controller != null)
        {
            UpdateStraw(controller.GetStraw());
        }
    }

    void UpdateStraw(int v)
    {
        if (!strawText || controller == null) return;
        strawText.text = "🌾 " + v + "/" + controller.targetStraw;
    }

    // التفاعل مع فقدان القلوب
    void OnHeartLost(int _lost)
    {
        // Update immediately - GameController should have already processed since events
        // are typically handled in subscription order, but if not, we'll use coroutine as fallback
        // Try immediate update first
        if (controller != null)
        {
            UpdateHearts(controller.GetHearts());
        }
        else
        {
            // Try to find controller if not assigned
            controller = FindObjectOfType<GameController>();
            if (controller != null)
            {
                UpdateHearts(controller.GetHearts());
            }
        }
        
        // Also schedule a delayed update as safety net to catch any timing issues
        StartCoroutine(UpdateHeartsDelayed());
    }
    
    System.Collections.IEnumerator UpdateHeartsDelayed()
    {
        // Wait one frame to catch any cases where immediate update missed the change
        yield return null;
        
        // Get the updated hearts value
        if (controller != null)
        {
            UpdateHearts(controller.GetHearts());
        }
    }

    // تحديث صور القلوب
    void UpdateHearts(int hearts)
    {
        if (heartImages == null || heartImages.Length == 0)
        {
            return;
        }

        // Ensure hearts is within valid range
        hearts = Mathf.Clamp(hearts, 0, heartImages.Length);

        // Show/hide hearts based on count
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                // Show heart if index is less than heart count (0-indexed)
                // e.g., if hearts = 3, show indices 0, 1, 2
                // if hearts = 2, show indices 0, 1 (hide index 2)
                bool shouldShow = i < hearts;
                
                // Always set active state to ensure it updates (UI sometimes needs forced refresh)
                heartImages[i].SetActive(shouldShow);
            }
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
