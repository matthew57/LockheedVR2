using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    protected Animator animator;

    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform eye = null;
   
    void Start()
    {
        animator = GetComponent<Animator>();
        eye = GameObject.Find("Camera (eye)").transform;
        leftHandObj = GameObject.Find("Controller (left)").transform;
        rightHandObj = GameObject.Find("Controller (right)").transform;
    }

    void Update()
    {
        if (eye != null)
        {

           this.transform.position = new Vector3(eye.transform.position.x, eye.transform.position.y-0.6f, eye.transform.position.z);
           this.transform.rotation = new Quaternion(0, eye.transform.rotation.y, 0, eye.transform.rotation.w);
        }
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    rightHandObj.transform.Rotate(0, 180, 180);
                    rightHandObj.transform.Translate(0, 0, 0.1f);

                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);

                    
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (leftHandObj != null)
                {
                    leftHandObj.transform.Rotate(0, 180, 180);
                    leftHandObj.transform.Translate(0, 0, 0.1f);

                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
