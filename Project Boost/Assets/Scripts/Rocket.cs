using Assets.Scripts;
using Extensions;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    GameObject pieces;
    private int sceneIndex;
    private State state;
    [SerializeField]
    private float nextLevelDelay = 2;
    [SerializeField]
    private float dieDelay = 3;
    [SerializeField]
    AudioClip thrustClip;
    [SerializeField]
    AudioClip levelSuccessFanfareClip;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = new FadingAudioSource(GetComponent<AudioSource>());
        thrustVector = new Vector3(0f, 0f, 0f);
        MainThrust = mainThrust;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        state = State.Alive;
    }

    void OnValidate()
    {
        MainThrust = mainThrust;
    }

    private void FixedUpdate()
    {
        if (!(state == State.Alive))
        {
            return;
        }

        FixRotation();
        ProcessThrustInput();
        ProcessManeuveringInput();
    }

    private void FixRotation()
    {
        var zRotation = transform.localRotation.eulerAngles.z;
        var newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(0, 0, zRotation);
        transform.localRotation = newRotation;
    }

    private void ProcessThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
            audioSource.PlayOneShot(thrustClip);
            
        }
        else
        {
            audioSource.FadeOut();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up);
        rigidBody.AddRelativeForce(thrustVector);
    }

    private void ProcessManeuveringInput()
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

    private void OnCollisionEnter(Collision collision)
    {

        if (!(state == State.Alive))
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case Tags.Friendly:
                // Do Nothing
                    break;
            case Tags.Finish:
                TransitionToNextLevel();
                break;
            default:
                Die(collision);
                break;
        }
    }

    private void TransitionToNextLevel()
    {
        var nextSceneIndex = sceneIndex + 1;
        // This is the final level
        if (SceneManager.sceneCountInBuildSettings - nextSceneIndex < 1)
        {
            return;
        }
        state = State.Transitioning;
        audioSource.PlayOneShot(levelSuccessFanfareClip);
        StartCoroutine(NextLevel());
    }

    private IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(nextLevelDelay);
        SceneManager.LoadScene(sceneIndex + 1);
    }

    private void Die(Collision collision)
    {
        state = State.Dying;
        audioSource.Stop();
        Explode(collision);
        StartCoroutine(RestartLevel());
    }

    private void Explode(Collision collision)
    {
        var brokenRocket = Instantiate(pieces, transform.position, transform.rotation);
        var parts = brokenRocket.GetComponentsInChildren<Rigidbody>();
        var explosionForce = collision.impulse.magnitude * 15;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, - 2000f);

        foreach (var part in parts)
        {
            part.AddExplosionForce(explosionForce, collision.contacts.First().point, 30f, 2f);
        }
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(dieDelay);
        SceneManager.LoadScene(sceneIndex);
    }

    enum Rotating { None, Left, Right };
    enum State { Alive, Dying, Transitioning };
}
