using UnityEngine;
using System.Collections;

public class TransformToObject : MonoBehaviour
{
    public Transform Anchor;
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.position = Anchor.transform.position;
	}
}
