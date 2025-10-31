
using UnityEngine;

public class ItemMover : MonoBehaviour
{
    public ItemType type;
    public float speed = 5f;
    public float lifetime = 10f;

    int direction = 1;
    float born;

    void OnEnable(){ born = Time.time; GameEvents.OnDirectionChanged += SetDirection; }
    void OnDisable(){ GameEvents.OnDirectionChanged -= SetDirection; }

    public void SetDirection(int dir){ direction = dir; }

    void Update(){
        transform.Translate(Vector3.left * speed * direction * Time.deltaTime, Space.World);
        if (Time.time - born >= lifetime) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (!other.name.StartsWith("HitZone_")) return;
        switch(type){
            case ItemType.Straw: GameEvents.RaiseStraw(); Destroy(gameObject); break;
            case ItemType.Rock:  GameEvents.RaiseHeartLost(1); Destroy(gameObject); break;
            case ItemType.Date:  GameEvents.RaiseInflate(5f, 1.6f); Destroy(gameObject); break;
            case ItemType.Coin:  GameEvents.RaiseInstantFail(); Destroy(gameObject); break;
        }
    }
}
