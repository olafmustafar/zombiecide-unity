using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public GameObject player;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;

    Vector3 cameraOffset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraOffset = new Vector3(-18, 20, -18);
    }

    void LateUpdate()
    {
        if( !player ){
            return;
        }
        Vector3 newPos = player.transform.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
    }
}
