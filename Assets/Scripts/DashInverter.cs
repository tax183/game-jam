using UnityEngine;

public class DashInverter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("الكائن اللي ينقلب بالكامل (GameRoot عادةً)")]
    public Transform worldRoot;

    [Header("Dash Settings")]
    [Tooltip("كم ثانية يبقى العالم مقلوب")]
    public float dashDuration = 3f;
    [Tooltip("كم ثانية انتظار قبل السماح بداش جديد")]
    public float cooldown = 6f;

    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float nextAllowed = 0f;

    void Update()
    {
        // لو حالياً في داش
        if (isDashing)
        {
            if (Time.time >= dashEndTime)
                EndDash();
            return;
        }

        // التحكم بالكيبورد (Shift)
        bool shiftPressed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        if (shiftPressed && Time.time >= nextAllowed)
            StartDash();
    }

    // 🟢 زر UI
    public void OnDashButtonPressed()
    {
        if (Time.time >= nextAllowed && !isDashing)
        {
            Debug.Log("✅ UI Dash Button Pressed — Flipping World!");
            StartDash();
        }
        else
        {
            Debug.Log("⏳ Dash not ready yet (cooldown)");
        }
    }

    // 🔄 يقلب العالم
    private void StartDash()
    {
        if (worldRoot == null)
        {
            Debug.LogWarning("⚠️ DashInverter: لم يتم تعيين World Root!");
            return;
        }

        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        nextAllowed = Time.time + cooldown;

        // يقلب العالم 180 درجة
        worldRoot.rotation = Quaternion.Euler(0f, 0f, 180f);
        Debug.Log("🌍 World Flipped UPSIDE DOWN!");
    }

    // 🔁 يرجعه للوضع الطبيعي
    private void EndDash()
    {
        if (worldRoot == null) return;

        isDashing = false;
        worldRoot.rotation = Quaternion.identity;
        Debug.Log("↩️ World Returned to Normal");
    }
}

