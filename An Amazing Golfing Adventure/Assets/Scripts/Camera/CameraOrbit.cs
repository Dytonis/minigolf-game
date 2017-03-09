using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class CameraOrbit : MonoBehaviour
{
    public Transform AnchorY;
    public Transform AnchorX;

    public Vector2 LimitsLook;
    public Vector2 LimitsScroll;

    public bool EnabledX;
    public bool EnabledY;

    public Vector2 rotation;

    // Update is called once per frame
    private Vector3 oldMousePosition = new Vector3(-1, -1, -1);

	void Update ()
    {
        Vector3 dif = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (EnabledX)
        {
            rotation.x += (dif.x * GameSettings.CameraSensitivity);

            AnchorX.transform.localEulerAngles = new Vector3(0, rotation.x);
        }

        if (EnabledY)
        {
            if (dif.y < 0)
            {
                if (rotation.y > LimitsLook.x)
                    rotation.y += (dif.y * GameSettings.CameraSensitivity);
            }
            else if (dif.y > 0)
            {
                if (rotation.y < LimitsLook.y)
                    rotation.y += (dif.y * GameSettings.CameraSensitivity);
            }

            AnchorY.transform.localEulerAngles = new Vector3(rotation.y, 0);
        }

        float scrollDelta = 0;

        if (Input.mouseScrollDelta.y > 0)
            scrollDelta = 1;
        else if (Input.mouseScrollDelta.y < 0)
            scrollDelta = -1;

        if(Input.mouseScrollDelta.y < 0)
        {
            if (transform.localPosition.z > LimitsScroll.y)
                transform.Translate(0, 0, scrollDelta / 2);
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            if (transform.localPosition.z < LimitsScroll.x)
                transform.Translate(0, 0, scrollDelta / 2);
        }
    }
}
