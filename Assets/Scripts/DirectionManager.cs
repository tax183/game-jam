
using UnityEngine;

public class DirectionManager : MonoBehaviour
{
    public float switchEverySeconds = 10f;
    public int Direction { get; private set; } = 1;
    float nextSwitch;

    void Start(){ nextSwitch = Time.time + switchEverySeconds; GameEvents.RaiseDirection(Direction); }
    void Update(){
        if (Time.time >= nextSwitch){
            Direction = -Direction;
            nextSwitch = Time.time + switchEverySeconds;
            GameEvents.RaiseDirection(Direction);
        }
    }
}
