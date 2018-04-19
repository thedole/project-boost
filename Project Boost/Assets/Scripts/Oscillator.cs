using UnityEngine;

public class Oscillator : MonoBehaviour {
    [SerializeField]
    private Vector3 movementVector;
    [Range(0, 1)]
    [SerializeField]
    private float movementFactor;
    [SerializeField]
    private float period = 2f;

    private Vector3 startingPosition;
    private float tau;

    // Use this for initialization
    void Start () {
        tau = Mathf.PI * 2;
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update () {
        var cycles = Time.time / period;
        var rawSine = Mathf.Sin(cycles * tau);
        movementFactor = rawSine / 2 + .5f;

        var offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
	}
}
