using UnityEngine;
using System.Collections;

public class handanimations : MonoBehaviour
{


	public enum HandPosition{Idle, Point, GrabSmall, GrabLarge }

    Animator anim;
	 int Idle = Animator.StringToHash("Idle");
    int Point = Animator.StringToHash("Point");
    int GrabLarge = Animator.StringToHash("GrabLarge");
    int GrabSmall = Animator.StringToHash("GrabSmall");
    int GrabStickUp = Animator.StringToHash("GrabStickUp");
    int GrabStickFront = Animator.StringToHash("GrabStickFront");
    int ThumbUp = Animator.StringToHash("ThumbUp");
    int Fist = Animator.StringToHash("Fist");
    int Gun = Animator.StringToHash("Gun");
    int GunShoot = Animator.StringToHash("GunShoot");
    int PushButton = Animator.StringToHash("PushButton");
    int Spread = Animator.StringToHash("Spread");
    int MiddleFinger = Animator.StringToHash("MiddleFinger");
    int Peace = Animator.StringToHash("Peace");
    int OK = Animator.StringToHash("OK");
	int Phone = Animator.StringToHash("Phone");
	int Rock = Animator.StringToHash("Rock");
	int Natural = Animator.StringToHash("Natural");
	int Number3 = Animator.StringToHash("Number3");
	int Number4 = Animator.StringToHash("Number4");
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

	public void SetHandAnim(HandPosition position)
	{
		switch (position) {

		case HandPosition.GrabLarge:
			anim.SetTrigger(GrabLarge);
			break;

		case HandPosition.GrabSmall:
			anim.SetTrigger(GrabSmall);
			break;

		case HandPosition.Idle:
			anim.SetTrigger(Idle);
			break;

		case HandPosition.Point:
			anim.SetTrigger(Point);
			break;
		}
	}


    void Start ()
    {
        anim = GetComponent<Animator>();
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
        /*
        device = SteamVR_Controller.Input((int) trackedObject.index);
        if(device == null)
        {
            print("Controller not initialized");
        }
        else if(device.GetPressDown(triggerButton))
        {
            anim.ResetTrigger(GrabLarge);
            print("I pressed the trigger");
        }
        else
			*/
			if (Input.GetKeyDown(KeyCode.Q))
        {
            print("I pressed Q");
            anim.SetTrigger(Idle);
        }

        else if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger(Point);
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger(GrabLarge);
        }

        else if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger(GrabSmall);
        }

        else if (Input.GetKeyDown(KeyCode.T))
        {
            anim.SetTrigger(GrabStickUp);
        }

        else if (Input.GetKeyDown(KeyCode.Y))
        {
            anim.SetTrigger(GrabStickFront);
        }

        else if (Input.GetKeyDown(KeyCode.U))
        {
            anim.SetTrigger(ThumbUp);
        }

        else if (Input.GetKeyDown(KeyCode.I))
        {
            anim.SetTrigger(Fist);
        }

        else if (Input.GetKeyDown(KeyCode.O))
        {
            anim.SetTrigger(Gun);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetTrigger(GunShoot);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger(PushButton);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger(Spread);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger(MiddleFinger);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger(Peace);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetTrigger(OK);
        }
		else if (Input.GetKeyDown(KeyCode.H))
        {
            anim.SetTrigger(Phone);
        }
		else if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger(Rock);
        }
		else if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetTrigger(Natural);
        }
		else if (Input.GetKeyDown(KeyCode.L))
        {
            anim.SetTrigger(Number3);
        }
		else if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger(Number4);
        }
    }




  
}