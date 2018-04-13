using Assets.Scripts;
using Extensions;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    private float mainThrust = 4500f;
    [SerializeField]
    float rotationThrust = 250f;
    [SerializeField]
    GameObject pieces;
    [SerializeField]
    private float nextLevelDelay = 1.5f;
    [SerializeField]
    private float dieDelay = 3;
    [SerializeField]
    AudioClip thrustClip;
    [SerializeField]
    AudioClip levelSuccessFanfareClip;
    [SerializeField]
    private ParticleSystem thrustParticles;
    [SerializeField]
    private ParticleSystem successParticles;
    [SerializeField]
    private ParticleSystem deathParticles;
    private ParticleSystem particles;

    public float MainThrust
    {
        get { return thrustVector.y; }
        set { thrustVector.y = value; }
    }

    private Rotating rotating;
    private Rigidbody rigidBody;
    private FadingAudioSource audioSource;
    private float mainThrustInternal;
    private Vector3 thrustVector;
    private int sceneIndex;
    private State state;
    private bool isThrusting;
    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = new FadingAudioSource(GetComponent<AudioSource>());
        thrustVector = new Vector3(0f, 0f, 0f);
        MainThrust = mainThrust;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        state = State.Alive;

        thrustParticles = Instantiate(thrustParticles, transform.position, transform.rotation);
        thrustParticles.transform.parent = transform;
        thrustParticles.transform.localPosition = new Vector3(0, -3.6f, 0);
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.FadeOut();
        isThrusting = false;
        if (thrustParticles.isPlaying)
        {
            StartCoroutine(StopParticleSystem(thrustParticles));
        }
    }

    private void ApplyThrust()
    {
        isThrusting = true;
        PlayParticles(thrustParticles);
        rigidBody.AddRelativeForce(Vector3.up);
        rigidBody.AddRelativeForce(thrustVector);
    }

    private void PlayParticles(ParticleSystem particleSystem)
    {
        if (particleSystem.isPlaying)
        {
            return;
        }
        particleSystem.Play();
    }

    private IEnumerator StopParticleSystem(ParticleSystem system)
    {
        yield return new WaitForSeconds(0.2f);
        if (!isThrusting)
        {
            system.Stop();
            system.Clear();
        }
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
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2000f);

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
