// ===================== DOCUMENTATION ======================= //
/* Drag this script onto the LightSaber gameobject. This script simply makes
 * it so when the lightsaber hits other objects in the scene, it erases
 * them by turning off their mesh renderer component. */

using UnityEngine;
using System.Collections;

public class LightSaberTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider collider)
    {
         collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerExit(Collider collider)
    {

    }
}
