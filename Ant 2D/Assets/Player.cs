using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Ant2 proxy;
    public float angle;
	
    void Start()
    {
        proxy.SubmitControl();
    }
    
	void Update ()
    {
        angle = Mathf.Atan2(proxy.transform.position.y, proxy.transform.position.x);
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (proxy.GetMotion() == Ant2.backward)
            {
                proxy.InvertBend();
            }
            proxy.WalkForward();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (proxy.GetMotion() == Ant2.forward)
            {
                proxy.StopWalking();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            proxy.InvertBend();
            proxy.WalkBackward();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (proxy.GetMotion() == Ant2.backward)
            {
                proxy.InvertBend();
                proxy.StopWalking();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            proxy.TurnCounterClockwise();
            if (proxy.GetMotion() == Ant2.backward)
            {
                proxy.InvertBend();
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (proxy.GetDirection() == Ant2.counterclockwise)
            {
                proxy.Straighten();
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            proxy.TurnClockwise();
            if (proxy.GetMotion() == Ant2.backward)
            {
                proxy.InvertBend();
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (proxy.GetDirection() == Ant2.clockwise)
            {
                proxy.Straighten();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            proxy.StartGrabbing();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            proxy.Release();
        }
    }
}
