using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))] 

public class IKControl : MonoBehaviour {

	protected Animator animator;

	public bool ikActive = false;
	public Transform rightHandObj = null;
	public Transform lookObj = null;

	public GameObject leftHand;
	public GameObject rightHand;
	public GameObject cameraHead;

	void Start () 
	{
		animator = GetComponent<Animator>();

	}

	//a callback for calculating IK
	void OnAnimatorIK()
	{
		if(animator) {

			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) {

				gameObject.transform.position = cameraHead.transform.position + new Vector3(0,-.6f,0);

				Quaternion tempQuat = cameraHead.transform.rotation;
				tempQuat.x = 0;
				tempQuat.z = 0;
				gameObject.transform.rotation = tempQuat;
				// Set the look target position, if one has been assigned
				if(lookObj != null) {
					animator.SetLookAtWeight(1);
					animator.SetLookAtPosition(lookObj.position);
				}    

				// Set the right hand target position and rotation, if one has been assigned
				if(rightHand != null) {
					animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
					animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
					animator.SetIKPosition(AvatarIKGoal.RightHand,rightHand.transform.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand,rightHand.transform.rotation);
				}     

				// Set the right hand target position and rotation, if one has been assigned
				if(leftHand != null) {
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
					animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHand.transform.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHand.transform.rotation);
				}        


			}

			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			else {          
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
				animator.SetLookAtWeight(0);
			}
		}
	}    
}
