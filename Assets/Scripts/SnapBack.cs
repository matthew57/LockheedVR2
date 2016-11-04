// ===================== DOCUMENTATION ======================= //
/* This script is already dynamically applied to every part in the 
 * model assembly upon starting the game - this is done in the Model
 * Manager script. This script works in conjunction with the Grabbing 
 * script to create a part copy when a part is grabbed. The part copy 
 * has a blue outline around it, which allows you to know where the 
 * part fits in the assembly. You can grab a removed part, put it near
 * where it originally was in the model, you'll then see the blue
 * outline of the part appear, which then you can release the part and 
 * it will snap back into that original position on the model assembly. */

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SnapBack : NetworkBehaviour
{
    public GameObject objectCopy;
    public Vector3 originalPos;
    public  Quaternion originalRot;
    public Transform originalParent;

    private Mesh copyMesh;
    private MeshRenderer copyMeshRenderer;
    
    // Use this for initialization
    void Start()
    {      
        originalPos = gameObject.transform.localPosition;
        originalRot = gameObject.transform.localRotation;
        originalParent = gameObject.transform.parent;
    }

    public void makeCopy()
    {
        objectCopy = new GameObject("Copy");

        copyMesh = gameObject.GetComponent<MeshFilter>().mesh;

        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            mat.shader = Shader.Find("Standard");
        }

        objectCopy.AddComponent<MeshFilter>();
        objectCopy.AddComponent<MeshRenderer>();
        objectCopy.GetComponent<MeshFilter>().mesh = copyMesh;

        Material outlineColor = Resources.Load("GlowingCollision") as Material;
        Debug.Log(outlineColor);
        objectCopy.GetComponent<MeshRenderer>().material = outlineColor;
        objectCopy.GetComponent<MeshRenderer>().enabled = false;

        objectCopy.transform.SetParent(originalParent, false);
        objectCopy.transform.localPosition = originalPos;
        objectCopy.transform.localRotation = originalRot;
    }

    public void destroyCopy()
    {
        transform.SetParent(originalParent, false);
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;

        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            mat.shader = Shader.Find("Custom/ClipShader_Color");
        }

        if (GetComponent<MeshRenderer>() && GetComponent<MeshRenderer>().enabled == false)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        if (objectCopy)
        {
            Destroy(objectCopy);
        }
    }
}

