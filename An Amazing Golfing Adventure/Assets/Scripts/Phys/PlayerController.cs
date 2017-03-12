﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public bool IsClientControlled = true;

    public GameObject PowerIndicator;
    public GameObject XIndicator;
    public CameraOrbit Orbiter;
    public float PowerLimit;
    public float SpeedSoundLimit;
    public float SpeedSoundDropLimit;

    public float Power;

    public bool Playable;
    public float PlayableSeconds;
    public float AirTime;
    public float AirTimeLastFrame;
    public float StationaryUnplayableTime;

    public Vector3 LastPositionRTS;

    public AudioSource RollingCarpet;
    public AudioSource BounceCarpet;
    public AudioSource BounceBrick;
    public AudioSource BounceConcrete;
    public AudioSource BounceLog;
    public AudioSource BounceWood;

    public Game game;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            IsClientControlled = true;
            //game.CmdJoin(gameObject);
        }
    }
   
    void Update()
    {
        if(!IsClientControlled)
        {
            return;
        }

        AirTimeLastFrame = AirTime;
        RaycastHit info;
        if (IsGrounded(out info))
        {
            AirTime = 0;

            if (info.collider.gameObject.tag == "Carpet")
            {
                if (!RollingCarpet.isPlaying)
                    RollingCarpet.Play();
                RollingCarpet.volume = Mathf.Clamp01(GetComponent<Rigidbody>().velocity.magnitude / SpeedSoundLimit);
            }
            if (info.collider.gameObject.tag == "Carpet" || info.collider.gameObject.tag == "Brick" || info.collider.gameObject.tag == "Concrete")
            {
                if (IsStationary())
                    Playable = true;
                else
                    Playable = false;
            }
            else
            {
                RollingCarpet.Stop();
                Playable = false;
            }
        }
        else
        {
            RollingCarpet.Stop();
            Playable = false;
            AirTime += Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
        {
            Orbiter.EnabledY = false;
        }

        if (Playable)
        {
            StationaryUnplayableTime = 0;
            PlayableSeconds += Time.deltaTime;
            if (PlayableSeconds >= 0.2)
            {
                LastPositionRTS = transform.localPosition;
                GetComponent<MeshRenderer>().materials[0].SetColor("_OutlineColor", Color.grey);

                if (Input.GetMouseButton(0))
                {
                    PowerIndicator.SetActive(true);
                    Power += Input.GetAxisRaw("Mouse Y");
                    if (Power <= 0)
                    {
                        Power = 0;
                    }
                    if (Power >= PowerLimit)
                    {
                        Power = PowerLimit;
                    }

                    Orbiter.EnabledY = false;
                }
                else
                {
                    Orbiter.EnabledY = true;
                }
            }
        }
        else
        {
            if(IsStationary())
            {
                StationaryUnplayableTime += Time.deltaTime;
                if(StationaryUnplayableTime >= 6f)
                {
                    ResetVelocityAndPosition();
                    ResetPosition(gameObject, LastPositionRTS);
                }
            }

            PlayableSeconds = 0;
            GetComponent<MeshRenderer>().materials[0].SetColor("_OutlineColor", Color.black);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(Power >= 0 && IsStationary())
            {
                GetComponent<Rigidbody>().AddForce(XIndicator.transform.forward * Mathf.Pow(3.6f * Power, 0.88f) * 10);
                Power = 0;
                PowerIndicator.SetActive(false);
            }
        }

        PowerIndicator.transform.localScale = new Vector3(0.05f, 0.05f, Power / 40);
        PowerIndicator.transform.rotation = XIndicator.transform.rotation;
        PowerIndicator.transform.position = transform.position;
    }

    void OnTriggerEnter(Collider col)
    {
        print(col.gameObject.tag);
        if (col.gameObject.tag == "Hole")
        {
            ResetVelocityAndPosition();
            //game.RpcNextHole();
        }
    }

    public void ResetPosition(GameObject player, Vector3 LRTS)
    {
        player.transform.localPosition = LRTS;
    }

    void OnCollisionEnter(Collision col)
    {
        float magnitude = Vector3.Project(GetComponent<Rigidbody>().velocity, col.contacts[0].normal).sqrMagnitude;

        RaycastHit hit;

        string Tag;
        int Layer;

        if(IsNormalTo(col, out hit))
        {
            Tag = hit.collider.gameObject.tag;
            Layer = hit.collider.gameObject.layer;
        }
        else if (IsGrounded(out hit))
        {
            Tag = hit.collider.gameObject.tag;
            Layer = hit.collider.gameObject.layer;
        }
        else
        {
            Tag = col.collider.gameObject.tag;
            Layer = col.collider.gameObject.layer;
        }

        if(col.gameObject.layer == 9 && magnitude < 5f)
        {
            magnitude = 5f;
        }

        if(magnitude < 1f)
        {
            magnitude = 1f;
        }

        if (Tag == "Carpet")
        {
            if (AirTimeLastFrame >= 0.1f)
            {
                BounceCarpet.pitch = Random.Range(0.95f, 1.05f);
                BounceCarpet.volume = Mathf.Clamp01(magnitude / SpeedSoundLimit);
                BounceCarpet.Play();
            }
        }
        if (Tag == "Brick")
        {
            BounceBrick.pitch = Random.Range(0.9f, 1.1f);
            print(magnitude);
            BounceBrick.volume = Mathf.Clamp01(magnitude / SpeedSoundLimit);
            BounceBrick.Play();
        }
        if (Tag == "Concrete")
        {
            BounceConcrete.pitch = Random.Range(0.9f, 1.1f);
            BounceConcrete.volume = Mathf.Clamp01(magnitude / SpeedSoundLimit);
            BounceConcrete.Play();           
        }
        if (Tag == "Concrete_Ramp")
        {
            BounceConcrete.pitch = Random.Range(0.9f, 1.1f);
            BounceConcrete.volume = Mathf.Clamp01(magnitude / SpeedSoundLimit);
            BounceConcrete.Play();
            
        }
        if (Tag == "Wood")
        {
            BounceWood.pitch = Random.Range(0.7f, 1.3f);
            BounceWood.volume = Mathf.Clamp01(magnitude / SpeedSoundLimit);
            BounceWood.Play();
        }
        if (Tag == "Log")
        {
            print(magnitude);
            BounceLog.pitch = Random.Range(0.6f, 1.4f);
            BounceLog.volume = Mathf.Clamp01(magnitude / SpeedSoundLimit);
            BounceLog.Play();
        }
        if (Tag == "OoB")
        {
            if(GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
            {
                ResetVelocityAndPosition();
                ResetPosition(gameObject, LastPositionRTS);
            }
        }
        else if (Tag == "Untagged")
        {
            BounceConcrete.pitch = Random.Range(0.9f, 1.1f);
            BounceConcrete.volume = Mathf.Clamp01(GetComponent<Rigidbody>().velocity.magnitude / SpeedSoundLimit);
            BounceConcrete.Play();         
        }

        if(Layer == 8) //reset layer
        {
            if (GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
            {
                ResetVelocityAndPosition();
                ResetPosition(gameObject, LastPositionRTS);
            }
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.collider.gameObject.tag == "OoB")
        {
            if (GetComponent<Rigidbody>().velocity.magnitude < 2)
            {
                ResetVelocityAndPosition();
                ResetPosition(gameObject, LastPositionRTS);
            }
        }
        if (col.collider.gameObject.layer == 8) //reset layer
        {
            if (GetComponent<Rigidbody>().velocity.magnitude < 2)
            {
                ResetVelocityAndPosition();
                ResetPosition(gameObject, LastPositionRTS);
            }
        }
    }

    void ResetVelocityAndPosition()
    {
        transform.localPosition = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    bool IsNormalTo(Collision col, out RaycastHit hit)
    {
        Debug.DrawRay(transform.position, -col.contacts[0].normal, Color.blue, 10f);
        return Physics.Raycast(new Ray(transform.position, -col.contacts[0].normal), out hit, 0.2f);
    }

    bool IsStationary()
    {
        return (GetComponent<Rigidbody>().velocity.magnitude <= 0.01f);
    }

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, -Vector3.up * 0.1f, Color.red, 10f);
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    bool IsGrounded(out RaycastHit hit)
    {
        Debug.DrawRay(transform.position, -Vector3.up * 0.1f, Color.red, 10f);
        return Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 0.1f);
    }
}
