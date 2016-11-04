//======= Copyright (c) Valve Corporation, All rights reserved. ===============

// ===================== DOCUMENTATION ======================= //
/* Drag this script onto the left or right hand of the VRPlayer prefab, depending
 * on which hand you'd like the laser pointer to be on. This script works in 
 * conjunction with the LaserPointer script. It is in this script that the 
 * actual laser pointer is created, the other LaserPointer script simply turns
 * it on/off based on a button click on the controller. */

using UnityEngine;
using System.Collections;

public struct PEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void PEventHandler(object sender, PEventArgs e);

public class VR_LaserPointer : MonoBehaviour
{
    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool addRigidBody = false;
    public Transform reference;
    public event PEventHandler PointerIn;
    public event PEventHandler PointerOut;

    Transform previousContact = null;


    void Start()
    {
        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if (collider)
            {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;

        this.transform.GetChild(0).gameObject.SetActive(false);

    }

    public virtual void OnPointerIn(PEventArgs e)
    {


        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(PEventArgs e)
    {

	
        if (PointerOut != null)
            PointerOut(this, e);
    }

    // Update is called once per frame
    void Update()
    {
        float dist = 100f;
        Ray raycast = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact != hit.transform)
        {
            PEventArgs args = new PEventArgs();

            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            PEventArgs argsIn = new PEventArgs();

            argsIn.distance = hit.distance;
            argsIn.flags = 0;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }
        if (!bHit)
        {
     
            previousContact = null;
        }
        if (bHit && hit.distance < 100f)
        {
       
            dist = hit.distance;
        }

        pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
    }
}
