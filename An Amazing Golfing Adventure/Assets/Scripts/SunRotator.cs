using UnityEngine;
using System.Collections;

public class SunRotator : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(new Vector3(0.005f, 0, 0));
	}
}
