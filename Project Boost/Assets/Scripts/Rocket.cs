using Extensions;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rotating rotating;
    private Rigidbody rigidBody;
    private FadingAudioSource audioSource;
    private static Vector3 thrustVector = new Vector3(0f, 5500f, 0f);
    private const float rotationThrust = 300f;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = new FadingAudioSource(GetComponent<AudioSource>());
	}

    private void FixedUpdate()
    {
        FixRotation();
        Thrust();
        Maneuver();
    }

    private void FixRotation()
    {
        var zRotation = transform.localRotation.eulerAngles.z;
        var newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(0, 0, zRotation);
        transform.localRotation = newRotation;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up);
            rigidBody.AddRelativeForce(thrustVector);
            if (!audioSource.IsPlaying || audioSource.FadingState == FadingState.FadingOut)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.IsPlaying)
        {
            audioSource.FadeOut();
        }
    }

    private void Maneuver()
    {
        var rotationSpeed = rotationThrust * Time.deltaTime;
        RotateLeft(rotationSpeed);
        RotateRight(rotationSpeed);
    }

    private void RotateRight(float rotationSpeed)
    {
        if (Input.GetKey(KeyCode.D) && rotating != Rotating.Left)
        {
            rotating = Rotating.Right;
            transform.Rotate(-Vector3.forward * rotationSpeed);

        }
        else if (rotating == Rotating.Right)
        {
            rotating = Rotating.None;
        }
    }

    private void RotateLeft(float rotationSpeed)
    {
        if (Input.GetKey(KeyCode.A) && rotating != Rotating.Right)
        {
            rotating = Rotating.Left;
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (rotating == Rotating.Left)
        {
            rotating = Rotating.None;
        }
    }

    enum Rotating { None, Left, Right };
}
