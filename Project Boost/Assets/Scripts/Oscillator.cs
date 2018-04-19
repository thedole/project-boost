using UnityEngine;

public class Oscillator : MonoBehaviour {
    [SerializeField]
    private Vector3 movementVector;
    [Range(0, 1)]
    [SerializeField]
    private float movementFactor;
    private Vector3 startingPosition;

    // Use this for initialization
    void Start () {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update () {
        var offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
	}
}
