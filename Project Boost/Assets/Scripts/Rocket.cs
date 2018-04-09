using Extensions;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rotating rotating;
    private Rigidbody rigidBody;
    private FadingAudioSource audioSource;
    private static Vector3 thrustVector = new Vector3(0f, 5500f, 0f);
    private const float rotationFactor = 4f;

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
        RotateLeft();
        RotateRight();
    }

    private void RotateRight()
    {
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

    private void RotateLeft()
    {
        if (Input.GetKey(KeyCode.A) && rotating != Rotating.Right)
        {
            rotating = Rotating.Left;
            transform.Rotate(Vector3.forward * rotationFactor);
        }
        else if (rotating == Rotating.Left)
        {
            rotating = Rotating.None;
        }
    }

    enum Rotating { None, Left, Right };
}
