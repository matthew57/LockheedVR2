using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class newArcTeleporter : MonoBehaviour {
public enum ArcMaterial
{
	MATERIAL,
	COLOUR
}
public enum ControlScheme
{
	TWO_BUTTON_MODE,
	PRESS_AND_RELEASE
}
public enum VRButtons
{
	TRIGGER,
	PAD,
	GRIP,
	MENU
}
public enum Transition
{
	INSTANT,
	FADE
}
public enum FiringMode
{
	ARC,
	PROJECTILE
}

public ControlScheme controlScheme = ControlScheme.TWO_BUTTON_MODE;
public FiringMode firingMode = FiringMode.ARC;
public VRButtons teleportButton = VRButtons.TRIGGER;
public VRButtons showTeleportButton = VRButtons.PAD;

public Transition transition;
public Material fadeMat;
public float fadeDuration = 0.5f;
public GameObject teleportProjectilePrefab;

public bool onlyLandOnFlat = true;
//Anywhere flat slot limit
public float slopeLimit = 20;
public bool onlyLandOnTag = false;
//Tags object has to have to be valid
public List<string> tags = new List<string>();
//Approximate max distance in unity units
public float maxDistance = 5f;
public bool disablePreMadeControls = false;
public ArcMaterial arcMat = ArcMaterial.MATERIAL;
// Material for the arc leave null to use colour
public Material goodTeleMat;
public Material badTeleMat;
// if material used the scale the texture should use
public float matScale = 5;
//	texture animated speed
public Vector2 texMovementSpeed = new Vector2(-0.035f, 0);
//	arc colours for good and bad spots if no material is used
public Color goodSpotCol = new Color(0, 0.6f, 1f, 0.2f);
public Color badSpotCol = new Color(0.8f, 0, 0, 0.2f);
public float arcLineWidth = 0.05f;
//Leave empty to collide with everything
public List<string> raycastLayer = new List<string>();
//Collide only with selected layers or only ignore selected layers
public bool ignoreRaycastLayers = false;
public bool disableRoomRotationWithTrackpad;
//	Teleport and room highlight, leave blank to not show
public GameObject teleportHighlight;
public GameObject roomShape;

private MeshRenderer fadeQuad;
private GameObject projectileInstance;
private Color fadeColour;
private bool fading;
private GameObject _teleportHighlightInstance;
private GameObject _roomShapeInstance;
private LayerMask raycastLayerMask;
private Transform _vrCamera;
private Transform _vrPlayArea;
private LineRenderer _lineRenderer;
private LineRenderer _lineRenderer2;
private Vector3 _teleportSpot;
private Quaternion _roomRotation;
Vector3 _roomPosition;
private Vector3 _destination;
private Vector3 _destinationNormal;
private Vector2 oldTrackpadAxis = Vector2.zero;
private bool _goodSpot;
private bool _teleportActive;
public bool teleportActive
{
	get { return _teleportActive; }
}

	public float HeightAboveGround;
void Start ()
{
	SteamVR_Camera steamVR_Camera = transform.parent.gameObject.GetComponentInChildren<SteamVR_Camera>();
	if (steamVR_Camera == null) _vrCamera = Camera.main.transform;
	else _vrCamera = steamVR_Camera.transform;
	if (_vrCamera == null)
	{
		Debug.LogError("ArcTeleporter can't find camera!");
		enabled = false;
		return;
	}
	SteamVR_PlayArea steamVR_PlayArea = transform.parent.gameObject.GetComponent<SteamVR_PlayArea>();
	if (steamVR_PlayArea == null) _vrPlayArea = transform.parent;
	else _vrPlayArea = steamVR_PlayArea.transform;
	if (_vrPlayArea == null)
	{
		Debug.LogError("ArcTeleporter must be a child of the steam vr play area attached to the controller tracked object");
		enabled = false;
		return;
	}

	SetControls();

	GameObject arcParentObject = new GameObject("ArcTeleporter");
	arcParentObject.transform.localScale = _vrPlayArea.localScale;
	GameObject arcLine1 = new GameObject("ArcLine1");
	arcLine1.transform.SetParent(arcParentObject.transform);
	_lineRenderer = arcLine1.AddComponent<LineRenderer>();
	GameObject arcLine2 = new GameObject("ArcLine2");
	arcLine2.transform.SetParent(arcParentObject.transform);
	_lineRenderer2 = arcLine2.AddComponent<LineRenderer>();
	_lineRenderer.SetWidth(arcLineWidth*_vrPlayArea.localScale.magnitude, arcLineWidth*_vrPlayArea.localScale.magnitude);
	_lineRenderer2.SetWidth(arcLineWidth*_vrPlayArea.localScale.magnitude, arcLineWidth*_vrPlayArea.localScale.magnitude);
	if (arcMat == ArcMaterial.COLOUR || goodTeleMat == null)
	{
		_lineRenderer.SetColors(goodSpotCol, goodSpotCol);
		_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		_lineRenderer2.SetColors(goodSpotCol, goodSpotCol);
		_lineRenderer2.material = new Material(Shader.Find("Sprites/Default"));
	} else
	{
		_lineRenderer.material = goodTeleMat;
		_lineRenderer2.material = goodTeleMat;
	}
	_lineRenderer.enabled = false;
	_lineRenderer2.enabled = false;

	if (teleportHighlight != null)
		_teleportHighlightInstance = (GameObject)Instantiate(teleportHighlight, Vector3.zero, Quaternion.identity);
	else
		_teleportHighlightInstance = new GameObject("TeleportHighlight");
	_teleportHighlightInstance.transform.SetParent(arcParentObject.transform);
	_teleportHighlightInstance.transform.localScale = Vector3.one;
	_teleportHighlightInstance.SetActive(false);
	if (roomShape != null)
		_roomShapeInstance = (GameObject)Instantiate(roomShape, Vector3.zero, Quaternion.identity);
	else
		_roomShapeInstance = new GameObject("TeleportRoom");
	_roomShapeInstance.transform.SetParent(arcParentObject.transform);
	_roomShapeInstance.transform.rotation = _vrPlayArea.rotation;
	_roomShapeInstance.transform.localScale = Vector3.one;
	_roomShapeInstance.SetActive(false);
}

void Update()
{
	if (!teleportActive) return;
	switch(firingMode)
	{
	case FiringMode.ARC:
		CalculateArc();
		break;
	case FiringMode.PROJECTILE:
		CheckProjectilePoint();
		break;
	}

	if (_teleportHighlightInstance != null && _teleportHighlightInstance.activeSelf)
	{
		_teleportHighlightInstance.transform.position = _destination+(_destinationNormal*0.05f);
		if (_destinationNormal == Vector3.zero)
			_teleportHighlightInstance.transform.rotation = Quaternion.identity;
		else 
			_teleportHighlightInstance.transform.rotation = Quaternion.LookRotation(_destinationNormal);
	}
	if (_roomShapeInstance != null && _roomShapeInstance.activeSelf && _teleportHighlightInstance != null)
	{
		Vector3 camSpot = new Vector3(_vrCamera.position.x, 0, _vrCamera.position.z);
		Vector3 roomSpot = new Vector3(_vrPlayArea.position.x, 0, _vrPlayArea.position.z);
		float angle = Quaternion.Angle(_vrPlayArea.rotation, _roomShapeInstance.transform.rotation);
		Vector3 cross = Vector3.Cross(_vrPlayArea.rotation * Vector3.forward, _roomShapeInstance.transform.rotation * Vector3.forward);
		if (cross.y < 0) angle = -angle;
		Quaternion difference = Quaternion.AngleAxis(angle, Vector3.up);
		Vector3 offset = difference * (roomSpot - camSpot);
			_roomShapeInstance.transform.position = (_destination + offset)+_destinationNormal*0.05f + Vector3.up * HeightAboveGround;
		if (!disableRoomRotationWithTrackpad)
		{
			ImprovedController controller = GetComponent<ImprovedController>();
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)controller.controllerIndex);
			Vector2 trackpadAxis = device.GetAxis();
			if (trackpadAxis != Vector2.zero && oldTrackpadAxis != Vector2.zero)
			{
				float diff = (trackpadAxis.x-oldTrackpadAxis.x)*100;
				_roomShapeInstance.transform.RotateAround(_destination, Vector3.up, diff);
			}
			oldTrackpadAxis = trackpadAxis;
		}
	}

}




//	Overide and change to suit your custom needs
virtual protected void SetControls()
{
	if (!disablePreMadeControls)
	{
		if (GetComponent<ImprovedController>() == null)
		{
			Debug.LogError("ArcTeleporter must be on a SteamVR_TrackedController");
			enabled = false;
			return;
		}
		ImprovedController trackController = GetComponent<ImprovedController>();
		if (teleportButton == VRButtons.TRIGGER || showTeleportButton == VRButtons.TRIGGER)
		{
			trackController.TriggerClicked += new ClickedEventHandler(TriggerClicked);
			trackController.TriggerUnclicked += new ClickedEventHandler(TriggerUnclicked);
		}
		if (teleportButton == VRButtons.PAD || showTeleportButton == VRButtons.PAD)
		{
			trackController.PadClicked += new ClickedEventHandler(PadClicked);
			trackController.PadUnclicked += new ClickedEventHandler(PadUnClicked);
		}
		if (teleportButton == VRButtons.GRIP || showTeleportButton == VRButtons.GRIP)
		{
			trackController.Gripped += new ClickedEventHandler(GripClicked);
			trackController.Ungripped += new ClickedEventHandler(GripUnclicked);
		}
		if (teleportButton == VRButtons.MENU || showTeleportButton == VRButtons.MENU)
		{
			trackController.MenuButtonClicked += new ClickedEventHandler(MenuClicked);
			trackController.MenuButtonUnclicked += new ClickedEventHandler(MenuUnClicked);
		}
	}
}

private void CheckProjectilePoint()
{
	if (!_teleportActive || projectileInstance == null) return;

	Ray ray = new Ray(projectileInstance.transform.position, Vector3.down);
	RaycastHit hit;
	//	Check if we hit something
	bool hitSomething = false;
	if (raycastLayer == null || raycastLayer.Count == 0)
		hitSomething = Physics.Raycast(ray, out hit, 1);
	else
	{
		LayerMask raycastLayerMask = 1 << LayerMask.NameToLayer(raycastLayer[0]);
		for(int j=1; j<raycastLayer.Count ; j++)
		{
			raycastLayerMask |= 1 << LayerMask.NameToLayer(raycastLayer[j]);
		}
		if (ignoreRaycastLayers) raycastLayerMask = ~raycastLayerMask;
		hitSomething = Physics.Raycast(ray, out hit, 1, raycastLayerMask);
	}
	_goodSpot = false;
	if (hitSomething)
	{
		_goodSpot = IsGoodSpot(hit);
		_destination = hit.point;
		if (_goodSpot)
		{
			if (!fading) _teleportSpot = hit.point;
			if (_roomShapeInstance != null)
			{
				_roomShapeInstance.SetActive(true);
			}
		}
		if (_teleportHighlightInstance != null) _teleportHighlightInstance.SetActive(true);
		_destinationNormal = hit.normal;
	}
	if (!hitSomething) _teleportHighlightInstance.SetActive(false);
	if (!_goodSpot) _roomShapeInstance.SetActive(false);
}

private void CalculateArc()
{
	//	Line renderer position storage (two because line renderer texture will stretch if one is used)
	List<Vector3> positions1 = new List<Vector3>();
	List<Vector3> positions2 = new List<Vector3>();

	//	Variables need for curve
	Quaternion currentRotation = transform.rotation;
	Vector3 currentPosition = transform.position;
	positions1.Add(currentPosition);
	Vector3 lastPostion = transform.position-transform.forward;
	Vector3 currentDirection = transform.forward;
	Vector3 downForward = new Vector3(transform.forward.x*0.01f, -1, transform.forward.z*0.01f);
	RaycastHit hit = new RaycastHit();
	float totalDistance1 = 0;
	float totalDistance2 = 0;

	//	first Vector3 positions array will be used for the curve and the second line renderer is used for the straight down after the curve
	bool useFirstArray = true;

	//	Should never come close to 500 iterations but just as safety to avoid indefinite looping
	int i=0;
	while(i<500)
	{
		i++;

		//	Rotate the current rotation toward the downForward rotation
		Quaternion downQuat = Quaternion.LookRotation(downForward);
		currentRotation = Quaternion.RotateTowards(currentRotation, downQuat, 1f);

		//	Make ray for new direction
		Ray newRay = new Ray(currentPosition, currentPosition-lastPostion);
		float length = (maxDistance*0.01f)*_vrPlayArea.localScale.magnitude;
		float raycastLength = length*1.1f;
		if (currentRotation == downQuat)
		{
			//We have finished the arc and are facing down
			//So were going to use the second line renderer and extend the normal length as a last effort to hit something
			useFirstArray = false;
			length = (maxDistance*matScale)*_vrPlayArea.localScale.magnitude;
			positions2.Add(currentPosition);
		}

		//	Check if we hit something
		bool hitSomething = false;
		if (raycastLayer == null || raycastLayer.Count == 0)
			hitSomething = Physics.Raycast(newRay, out hit, raycastLength);
		else
		{
			LayerMask raycastLayerMask = 1 << LayerMask.NameToLayer(raycastLayer[0]);
			for(int j=1; j<raycastLayer.Count ; j++)
			{
				raycastLayerMask |= 1 << LayerMask.NameToLayer(raycastLayer[j]);
			}
			if (ignoreRaycastLayers) raycastLayerMask = ~raycastLayerMask;
			hitSomething = Physics.Raycast(newRay, out hit, raycastLength, raycastLayerMask);
		}

		if (hitSomething)
		{
			//	Depending on whether we had switched to the first or second line renderer
			//	add the point and finish calculating the total distance
			if (useFirstArray)
			{
				totalDistance1 += (currentPosition-hit.point).magnitude;
				positions1.Add(hit.point);
			} else
			{
				totalDistance2 += (currentPosition-hit.point).magnitude;
				positions2.Add(hit.point);
			}
			_destinationNormal = hit.normal;
			//	And we're done
			break;
		}

		//	Convert the rotation to a forward vector and apply to our current position
		currentDirection = currentRotation * Vector3.forward;
		lastPostion = currentPosition;
		currentPosition += currentDirection*length;

		//	Depending on whether we have switched to the second line renderer add this point and update total distance
		if (useFirstArray)
		{
			totalDistance1 += length;
			positions1.Add(currentPosition);
		} else
		{
			totalDistance2 += length;
			positions2.Add(currentPosition);
		}

		//	If we're pointing down then we did the whole arc and down without hitting anything so we're done
		if (currentRotation == downQuat) break;
	}

	//Vector3 teleportSpot = Vector3.zero;
	if (useFirstArray)
	{
		_lineRenderer2.enabled = false;
		_destination = positions1[positions1.Count-1];
	} else
	{
		_lineRenderer2.enabled = true;
		_destination = positions2[positions2.Count-1];
	}

	//	Decide using the current teleport rule whether this is a good teleporting spot or not
	_goodSpot = IsGoodSpot(hit);

	//	Update line, teleport highlight and room highlight based on it being a good spot or bad
	if (_goodSpot)
	{
		//	Toggle the second line renderer and get the last position from the appropirate array for our destination
		if (useFirstArray)
		{
			if (!fading) _teleportSpot = _destination;
		} else
		{
			if (!fading) _teleportSpot = _destination;
		}

		if (arcMat == ArcMaterial.COLOUR || goodTeleMat == null)
		{
			_lineRenderer.SetColors(goodSpotCol, goodSpotCol);
			_lineRenderer2.SetColors(goodSpotCol, goodSpotCol);
		} else
		{
			if (_lineRenderer.material.mainTexture.name != goodTeleMat.mainTexture.name) _lineRenderer.material = goodTeleMat;
			if (_lineRenderer2.material.mainTexture.name != goodTeleMat.mainTexture.name) _lineRenderer2.material = goodTeleMat;
		}
		if (_roomShapeInstance != null)
		{
			_roomShapeInstance.SetActive(true);
		}
	} else
	{
		if (arcMat == ArcMaterial.COLOUR || badTeleMat == null)
		{
			_lineRenderer.SetColors(badSpotCol, badSpotCol);
			_lineRenderer2.SetColors(badSpotCol, badSpotCol);
		} else
		{
			if (_lineRenderer.material.mainTexture.name != badTeleMat.mainTexture.name) _lineRenderer.material = badTeleMat;
			if (_lineRenderer2.material.mainTexture.name != badTeleMat.mainTexture.name) _lineRenderer2.material = badTeleMat;
		}
		if (_roomShapeInstance != null) _roomShapeInstance.SetActive(false);
	}

	_lineRenderer.SetVertexCount(positions1.Count);
	_lineRenderer.SetPositions(positions1.ToArray());
	_lineRenderer.material.mainTextureScale = new Vector2((totalDistance1*matScale)/_vrPlayArea.localScale.magnitude, 1);
	_lineRenderer.material.mainTextureOffset = new Vector2(_lineRenderer.material.mainTextureOffset.x+texMovementSpeed.x, _lineRenderer.material.mainTextureOffset.y+texMovementSpeed.y);

	if (_lineRenderer2.enabled)
	{
		_lineRenderer2.SetVertexCount(positions2.Count);
		_lineRenderer2.SetPositions(positions2.ToArray());
		_lineRenderer2.material.mainTextureScale = new Vector2((totalDistance2*matScale)/_vrPlayArea.localScale.magnitude, 1);
		_lineRenderer2.material.mainTextureOffset = new Vector2(_lineRenderer2.material.mainTextureOffset.x+texMovementSpeed.x, _lineRenderer2.material.mainTextureOffset.y+texMovementSpeed.y);
	}
}

//	Overide and change to expand on what is a good landing spot
virtual protected bool IsGoodSpot(RaycastHit hit)
{
	if (hit.transform == null) return false;
	if (onlyLandOnFlat)
	{
		float angle = Vector3.Angle(Vector3.up, hit.normal);
		if (angle > slopeLimit)
			return false;
	}
	if (onlyLandOnTag)
	{
		foreach(string tag in tags)
		{
			if (hit.transform.tag == tag)
				return true;
		}
		return false;
	}
	return true;
}

void PadClicked(object sender, ClickedEventArgs e)
{
	ButtonClicked(VRButtons.PAD);
}

void PadUnClicked(object sender, ClickedEventArgs e)
{
	ButtonUnClicked(VRButtons.PAD);
}

void TriggerClicked(object sender, ClickedEventArgs e)
{
	ButtonClicked(VRButtons.TRIGGER);
}

void TriggerUnclicked(object sender, ClickedEventArgs e)
{
	ButtonUnClicked(VRButtons.TRIGGER);
}

void GripClicked(object sender, ClickedEventArgs e)
{
	ButtonClicked(VRButtons.GRIP);
}

void GripUnclicked(object sender, ClickedEventArgs e)
{
	ButtonUnClicked(VRButtons.GRIP);
}

void MenuClicked(object sender, ClickedEventArgs e)
{
	ButtonClicked(VRButtons.MENU);
}

void MenuUnClicked(object sender, ClickedEventArgs e)
{
	ButtonUnClicked(VRButtons.MENU);
}

virtual protected void ButtonClicked(VRButtons button)
{
	if (disablePreMadeControls) return;
	switch(controlScheme)
	{
	case ControlScheme.TWO_BUTTON_MODE:
		if (showTeleportButton == button)
			EnableTeleport();
		else if (teleportButton == button)
			Teleport();
		break;
	case ControlScheme.PRESS_AND_RELEASE:
		if (teleportButton == button)
			EnableTeleport();
		break;
	}
}

virtual protected void ButtonUnClicked(VRButtons button)
{
	if (disablePreMadeControls) return;
	switch(controlScheme)
	{
	case ControlScheme.TWO_BUTTON_MODE:
		if (showTeleportButton == button)
			DisableTeleport();
		break;
	case ControlScheme.PRESS_AND_RELEASE:
		if (teleportButton == button)
		{
			Teleport();
			DisableTeleport();
		}
		break;
	}
}

virtual public void EnableTeleport()
{
	_teleportActive = true;
	switch(firingMode)
	{
	case FiringMode.ARC:
		_lineRenderer.enabled = true;
		if (_teleportHighlightInstance != null) _teleportHighlightInstance.SetActive(true);
		if (_roomShapeInstance != null)
		{
			_roomShapeInstance.SetActive(true);
		}
		CalculateArc();
		break;
	case FiringMode.PROJECTILE:
		if (teleportProjectilePrefab == null)
		{
			Debug.LogError("Teleport projectile prefab is not set");
			return;
		}
		if (projectileInstance != null) DisableTeleport();
		projectileInstance = (GameObject)Instantiate(teleportProjectilePrefab, Vector3.zero, Quaternion.identity);
		projectileInstance.transform.position = transform.position+(transform.forward*0.5f);
		Rigidbody body = projectileInstance.GetComponentInChildren<Rigidbody>();
		if (body == null)
		{
			Debug.LogError("Teleport projectile has no rigidbody");
			return;
		}
		body.AddForce(transform.forward*(maxDistance*100));
		CheckProjectilePoint();
		break;
	}
}

virtual public void DisableTeleport()
{
	_teleportActive = false;
	switch(firingMode)
	{
	case FiringMode.ARC:
		_lineRenderer.enabled = false;
		_lineRenderer2.enabled = false;
		break;
	case FiringMode.PROJECTILE:
		Destroy(projectileInstance);
		break;
	}
	if (_teleportHighlightInstance != null)
		_teleportHighlightInstance.SetActive(false);
	if (_roomShapeInstance != null)
	{
		_roomShapeInstance.transform.rotation = _vrPlayArea.rotation;
		_roomShapeInstance.transform.position = _vrPlayArea.position;
		_roomShapeInstance.SetActive(false);
		oldTrackpadAxis = Vector2.zero;
	}
}

virtual public void Teleport()
{
	_roomRotation = _roomShapeInstance.transform.rotation;
	_roomPosition = _roomShapeInstance.transform.position;
	switch(firingMode)
	{
	case FiringMode.ARC:
		if (teleportActive && _goodSpot)
		{
			switch(transition)
			{
			case Transition.INSTANT:
				MoveToTarget(_teleportSpot);
				break;
			case Transition.FADE:
				if (fadeQuad == null) 
				{
					CreateFadeQuad();
					StartCoroutine(fadeTransition(_teleportSpot));
				}
				break;
			}

				foreach (IKControl ik in GameObject.FindObjectsOfType<IKControl>()) {
					ik.OnAnimatorIK ();
				}
		}
		break;
	case FiringMode.PROJECTILE:
		if (projectileInstance != null)
		{
			switch(transition)
			{
			case Transition.INSTANT:
				if (_goodSpot)
				{
					MoveToTarget(_teleportSpot);
						foreach (IKControl ik in GameObject.FindObjectsOfType<IKControl>()) {
							ik.OnAnimatorIK ();
						}
				}
				break;
			case Transition.FADE:
				if (_goodSpot && fadeQuad == null) 
				{
					CreateFadeQuad();
					StartCoroutine(fadeTransition(_teleportSpot));
						foreach (IKControl ik in GameObject.FindObjectsOfType<IKControl>()) {
							ik.OnAnimatorIK ();
						}
				}
				break;
			}
		}
		break;
	}


}

virtual protected void MoveToTarget(Vector3 target)
{
	_vrPlayArea.position = _roomPosition;
	_vrPlayArea.rotation = _roomRotation;
	if (_roomShapeInstance != null)
	{
		_roomShapeInstance.transform.rotation = _vrPlayArea.rotation;
		_roomShapeInstance.transform.position = _vrPlayArea.position;
		_roomShapeInstance.SetActive(false);
		oldTrackpadAxis = Vector2.zero;
	}
}

virtual protected void CreateFadeQuad()
{
	GameObject fadeQuadObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
	fadeQuadObj.transform.SetParent(_vrCamera);
	Vector3 fadeQuadPosition = _vrCamera.position + (_vrCamera.forward*(_vrPlayArea.localScale.magnitude*0.01f));
	Vector3 fadeQuadScale = Vector3.one;
	if (Camera.main.nearClipPlane != 0) 
	{
		fadeQuadPosition += _vrCamera.forward*Camera.main.nearClipPlane;
		if (Camera.main.nearClipPlane > 1)
			fadeQuadScale *= Camera.main.nearClipPlane*5f;
	}
	fadeQuadObj.transform.position = fadeQuadPosition;
	fadeQuadObj.transform.LookAt(fadeQuadObj.transform.position+_vrCamera.forward, _vrCamera.up);
	fadeQuadObj.transform.localScale = fadeQuadScale;
	Destroy(fadeQuadObj.GetComponent<Collider>());
	fadeQuad = fadeQuadObj.GetComponent<MeshRenderer>();
	fadeQuad.material = fadeMat;
	fadeQuad.receiveShadows = false;
	fadeQuad.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	fadeQuad.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
	fadeQuad.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
	fadeColour = fadeQuad.material.color;
}

virtual protected IEnumerator fadeTransition(Vector3 target)
{
	if (fadeQuad == null) yield break;
	fading = true;
	Color transparent = new Color(0, 0, 0, 0);

	fadeQuad.material.color = transparent;
	float t = 0;
	float startTime = Time.time;
	while(t < 1)
	{
		if (fadeQuad == null)
		{
			fading = false;
			yield break;
		}
		float currentTime = Time.time;
		float elapsedTime = currentTime - startTime;
		t = elapsedTime / (fadeDuration*0.5f);
		fadeQuad.material.color = Color.Lerp(transparent, fadeColour, t);
		yield return null;
	}

	MoveToTarget(target);

	startTime = Time.time;
	t = 0;
	while(t < 1)
	{
		if (fadeQuad == null)
		{
			fading = false;
			yield break;
		}
		float currentTime = Time.time;
		float elapsedTime = currentTime - startTime;
		t = elapsedTime / (fadeDuration*0.5f);
		fadeQuad.material.color = Color.Lerp(fadeColour, transparent, t);
		yield return null;
	}
	if (fadeQuad == null)
	{
		fading = false;
		yield break;
	}
	Destroy(fadeQuad.gameObject);
	fading = false;
}
}
