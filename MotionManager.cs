using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionManager : MonoBehaviour
{
    public static MotionManager instance;
    public delegate void MoveDelegate(string direction);
    public MoveDelegate MoveEvent;
    void Awake() {  instance = this; }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveEvent?.Invoke("left");
        }
        if (Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveEvent?.Invoke("right");
        }
    }
}
