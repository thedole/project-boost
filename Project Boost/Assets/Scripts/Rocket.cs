using Assets.Scripts;
using Extensions;
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
    private RocketInput input;

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
        input = new RocketInput();
        audioSource = new FadingAudioSource(GetComponent<AudioSource>());
        thrustVector = new Vector3(0f, 0f, 0f);
        MainThrust = mainThrust;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        state = State.Alive;

        InitializeThrustParticles();
        InitializeSuccessParticles();
    }

    private void InitializeThrustParticles()
    {
        thrustParticles = Instantiate(thrustParticles, transform.position, transform.rotation);
        thrustParticles.transform.parent = transform;
        thrustParticles.transform.localPosition = new Vector3(0, -3.6f, 0);
    }

    private void InitializeSuccessParticles()
    {
        successParticles = Instantiate(successParticles, transform.position, transform.rotation);
        successParticles.transform.parent = transform;
        successParticles.transform.localPosition = Vector3.zero;
    }

    void OnValidate()
    {
        MainThrust = mainThrust;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        ProcessThrustInput();
        ProcessManeuverInput();
    }

    private void ProcessThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            input.Thrust = true;
        }
        else
        {
            input.Thrust = false;
        }
    }

    private void ProcessManeuverInput()
    {
        ProcessLeftRotationInput();
        ProcessRigthRotationInput();
    }

    private void ProcessRigthRotationInput()
    {
        if (Input.GetKey(KeyCode.A) && rotating != Rotating.Right)
        {
            input.Maneuver = RocketInput.ManeuverDirection.Left;

        }
        else if (input.Maneuver == RocketInput.ManeuverDirection.Left)
        {
            input.Maneuver = RocketInput.ManeuverDirection.None;
        }
    }

    private void ProcessLeftRotationInput()
    {
        if (Input.GetKey(KeyCode.D) && rotating != Rotating.Left)
        {
            input.Maneuver = RocketInput.ManeuverDirection.Right;

        }
        else if (input.Maneuver == RocketInput.ManeuverDirection.Right)
        {
            input.Maneuver = RocketInput.ManeuverDirection.None;
        }
    }
    
    private void FixedUpdate()
    {
        if (!(state == State.Alive))
        {
            return;
        }

        FixRotation();
        HandleThrustInput();
        Maneuver();
    }

    private void FixRotation()
    {
        var zRotation = transform.localRotation.eulerAngles.z;
        var newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(0, 0, zRotation);
        transform.localRotation = newRotation;
    }

    private void HandleThrustInput()
    {
        if (input.Thrust)
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

    private void Maneuver()
    {
        var rotation = rotationThrust * Time.deltaTime;
        var direction = input.Maneuver == RocketInput.ManeuverDirection.Left
            ? Vector3.forward
            : -Vector3.forward;

        if (input.Maneuver != RocketInput.ManeuverDirection.None)
        {
            transform.Rotate(direction * rotation);
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
        PlayParticles(successParticles);
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
        //PlayParticles(deathParticles);
        var brokenRocket = Instantiate(pieces, transform.position, transform.rotation);
        PlayDeathParticles(brokenRocket);
        var parts = brokenRocket.GetComponentsInChildren<Rigidbody>();
        var explosionForce = collision.impulse.magnitude * 15;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2000f);

        foreach (var part in parts)
        {
            part.AddExplosionForce(explosionForce, collision.contacts.First().point, 30f, 2f);
        }
    }

    private void PlayDeathParticles(GameObject brokenRocket)
    {
        deathParticles = Instantiate(deathParticles, brokenRocket.transform.position, brokenRocket.transform.rotation);
        deathParticles.transform.parent = brokenRocket.transform;
        deathParticles.transform.localPosition = Vector3.zero;
        deathParticles.Clear();
        deathParticles.Play();
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(dieDelay);
        SceneManager.LoadScene(sceneIndex);
    }

    enum Rotating { None, Left, Right };
    enum State { Alive, Dying, Transitioning };
}
