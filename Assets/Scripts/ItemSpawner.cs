using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Lanes")]
    public Transform laneTop, laneMid, laneBottom;

    [Header("Prefabs")]
    public GameObject strawPF, rockPF, datePF, coinPF;

    [Header("Spawn Settings")]
    public float spawnEvery   = 1.25f;
    public float itemSpeed    = 5f;
    public float itemLifetime = 10f;
    public float spawnX       = 11f;

    [Range(0,100)] public int strawWeight = 60;
    [Range(0,100)] public int rockWeight  = 20;
    [Range(0,100)] public int dateWeight  = 10;
    [Range(0,100)] public int coinWeight  = 10;

    [Header("Parent (optional)")]
    public Transform itemsParent;

    [Header("Timer Binding (MANDATORY)")]
    public GameController controller; // اسحبي عليه GameController من الهيَراركي

    [Header("Directions (half & half)")]
    [Tooltip("+1 = يولّد من اليمين ويتحرك لليسار | -1 = يولّد من اليسار ويتحرك لليمين")]
    public int firstHalfDirection  = +1;   // أول 30 ثانية
    public int secondHalfDirection = -1;   // آخر 30 ثانية

    [Header("Control")]
    public bool autoStart = true;

    // داخلي
    bool  spawning = false;
    float nextAt   = 0f;

    public void StartSpawning()
    {
        spawning = true;
        nextAt = Time.time + 0.2f; // أول دفعة بعد مهلة قصيرة
    }
    public void StopSpawning() => spawning = false;

    void Start()
    {
        if (itemsParent == null) itemsParent = transform;
        if (autoStart) StartSpawning();
    }

    void Update()
    {
      //  if (!spawning) return;

        // مربوطة مباشرة بالتايمر
        float left = (controller != null) ? controller.GetRemainingTime() : 0f;
        if (left <= 0f) { StopSpawning(); return; }

        // حددي الاتجاه حسب النصف
        float half = (controller != null) ? controller.roundSeconds * 0.5f : 30f;
        int desiredDir = (left > half) ? firstHalfDirection : secondHalfDirection;
        int dir = (desiredDir >= 0) ? +1 : -1;

        // سبون جدولي يعتمد على Time.time — لا يتوقف
        while (Time.time >= nextAt)
        {
            SpawnOne(dir);
            nextAt += spawnEvery;
        }
    }

    void SpawnOne(int dir)
    {
        var lane   = PickLane();
        var prefab = PickPrefab();
        if (!lane || !prefab) return;

        float x   = spawnX * (dir > 0 ? 1f : -1f);
        var   pos = new Vector3(x, lane.position.y, 0f);

        var go = Instantiate(prefab, pos, Quaternion.identity, itemsParent);

        var mover = go.GetComponent<ItemMover>() ?? go.AddComponent<ItemMover>();
        mover.speed    = itemSpeed;
        mover.lifetime = itemLifetime;
        mover.SetDirection(dir); // يتحرك بنفس اتجاه النصف الحالي

        var col = go.GetComponent<Collider2D>() ?? go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        
        // Ensure Rigidbody2D exists for trigger detection (kinematic, no gravity)
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.simulated = true;
        }
    }

    Transform PickLane()
    {
        int r = Random.Range(0, 3);
        if (r == 0) return laneTop; if (r == 1) return laneMid; return laneBottom;
    }

    GameObject PickPrefab()
    {
        int total = strawWeight + rockWeight + dateWeight + coinWeight;
        if (total <= 0) return strawPF;

        int r = Random.Range(0, total);
        if (r < strawWeight) return strawPF; r -= strawWeight;
        if (r < rockWeight)  return rockPF;  r -= rockWeight;
        if (r < dateWeight)  return datePF;
        return coinPF;
    }
}




