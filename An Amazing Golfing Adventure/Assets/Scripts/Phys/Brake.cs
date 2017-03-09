using UnityEngine;
using System.Collections;

public class Brake : MonoBehaviour {

    Rigidbody rigid;

	// Use this for initialization
	void Start ()
    {
        rigid = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update ()
    {
	    if(IsGrounded())
        {
            rigid.drag = 0.8f / rigid.velocity.magnitude > 5f ? 5f : 0.8f / rigid.velocity.magnitude;
            rigid.angularDrag = 0.8f / rigid.velocity.magnitude > 5f ? 5f : 0.8f / rigid.velocity.magnitude;

            if(rigid.drag < 0.1f)
            {
                rigid.drag = 0f;
            }
            if(rigid.angularDrag < 0.05f)
            {
                rigid.angularDrag = 0.05f;
            }
        }
        else
        {
            rigid.drag = 0f;
            rigid.angularDrag = 0.05f;
        }
	}

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }
}
