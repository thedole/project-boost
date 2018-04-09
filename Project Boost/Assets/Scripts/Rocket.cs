using Extensions;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rotating rotating;
    private Rigidbody rigidBody;
    private FadingAudioSource audioSource;
    [SerializeField]
    private float mainThrust = 4500f;
    private float mainThrustInternal;
    public float MainThrust {
        get { return thrustVector.y; }
        set { thrustVector.y = value; }
    }
    private Vector3 thrustVector;
    [SerializeField]
    float rotationThrust = 250f;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = new FadingAudioSource(GetComponent<AudioSource>());
        thrustVector = new Vector3(0f, 0f, 0f);
        MainThrust = mainThrust;
    }

    void OnValidate()
    {
        MainThrust = mainThrust;
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
        var rotation = rotationThrust * Time.deltaTime;
        RotateLeft(rotation);
        RotateRight(rotation);
    }

    private void RotateRight(float rotation)
    {
        if (Input.GetKey(KeyCode.D) && rotating != Rotating.Left)
        {
            rotating = Rotating.Right;
            transform.Rotate(-Vector3.forward * rotation);

        }
        else if (rotating == Rotating.Right)
        {
            rotating = Rotating.None;
        }
    }

    private void RotateLeft(float rotation)
    {
        if (Input.GetKey(KeyCode.A) && rotating != Rotating.Right)
        {
            rotating = Rotating.Left;
            transform.Rotate(Vector3.forward * rotation);
        }
        else if (rotating == Rotating.Left)
        {
            rotating = Rotating.None;
        }
    }

    enum Rotating { None, Left, Right };
}
