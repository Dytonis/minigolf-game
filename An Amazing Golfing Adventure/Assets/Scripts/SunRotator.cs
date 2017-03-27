using UnityEngine;
using System.Collections;

public class SunRotator : MonoBehaviour
{
    public float limit;
    public float speed;

	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(new Vector3(speed * Time.deltaTime, 0, 0));

        if(transform.eulerAngles.x >= limit)
        {
            enabled = false;
        }
	}
}
