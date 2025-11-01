using UnityEngine;

public class ItemMover : MonoBehaviour
{
    public ItemType type;
    public float speed = 5f;
    public float lifetime = 60f;

    [Header("Direction Control")]
    [Tooltip("إن فعلتها سيتابع تغيّر الاتجاه من GameEvents. اتركيها مطفأة لاتجاه ثابت يُمرر من الـSpawner.")]
    public bool followGlobalDirection = false;

    private int direction = +1; // +1: يمين→يسار، -1: يسار→يمين
    private float born;

    void OnEnable()
    {
        born = Time.time;
        if (followGlobalDirection)
            GameEvents.OnDirectionChanged += SetDirection;
    }

    void OnDisable()
    {
        if (followGlobalDirection)
            GameEvents.OnDirectionChanged -= SetDirection;
    }

    // يُستدعى من الـSpawner فور الإنشاء
    public void SetDirection(int dir)
    {
        direction = (dir >= 0) ? +1 : -1;
    }

    void Update()
    {
        // حركة ثابتة بحسب الإشارة
        float sign = (direction > 0) ? 1f : -1f;
        transform.Translate(Vector3.left * speed * sign * Time.deltaTime, Space.World);

        // إنهاء العمر
        if (Time.time - born >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print (other.name);
        if (!other.name.StartsWith("Hit")) return;
        print (other.name);
        switch (type)
        {
            case ItemType.Straw: GameEvents.RaiseStraw();        break;
            case ItemType.Rock:  GameEvents.RaiseHeartLost(1);    break;
            case ItemType.Date:  GameEvents.RaiseInflate(5f,1.6f);break;
            case ItemType.Coin:  GameEvents.RaiseInstantFail();   break;
        }
        Destroy(gameObject);
    }
}

