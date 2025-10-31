
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public Transform laneTop, laneMid, laneBottom;

    public GameObject strawPF, rockPF, datePF, coinPF;

    public float spawnEvery = 1.25f;
    public float itemSpeed = 5f;
    public float itemLifetime = 10f;
    public float spawnX = 11f;
    [Range(0,100)] public int strawWeight = 60;
    [Range(0,100)] public int rockWeight  = 20;
    [Range(0,100)] public int dateWeight  = 10;
    [Range(0,100)] public int coinWeight  = 10;

    public Transform itemsParent;

    float nextAt = 0f;
    int dir = 1;

    void OnEnable(){ GameEvents.OnDirectionChanged += (d)=>dir=d; }
    void OnDisable(){ GameEvents.OnDirectionChanged -= (d)=>dir=d; }

    void Update(){
        if (Time.time >= nextAt){ SpawnOne(); nextAt = Time.time + spawnEvery; }
    }

    void SpawnOne(){
        var lane = PickLane();
        var prefab = PickPrefab();
        if (!lane || !prefab) return;

        float x = spawnX * (dir > 0 ? 1f : -1f);
        Vector3 pos = new Vector3(x, lane.position.y, 0f);
        var go = GameObject.Instantiate(prefab, pos, Quaternion.identity, itemsParent);

        var mover = go.GetComponent<ItemMover>(); if (!mover) mover = go.AddComponent<ItemMover>();
        mover.speed = itemSpeed;
        mover.lifetime = itemLifetime;
        mover.SetDirection(dir);

        var col = go.GetComponent<Collider2D>(); if (!col) col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    Transform PickLane(){
        int r = Random.Range(0,3);
        if (r==0) return laneTop;
        if (r==1) return laneMid;
        return laneBottom;
    }

    GameObject PickPrefab(){
        int total = strawWeight + rockWeight + dateWeight + coinWeight;
        int r = Random.Range(0, total);
        if (r < strawWeight) return strawPF; r -= strawWeight;
        if (r < rockWeight)  return rockPF;  r -= rockWeight;
        if (r < dateWeight)  return datePF;
        return coinPF;
    }
}
