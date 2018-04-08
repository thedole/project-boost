using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rotating rotating;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
	}

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            print("Thrust");
        }

        if(Input.GetKey(KeyCode.A) && rotating != Rotating.Right)
        {
            rotating = Rotating.Left;
            print("Rotate Left");
        }
        else if (Input.GetKeyUp(KeyCode.A) && rotating == Rotating.Left) {
            rotating = Rotating.None;
        }

        if (Input.GetKey(KeyCode.D) && rotating != Rotating.Left)
        {
            rotating = Rotating.Right;
            print("Rotate Right");
        }
        else if (Input.GetKeyUp(KeyCode.D) && rotating == Rotating.Right)
        {
            rotating = Rotating.None;
        }
    }
    enum Rotating { None, Left, Right };
}
