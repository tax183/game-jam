
using UnityEngine;
using System.Collections;

public class HazardScaler : MonoBehaviour
{
    public ItemType myType = ItemType.Rock;
    Vector3 original;

    void OnEnable(){ original = transform.localScale; GameEvents.OnInflateHazards += HandleInflate; }
    void OnDisable(){ GameEvents.OnInflateHazards -= HandleInflate; }

    void HandleInflate(float dur, float scale){
        if (myType == ItemType.Rock || myType == ItemType.Coin) StartCoroutine(InflateThenBack(dur, scale));
    }

    IEnumerator InflateThenBack(float dur, float scale){
        transform.localScale = original * scale;
        yield return new WaitForSeconds(dur);
        transform.localScale = original;
    }
}
