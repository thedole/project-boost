using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rotating rotating;
    private Rigidbody rigidBody;
    private static Vector3 thrustVector = new Vector3(0f, 5500f, 0f);
    private static Vector3 leftTorque = new Vector3(0f, 0f, 9000f);
    private static Vector3 rightTorque = new Vector3(0f, 0f, -9000f);


    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void FixedUpdate()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(thrustVector);
        }

        if(Input.GetKey(KeyCode.A) && rotating != Rotating.Right)
        {
            rotating = Rotating.Left;
            rigidBody.AddRelativeTorque(leftTorque);
        }
        else if (rotating == Rotating.Left) {
            rotating = Rotating.None;
        }

        if (Input.GetKey(KeyCode.D) && rotating != Rotating.Left)
        {
            rotating = Rotating.Right;
            rigidBody.AddRelativeTorque(rightTorque);

        }
        else if (rotating == Rotating.Right)
        {
            rotating = Rotating.None;
        }
    }
    enum Rotating { None, Left, Right };
}
