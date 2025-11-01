using UnityEngine;

public class PlayerBrickController : MonoBehaviour
{
    public float jumpHeight = 1.4f;     // أطول شوي افتراضيًا
    public float jumpUpTime = 0.25f;    // زمن صعود مريح
    public float jumpDownTime = 0.20f;  // زمن نزول أسرع قليلًا
    public float underOffset = 0.7f;

    [Header("Lane References (Optional - for auto-alignment)")]
    public Transform laneTop;
    public Transform laneMid;
    public Transform laneBottom;

    public Collider2D hitTop;
    public Collider2D hitMid;
    public Collider2D hitBottom;

    private enum Pose { NormalMid, InvertedUnder, Jumping }
    private Pose pose = Pose.NormalMid;

    private float tStart;
    private Vector3 startPos, peakPos;

    void Start() { UpdateHitzones(); }

    void Update()
    {
        if (pose != Pose.Jumping)
        {
            // زر الأعلى يقوم بالقفز (Jump up)
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (pose == Pose.InvertedUnder) ExitInverted();
                else StartJump();
            }

            // زر الأسفل يقوم بالنزول (Go inverted/under)
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                EnterInverted();
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
                UpdateHitzones(); // Ensure colliders are updated when landing
            }
        }
    }

    // Smoothstep: منحنى لطيف بدل lerp الخطي
    private float Smooth(float x) => x * x * (3f - 2f * x);

    void StartJump()
    {
        if (pose != Pose.NormalMid) return;
        pose = Pose.Jumping;
        // NormalMid position should always be zero
        startPos = Vector3.zero;
        peakPos = new Vector3(0, jumpHeight, 0);
        tStart = Time.time;
        transform.localEulerAngles = Vector3.zero;
        UpdateHitzones(); // Use UpdateHitzones for consistency
    }

    void EnterInverted()
    {
        if (pose == Pose.InvertedUnder) return;
        pose = Pose.InvertedUnder;
        
        // Calculate the correct offset to align bottom hitzone with bottom lane
        float calculatedOffset = underOffset;
        
        if (laneBottom != null && laneMid != null && hitBottom != null && hitMid != null)
        {
            // Get hitzone local positions
            float hitMidLocalY = hitMid.transform.localPosition.y;
            float hitBottomLocalY = hitBottom.transform.localPosition.y;
            
            // Get lane world positions
            float laneMidY = laneMid.position.y;
            float laneBottomY = laneBottom.position.y;
            
            // When at normal mid position (local 0,0,0):
            // Mid hitzone world Y = transform.parent.y + hitMidLocalY ≈ laneMidY
            
            // We need: when inverted, bottom hitzone world Y = laneBottomY
            // When inverted: player local position is (0, -offset, 0) and rotated 180°
            // After 180° rotation, the bottom hitzone's local position (x, y) becomes relative to rotated player
            // Since we're using local positions, the calculation is:
            // World bottom hitzone Y = parent.worldY + player.localY + hitBottomLocalY (accounting for rotation)
            
            // Simplified approach: Calculate offset needed to move from mid to bottom alignment
            // The offset = difference between where bottom hitzone would be at mid vs where it should be at bottom lane
            // offset ≈ (laneMidY - laneBottomY) - (hitMidLocalY - hitBottomLocalY)
            
            float laneDifference = laneMidY - laneBottomY;
            float hitzoneDifference = hitMidLocalY - hitBottomLocalY;
            
            // When we move down by offset, we want bottom hitzone to align with bottom lane
            // So: laneMidY - offset + (hitBottomLocalY - hitMidLocalY) ≈ laneBottomY
            // offset ≈ laneMidY - laneBottomY + (hitBottomLocalY - hitMidLocalY)
            calculatedOffset = laneDifference - hitzoneDifference;
            
            // Ensure offset is positive (moving down)
            if (calculatedOffset < 0) calculatedOffset = -calculatedOffset;
        }
        
        transform.localPosition = new Vector3(0, -calculatedOffset, 0);
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
        switch (pose)
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
