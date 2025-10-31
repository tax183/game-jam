using UnityEngine;

public class PlayerBrickController : MonoBehaviour
{
    public float jumpHeight = 1.4f;     // أطول شوي افتراضيًا
    public float jumpUpTime = 0.25f;    // زمن صعود مريح
    public float jumpDownTime = 0.20f;  // زمن نزول أسرع قليلًا
    public float underOffset = 0.7f;

    public Collider2D hitTop;
    public Collider2D hitMid;
    public Collider2D hitBottom;

    private enum Pose { NormalMid, InvertedUnder, Jumping }
    private Pose pose = Pose.NormalMid;

    private float tStart;
    private Vector3 startPos, peakPos;

    void Start(){ UpdateHitzones(); }

    void Update()
    {
        if (pose != Pose.Jumping)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                EnterInverted();
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (pose == Pose.InvertedUnder) ExitInverted();
                else StartJump();
            }
        }

        if (pose == Pose.Jumping)
        {
            float t = (Time.time - tStart);
            if (t <= jumpUpTime)
            {
                float n = Mathf.Clamp01(t / jumpUpTime);
                n = Smooth(n); // ← smoothstep للصعود
                transform.localPosition = Vector3.Lerp(startPos, peakPos, n);
            }
            else if (t <= jumpUpTime + jumpDownTime)
            {
                float n = Mathf.Clamp01((t - jumpUpTime) / jumpDownTime);
                n = Smooth(n); // ← smoothstep للنزول
                transform.localPosition = Vector3.Lerp(peakPos, startPos, n);
            }
            else
            {
                transform.localPosition = startPos;
                pose = Pose.NormalMid;
                UpdateHitzones();
            }
        }
    }

    // Smoothstep: منحنى لطيف بدل lerp الخطي
    private float Smooth(float x) => x * x * (3f - 2f * x);

    void StartJump()
    {
        if (pose != Pose.NormalMid) return;
        pose = Pose.Jumping;
        startPos = Vector3.zero;
        peakPos  = new Vector3(0, jumpHeight, 0);
        tStart = Time.time;
        transform.localEulerAngles = Vector3.zero;
        SetZones(false, false, true); // top active during jump
    }

    void EnterInverted()
    {
        if (pose == Pose.InvertedUnder) return;
        pose = Pose.InvertedUnder;
        transform.localPosition = new Vector3(0, -underOffset, 0);
        transform.localEulerAngles = new Vector3(0, 0, 180f);
        UpdateHitzones();
    }

    void ExitInverted()
    {
        pose = Pose.NormalMid;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        UpdateHitzones();
    }

    void UpdateHitzones()
    {
        switch(pose)
        {
            case Pose.NormalMid:    SetZones(false, true,  false); break; // mid active
            case Pose.InvertedUnder:SetZones(true,  false, false); break; // bottom active
            case Pose.Jumping:      SetZones(false, false, true ); break; // top active
        }
    }

    void SetZones(bool bottom, bool mid, bool top)
    {
        if (hitBottom) hitBottom.enabled = bottom;
        if (hitMid)    hitMid.enabled = mid;
        if (hitTop)    hitTop.enabled = top;
    }
}

