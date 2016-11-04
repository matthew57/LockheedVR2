using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClippingPlane : NetworkBehaviour
{

    private Vector3 ClipNormal;
    private float a = 0.0f;
    private float b = 0.0f;
    private float c = 0.0f;
    private float d = 0.0f;
    private float clip = 1.0f;

    private bool rotatePlane = false;
    private bool resetPlane = false;

    public Plane plane;

    // Use this for initialization
    void Start()
    {

        Shader.SetGlobalFloat("_a", a);
        Shader.SetGlobalFloat("_b", b);
        Shader.SetGlobalFloat("_c", c);
        Shader.SetGlobalFloat("_d", d);
        Shader.SetGlobalFloat("_clip", 0.0f);
       

    }

    // Update is called once per frame
    void Update()
    {

        plane.SetNormalAndPosition(gameObject.transform.up, gameObject.transform.position);

        ClipNormal = gameObject.transform.up;
        a = ClipNormal.x;
        b = ClipNormal.y;
        c = ClipNormal.z;
        d = plane.GetDistanceToPoint(Vector3.zero);

        Shader.SetGlobalFloat("_a", a);
        Shader.SetGlobalFloat("_b", b);
        Shader.SetGlobalFloat("_c", c);
        Shader.SetGlobalFloat("_d", d);

    }

    void ManipulatePlane(string action)
    {
        switch (action)
        {
            case "reset":

                resetPlane = !resetPlane;

                if (resetPlane)
                {
                    //Set Main Camera as parent to Clipping Plane
                    transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;

                    //Set Clipping Plane at 1.5 meters from front of camera
                    transform.localPosition = Vector3.forward * 2.0f;
                    //gameObject.GetComponent<HoloToolkit.Billboard>().enabled = true;
                    //transform.localRotation = Quaternion.identity;
                }
                else
                {
                    //Set Clipping Plane parent to null
                    transform.parent = null;
                    //gameObject.GetComponent<HoloToolkit.Billboard>().enabled = false;
                }

                break;

            case "rotate":

                rotatePlane = !rotatePlane;
                //gameObject.GetComponent<HoloToolkit.Billboard>().enabled = rotatePlane;

                break;

        }
    }
}
