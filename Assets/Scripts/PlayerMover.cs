
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public Transform laneTop;
    public Transform laneMid;
    public Transform laneBottom;
    public float transitionTime = 0.18f;

    private enum Lane { Bottom=0, Mid=1, Top=2 }
    private Lane currentLane = Lane.Mid;
    private bool inTransition = false;
    private float tStart;
    private Vector3 fromPos, toPos;

    void Update()
    {
        // Handle input (PC)
        if (!inTransition)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                MoveTo(Lane.Top);
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                MoveTo(Lane.Bottom);
        }

        // Lerp movement
        if (inTransition)
        {
            float t = (Time.time - tStart) / transitionTime;
            if (t >= 1f)
            {
                transform.position = toPos;
                inTransition = false;
            }
            else
            {
                transform.position = Vector3.Lerp(fromPos, toPos, t);
            }
        }
    }

    void MoveTo(Lane lane)
    {
        if (lane == currentLane) return;
        Transform target = lane == Lane.Top ? laneTop : (lane == Lane.Mid ? laneMid : laneBottom);
        fromPos = transform.position;
        toPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
        tStart = Time.time;
        inTransition = true;
        currentLane = lane;
    }
}
