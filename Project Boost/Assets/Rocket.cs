using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rotating rotating;
    private Rigidbody rigidBody;
    private static Vector3 thrustVector = new Vector3(0f, 5500f, 0f);
    private const float rotationFactor = 4f;
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
        var zRotation = transform.localRotation.eulerAngles.z;
        var newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(0, 0, zRotation);
        transform.localRotation = newRotation;
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
            transform.Rotate(Vector3.forward * rotationFactor);
        }
        else if (rotating == Rotating.Left) {
            rotating = Rotating.None;
        }

        if (Input.GetKey(KeyCode.D) && rotating != Rotating.Left)
        {
            rotating = Rotating.Right;
            transform.Rotate(-Vector3.forward * rotationFactor);

        }
        else if (rotating == Rotating.Right)
        {
            rotating = Rotating.None;
        }
    }
    enum Rotating { None, Left, Right };
}
