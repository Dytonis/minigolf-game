using UnityEngine;
using System.Collections;

public class Pusher : MonoBehaviour
{
    public float Speed = 5f;
    public float Range;
    public float Timing;
    private Vector3 startingPos;
    public Vector3 Offset;
    void Start()
    {
        startingPos = transform.position;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 movement = transform.TransformDirection((new Vector3(0, Hermite(Mathf.PingPong((Time.time + Timing) * Speed, 1)), 0) * Range) + Offset);
        GetComponent<Rigidbody>().MovePosition(startingPos + movement);
	}

    float Hermite(float t)
    {
        return -t * t * t * 2f + t * t * 3f;
    }
}
