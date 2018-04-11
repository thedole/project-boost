using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    GameObject player;
    float offsetY;
    int screenYLimit;
    bool tracking;
    float initialHeight;

	// Use this for initialization
	void Start () {
        offsetY = transform.position.y - player.transform.position.y;
        screenYLimit = Screen.height / 2;
        initialHeight = transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {
        if (player == null)
        {
            return;
        }

        if (tracking)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y + offsetY, transform.position.z);
        }

        var playerScreenCoordinateY = Camera.main.WorldToScreenPoint(player.transform.position).y;
       
        if (!tracking && Screen.height - playerScreenCoordinateY < screenYLimit)
        {
            tracking = true;
            offsetY = transform.position.y - player.transform.position.y;
        }
        else if (tracking && transform.position.y < initialHeight)
        {
            tracking = false;
        }
    }
}
