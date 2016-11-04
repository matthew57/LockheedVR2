using UnityEngine;
using System.Collections;

public class HumanoidMover : MonoBehaviour {

    public Vector3 direction;

    private IVR.InstantVR humanoid;

	// Use this for initialization
	void Start () {
        humanoid = GetComponent<IVR.InstantVR>();
	}
	
	// Update is called once per frame
	void Update () {
        humanoid.Move(direction);
	}
}
