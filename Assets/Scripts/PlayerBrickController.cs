using UnityEngine;

public class PlayerBrickController : MonoBehaviour
{
    // إعدادات القفز
    public float jumpHeight = 1.4f;     // ارتفاع القفز
    public float jumpUpTime = 0.25f;    // زمن صعود مريح
    public float jumpDownTime = 0.20f;  // زمن نزول أسرع قليلًا
    public float underOffset = 0.7f;    // المسافة عند النزول للأسفل (Inverted)

    [Header("Lane References (Optional - for auto-alignment)")]
    public Transform laneTop;   // Lane العلوي
    public Transform laneMid;   // Lane الوسط
    public Transform laneBottom; // Lane السفلي

    public Collider2D hitTop;   // كوليدر للمنطقة العلوية
    public Collider2D hitMid;   // كوليدر للمنطقة الوسطى
    public Collider2D hitBottom; // كوليدر للمنطقة السفلية

    private enum Pose { NormalMid, InvertedUnder, Jumping }
    private Pose pose = Pose.NormalMid;

    private float tStart;   // بداية التوقيت
    private Vector3 startPos, peakPos;   // بداية ونقطة الذروة

    void Start() 
    { 
        UpdateHitzones(); 
    }

    void Update()
    {
        if (pose != Pose.Jumping)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (pose == Pose.InvertedUnder) ExitInverted();
                else StartJump();  // يبدأ القفز للأعلى
            }

            // زر الأسفل يقوم بالنزول (Go inverted/under)
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                EnterInverted();
// ينقل الشخصية للأسفل (Inverted)
        }

        // التحقق إذا كانت الشخصية في وضع القفز
        if (pose == Pose.Jumping)
        {
            float t = (Time.time - tStart);
            if (t <= jumpUpTime)
            {
                // صعود الشخصية
                float n = Mathf.Clamp01(t / jumpUpTime);
                n = Smooth(n); // منحنى ناعم للصعود
                transform.localPosition = Vector3.Lerp(startPos, peakPos, n);
            }
            else if (t <= jumpUpTime + jumpDownTime)
            {
                // نزول الشخصية
                float n = Mathf.Clamp01((t - jumpUpTime) / jumpDownTime);
                n = Smooth(n); // منحنى ناعم للنزول
                transform.localPosition = Vector3.Lerp(peakPos, startPos, n);
            }
            else
            {
                transform.localPosition = startPos;
                pose = Pose.NormalMid;
                UpdateHitzones(); // تأكد من تحديث الـcolliders بعد الهبوط
            }
        }
    }

    // منحنى ناعم (Smoothstep)
    private float Smooth(float x) => x * x * (3f - 2f * x);

    // يبدأ القفز
    void StartJump()
    {
        if (pose != Pose.NormalMid) return;
        pose = Pose.Jumping;
        startPos = Vector3.zero;
        peakPos = new Vector3(0, jumpHeight, 0);
        tStart = Time.time;
        transform.localEulerAngles = Vector3.zero;
        UpdateHitzones(); // تأكد من تحديث الـhit zones عند البدء بالقفز
    }

    // دخول وضعية النزول (Inverted)
    void EnterInverted()
    {
        if (pose == Pose.InvertedUnder) return;
        pose = Pose.InvertedUnder;

        // حساب المسافة الصحيحة لمزامنة الكوليدر السفلي مع الـlane السفلي
        float calculatedOffset = underOffset;

        if (laneBottom != null && laneMid != null && hitBottom != null && hitMid != null)
        {
            // حساب الفرق بين موقع الـlane والموقع المحلي للكوليدرات
            float hitMidLocalY = hitMid.transform.localPosition.y;
            float hitBottomLocalY = hitBottom.transform.localPosition.y;
            float laneMidY = laneMid.position.y;
            float laneBottomY = laneBottom.position.y;

            // حساب الفرق بين الـlane والـhit zones
            float laneDifference = laneMidY - laneBottomY;
            float hitzoneDifference = hitMidLocalY - hitBottomLocalY;

            calculatedOffset = laneDifference - hitzoneDifference;

            // التأكد من أن المسافة موجبة
            if (calculatedOffset < 0) calculatedOffset = -calculatedOffset;
        }

        transform.localPosition = new Vector3(0, -calculatedOffset, 0);
        transform.localEulerAngles = new Vector3(0, 0, 180f);  // التدوير 180 درجة
        UpdateHitzones(); // تحديث الـhit zones عند الدخول في وضعية Inverted
    }

    // الخروج من وضعية النزول (Inverted)
    void ExitInverted()
    {
        pose = Pose.NormalMid;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        UpdateHitzones(); // تحديث الـhit zones عند العودة للوضع الطبيعي
    }

    // تحديث الـhit zones بناءً على الوضع الحالي
    void UpdateHitzones()
    {
        switch (pose)
        {
            case Pose.NormalMid:    SetZones(false, true,  false); break; // mid active
            case Pose.InvertedUnder:SetZones(true,  false, false); break; // bottom active
            case Pose.Jumping:      SetZones(false, false, true ); break; // top active
        }
    }

    // تفعيل أو إلغاء الـcolliders
    void SetZones(bool bottom, bool mid, bool top)
    {
        if (hitBottom) hitBottom.enabled = bottom;
        if (hitMid)    hitMid.enabled = mid;
        if (hitTop)    hitTop.enabled = top;
    }
}

