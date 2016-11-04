// ===================== DOCUMENTATION ======================= //
/* Drag this script onto the Model Manager gameobject in the scene. It is here
 * where you will simply drag your desired model assembly into the 
 * Cad Model slot on this script. Take note that you must also drag your 
 * model assembly into the scene as a gameobject. Also take note that 
 * many other scripts rely on this one, as they refer to the model from 
 * this script on this Model Manager gameobject. This script cycles through
 * each part of the model assembly and adds various components to make them
 * all interactive and synced over a network setting. */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ModelManager : NetworkBehaviour {

    public GameObject cadModel;
    public bool resetPos;

    public List<NetworkIdentity> movedObjects = new List<NetworkIdentity>();

    // Use this for initialization
    void Awake ()
    {
        if (cadModel != null)
        {
            AddComponentsHandler();
        }
        else
        {
            Debug.Log("cad model is null");
        }
	}

    void Start()
    {
        var allChildren = cadModel.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            var cNetTrans = child.gameObject.GetComponent<LocalPNetworkTransform>();
            if (cNetTrans)
            {
                cNetTrans.enabled = false;
            }
        }
    }

    void AddComponentsHandler()
    {
        var allChildren = cadModel.GetComponentsInChildren<Transform>();

        var pTransform = cadModel.GetComponent<Transform>();


        //UnityEditor.PrefabUtility.CreatePrefab
        foreach (Transform child in allChildren)
        {
            if (child == pTransform)
            {
                child.gameObject.AddComponent<NetworkIdentity>();
                child.gameObject.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
                child.gameObject.AddComponent<NetworkTransform>();
                child.gameObject.GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
                setShader(child.gameObject);
            }
            else
            {
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                child.gameObject.AddComponent<MeshCollider>();
                child.gameObject.AddComponent<NetworkIdentity>();
                child.gameObject.AddComponent<SnapBack>();
                child.gameObject.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
                child.gameObject.AddComponent<LocalPNetworkTransform>();
                child.gameObject.GetComponent<LocalPNetworkTransform>().transformSyncMode = LocalPNetworkTransform.TransformSyncMode.SyncTransform;
                //disable comp and enable on pickup

                setShader(child.gameObject);
            }

        }
        Destroy(cadModel.GetComponent<SnapBack>());
    }

    private void setShader(GameObject obj)
    {
        if (obj.GetComponent<Renderer>())
        {
            foreach (Material mat in obj.GetComponent<Renderer>().sharedMaterials)
            {
                mat.shader = Shader.Find("Custom/ClipShader_Color");
            }
        }
    }
}
