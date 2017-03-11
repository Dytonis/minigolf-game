using UnityEngine;
using System.Collections;

public class CameraSwivelSSO : MonoBehaviour
{

    public float Range = 5;
    private Vector3 StartingEuler;

	// Use this for initialization
	void Start ()
    {
        StartingEuler = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 RangeMousePosition = Input.mousePosition;

        if (RangeMousePosition.x > Screen.width) RangeMousePosition.x = Screen.width;
        if (RangeMousePosition.x < 0) RangeMousePosition.x = 0;
        if (RangeMousePosition.y > Screen.height) RangeMousePosition.y = Screen.height;
        if (RangeMousePosition.y < 0) RangeMousePosition.y = 0;

        Vector2 PositionNormalized = new Vector2((RangeMousePosition.x / (Screen.width/2)), (RangeMousePosition.y / (Screen.height/2))); //range of 0, 2

        Vector2 offset = new Vector2(PositionNormalized.x - 1f, PositionNormalized.y - 1f); //range of -1, 1

        transform.rotation = Quaternion.Euler(StartingEuler.x + (-offset.y * Range), StartingEuler.y + (offset.x * Range), StartingEuler.z);
    }
}
