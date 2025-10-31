using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TumbleweedNatural : MonoBehaviour
{
    [Header("Horizontal")]
    public float moveSpeed = 3f;              // سرعة ثابتة يسار
    public float rotationSpeed = 220f;        // دوران

    [Header("Vertical Sway (على الأرض)")]
    public float swayAmplitude = 0.10f;       // مقدار التموج
    public float swayFrequency = 3.2f;        // سرعة التموج
    public float perlinGain = 0.06f;          // ضوضاء خفيفة

    [Header("Hops (طَقّات متباعدة)")]
    public float hopForceMin = 1.6f;
    public float hopForceMax = 2.6f;
    public float hopIntervalMin = 1.4f;
    public float hopIntervalMax = 2.2f;
    public float maxUpVelocity = 6f;          // سقف السرعة العمودية

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckPadding = 0.03f;

    Rigidbody2D rb;
    Collider2D col;
    float nextHopTime, baseY, t0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 1.2f;
        rb.freezeRotation = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        t0 = Random.Range(0f, 10f);
        baseY = transform.position.y;
        ScheduleNextHop();
    }

    void FixedUpdate()
    {
        // سرعة أفقية ثابتة يسار
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);

        // دوران متناسق مع الحركة
        rb.MoveRotation(rb.rotation + rotationSpeed * Time.fixedDeltaTime);

        bool grounded = IsGrounded();

        // تموّج طبيعي عند ملامسة الأرض (بدون رفع مصطنع كبير)
        if (grounded)
        {
            float t = Time.time + t0;
            float sway = Mathf.Sin(t * swayFrequency) * swayAmplitude;
            float noise = (Mathf.PerlinNoise(t * 0.8f, 0.0f) - 0.5f) * 2f * perlinGain;

            // نحرّك القشّة حول قاعدة Y بتموّج بسيط (نستخدم سرعة عمودية لطيفة)
            float targetVy = ((baseY + sway + noise) - transform.position.y) * 8f; // عامل تتبع ناعم
            // حد سرعة التموج عشان ما تصير “نطّة”
            targetVy = Mathf.Clamp(targetVy, -1.0f, 1.0f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + targetVy * Time.fixedDeltaTime);

            // طقّات متباعدة: قفزة قصيرة وعشوائية
            if (Time.time >= nextHopTime)
            {
                float hop = Random.Range(hopForceMin, hopForceMax);
                rb.AddForce(Vector2.up * hop, ForceMode2D.Impulse);
                if (rb.linearVelocity.y > maxUpVelocity)
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxUpVelocity);
                ScheduleNextHop();

                // عدّل القاعدة شوي حتى ما يثبت على نفس الـY (إحساس أرض غير تامة)
                baseY += Random.Range(-0.03f, 0.03f);
            }
        }
        // في الجو: لا نغيّر الـY إلا بالجاذبية/الدفعات
        else
        {
            if (rb.linearVelocity.y > maxUpVelocity)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxUpVelocity);
        }
    }

    void ScheduleNextHop() =>
        nextHopTime = Time.time + Random.Range(hopIntervalMin, hopIntervalMax);

    bool IsGrounded()
    {
        var b = col.bounds;
        float castDist = groundCheckPadding + 0.06f;
        RaycastHit2D hit = Physics2D.BoxCast(b.center, b.size * 0.95f, 0f, Vector2.down, castDist, groundMask);
        return hit.collider != null;
    }
}
