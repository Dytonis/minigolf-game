using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{

    public bool X, Y, Z;
    public float Speed;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(X)
        {
            transform.Rotate(new Vector3(Speed * Time.fixedDeltaTime, 0, 0));
        }
        if (Y)
        {
            transform.Rotate(new Vector3(0, Speed * Time.fixedDeltaTime, 0));
        }
        if (Z)
        {
            transform.Rotate(new Vector3(0, 0, Speed * Time.fixedDeltaTime));
        }
    }
}
