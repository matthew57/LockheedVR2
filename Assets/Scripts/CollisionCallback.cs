using UnityEngine;
using System.Collections;

public class CollisionCallback : MonoBehaviour {

	// Use this for initialization
    public GameObject lPlayer;


    public delegate void triggerEventHandler(object sender, Collider e);

    public event triggerEventHandler triggerEnter;
    public event triggerEventHandler triggerExit;

    public virtual void OnTriggerClicked(Collider e)
    {
        if (triggerEnter != null)
            triggerEnter(this, e);
    }

    public virtual void OnTriggerUnclicked(Collider e)
    {
        if (triggerEnter != null)
            triggerExit(this, e);
    }

    private void OnTriggerEnter(Collider collider)
    {

		//Debug.Log ("Collision call back");
        OnTriggerClicked(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        OnTriggerUnclicked(collider);
    }
}
