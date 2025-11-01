using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public Transform target;      // الصندوق كهدف.
    public float moveSpeed = 5f;  // سرعة الحركة.
    public float stopDistance = 0.1f;  // المسافة التي تُعتبر "وصلت" عندها.

    private Rigidbody2D rb;       // Rigidbody2D لتمكين الحركة السلسة.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // ربط الـRigidbody2D.

        // التحقق إذا كان الكائن يحتوي على الـTag "Player"
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("This is the player object!");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // حساب الاتجاه نحو الصندوق.
            Vector2 direction = (target.position - transform.position).normalized;

            // حساب المسافة بين الشخصية والصندوق.
            float distance = Vector2.Distance(transform.position, target.position);

            // إذا كانت الشخصية لا تزال بعيدة عن الصندوق
            if (distance > stopDistance)
            {
                // استخدم MoveTowards لتحريك الشخصية بسرعة ثابتة نحو الصندوق
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                // إذا وصلت، إيقاف الحركة.
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
