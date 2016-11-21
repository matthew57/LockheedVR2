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


	Quaternion rRotate;
	Quaternion lRotate;

	[Tooltip("this number defines how high off the body the head is put based on the head's rotation")]
	public float bodyOffsetAmount = .6f;
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
				Quaternion tempQuat = cameraHead.transform.rotation;
				tempQuat.x = 0;
				tempQuat.z = 0;
				// rotate the body to match the head
				gameObject.transform.rotation = tempQuat;

				// body will moved forward or back based on if the head is tilted up or down
				float amount = ((-1*(Vector3.Angle (cameraHead.transform.forward, Vector3.up) - 90))/300) ;

				// set the body position below the head
				gameObject.transform.position = cameraHead.transform.position + new Vector3(0,(-.7f - amount *bodyOffsetAmount) + .05f,0);

				//move the body that amount
				gameObject.transform.position += (gameObject.transform.forward * (amount + .05f));

				// Set the look target position, if one has been assigned / head will look at this object, should be bound in front of camera
				if(lookObj != null) {
					animator.SetLookAtWeight(1);
					animator.SetLookAtPosition(lookObj.position);
				
				}   

			   

				// Set the right hand target position and rotation, if one has been assigned
				if(rightHand != null) {
					animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
					animator.SetIKRotationWeight(AvatarIKGoal.RightHand,.8f);  
					animator.SetIKPosition (AvatarIKGoal.RightHand, rightHand.transform.position);// - rightHand.transform.rotation * (new Vector3 (0,-.04f,.1f)));
					rRotate = rightHand.transform.rotation;
					rRotate *= Quaternion.Euler (rightHand.transform.forward * 65);
					animator.SetIKRotation(AvatarIKGoal.RightHand,rRotate);
				}     

				// Set the right hand target position and rotation, if one has been assigned
				if(leftHand != null) {
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,.8f);  
					animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHand.transform.position);// - leftHand.transform.rotation * (new Vector3 (0,-.04f,.1f)));
					lRotate = leftHand.transform.rotation;
					//lRotate *= Quaternion.Euler (leftHand.transform.right * 65);
					animator.SetIKRotation(AvatarIKGoal.LeftHand,lRotate);
				}   

				RaycastHit rayHit;

				Vector3 floorLocation = transform.position;
				if (Physics.Raycast (transform.position, Vector3.down,  out  rayHit, 20,1 << 9)) {

					floorLocation = rayHit.point + Vector3.up * .1f;
				}


			// RIGHT FOOT POSITION
					animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
					animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);  
					animator.SetIKPosition(AvatarIKGoal.RightFoot, floorLocation + this.transform.rotation *Vector3.right * .1f);
					animator.SetIKRotation(AvatarIKGoal.RightFoot,tempQuat);
			  


			// LEFT FOOT POSITION
					animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1);  
					animator.SetIKPosition(AvatarIKGoal.LeftFoot, floorLocation + this.transform.rotation *Vector3.left * .1f);
					animator.SetIKRotation(AvatarIKGoal.LeftFoot,tempQuat);



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
