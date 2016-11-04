using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ArcTeleporter))]
public class ArcTeleporterEditor : Editor
{
	// target component
	public ArcTeleporter teleporter = null;
	SerializedObject serializedTeleporter;
	static bool raycastLayerFoldout = false;
	int raycastLayersSize = 0;
	static bool tagsFoldout = false;
	int tagsSize = 0;

	public void OnEnable()
	{
		teleporter = (ArcTeleporter)target;
		serializedTeleporter = new SerializedObject(teleporter);
		if (teleporter.raycastLayer != null)
			raycastLayersSize = teleporter.raycastLayer.Count;
		else raycastLayersSize = 0;
		if (teleporter.tags != null)
			tagsSize = teleporter.tags.Count;
		else tagsSize = 0;
	}

	public override void OnInspectorGUI()
	{
		if (serializedTeleporter == null) serializedTeleporter = new SerializedObject(teleporter);

		serializedTeleporter.Update();

		SerializedProperty controlScheme = serializedTeleporter.FindProperty("controlScheme");
		controlScheme.intValue = (int)(ArcTeleporter.ControlScheme)EditorGUILayout.EnumPopup("Control Scheme", (ArcTeleporter.ControlScheme)controlScheme.intValue);
		ArcTeleporter.ControlScheme controlSchemeEnum = (ArcTeleporter.ControlScheme)controlScheme.intValue;

		EditorGUI.indentLevel++;

		SerializedProperty teleportButton = serializedTeleporter.FindProperty("teleportButton");
		teleportButton.intValue = (int)(ArcTeleporter.VRButtons)EditorGUILayout.EnumPopup("Teleport Button", (ArcTeleporter.VRButtons)teleportButton.intValue);
		if (controlSchemeEnum == ArcTeleporter.ControlScheme.TWO_BUTTON_MODE)
		{
			SerializedProperty showTeleportButton = serializedTeleporter.FindProperty("showTeleportButton");
			showTeleportButton.intValue = (int)(ArcTeleporter.VRButtons)EditorGUILayout.EnumPopup("Show Teleport Button", (ArcTeleporter.VRButtons)showTeleportButton.intValue);
			if (teleportButton.intValue == showTeleportButton.intValue)
				EditorGUILayout.HelpBox("", MessageType.Warning);
		}

		EditorGUI.indentLevel--;

		SerializedProperty transition = serializedTeleporter.FindProperty("transition");
		transition.intValue = (int)(ArcTeleporter.Transition)EditorGUILayout.EnumPopup("Transition", (ArcTeleporter.Transition)transition.intValue);
		ArcTeleporter.Transition transitionEnum = (ArcTeleporter.Transition)transition.intValue;
		EditorGUI.indentLevel++;
		switch(transitionEnum)
		{
		case ArcTeleporter.Transition.FADE:
			SerializedProperty fadeMat = serializedTeleporter.FindProperty("fadeMat");
			fadeMat.objectReferenceValue = EditorGUILayout.ObjectField("Fade Material", fadeMat.objectReferenceValue, typeof(Material), false);
			EditorGUILayout.HelpBox("Material should be using a transparent shader with a colour field. Use the ExampleFade material in the materials folder or make your own", MessageType.Info);
			SerializedProperty fadeDuration = serializedTeleporter.FindProperty("fadeDuration");
			fadeDuration.floatValue = EditorGUILayout.FloatField("Fade Duration", fadeDuration.floatValue);
			break;
		}
		EditorGUI.indentLevel--;

		SerializedProperty maxDistance = serializedTeleporter.FindProperty("maxDistance");
		maxDistance.floatValue = EditorGUILayout.FloatField("Max Distance", maxDistance.floatValue);

		SerializedProperty disablePreMadeControls = serializedTeleporter.FindProperty("disablePreMadeControls");
		disablePreMadeControls.boolValue = EditorGUILayout.Toggle("Disable Pre Made Controls", disablePreMadeControls.boolValue);

		SerializedProperty firingMode = serializedTeleporter.FindProperty("firingMode");
		firingMode.intValue = (int)(ArcTeleporter.FiringMode)EditorGUILayout.EnumPopup("Firing Mode", (ArcTeleporter.FiringMode)firingMode.intValue);
		ArcTeleporter.FiringMode firingModeEnum = (ArcTeleporter.FiringMode)firingMode.intValue;

		EditorGUI.indentLevel++;

		switch(firingModeEnum)
		{
		case ArcTeleporter.FiringMode.ARC:
			SerializedProperty arcLineWidth = serializedTeleporter.FindProperty("arcLineWidth");
			arcLineWidth.floatValue = EditorGUILayout.FloatField("Arc Width", arcLineWidth.floatValue);

			SerializedProperty arcMat = serializedTeleporter.FindProperty("arcMat");
			arcMat.intValue = (int)(ArcTeleporter.ArcMaterial)EditorGUILayout.EnumPopup("Use Material", (ArcTeleporter.ArcMaterial)arcMat.intValue);
			ArcTeleporter.ArcMaterial arcMatEnum = (ArcTeleporter.ArcMaterial)arcMat.intValue;

			if (arcMatEnum == ArcTeleporter.ArcMaterial.MATERIAL)
			{
				SerializedProperty goodTeleMat = serializedTeleporter.FindProperty("goodTeleMat");
				goodTeleMat.objectReferenceValue = EditorGUILayout.ObjectField("Good Material", goodTeleMat.objectReferenceValue, typeof(Material), false);

				SerializedProperty badTeleMat = serializedTeleporter.FindProperty("badTeleMat");
				badTeleMat.objectReferenceValue = EditorGUILayout.ObjectField("Bad Material", badTeleMat.objectReferenceValue, typeof(Material), false);

				SerializedProperty matScale = serializedTeleporter.FindProperty("matScale");
				matScale.floatValue = EditorGUILayout.FloatField("Material scale", matScale.floatValue);

				SerializedProperty texMovementSpeed = serializedTeleporter.FindProperty("texMovementSpeed");
				texMovementSpeed.vector2Value = EditorGUILayout.Vector2Field("Material Movement Speed", texMovementSpeed.vector2Value);
			} else
			{
				SerializedProperty goodSpotCol = serializedTeleporter.FindProperty("goodSpotCol");
				goodSpotCol.colorValue = EditorGUILayout.ColorField("Good Colour", goodSpotCol.colorValue);

				SerializedProperty badSpotCol = serializedTeleporter.FindProperty("badSpotCol");
				badSpotCol.colorValue = EditorGUILayout.ColorField("Bad Colour", badSpotCol.colorValue);
			}
			break;
		case ArcTeleporter.FiringMode.PROJECTILE:

			SerializedProperty teleportProjectile = serializedTeleporter.FindProperty("teleportProjectilePrefab");
			teleportProjectile.objectReferenceValue = EditorGUILayout.ObjectField("Teleport Projectile Prefab", teleportProjectile.objectReferenceValue, typeof(GameObject), false);
			EditorGUILayout.HelpBox("Projectile prefab should have a rigidbody attached", MessageType.Info);
			break;
		}

		EditorGUI.indentLevel--;

		SerializedProperty disableRoomRotationWithTrackpad = serializedTeleporter.FindProperty("disableRoomRotationWithTrackpad");
		disableRoomRotationWithTrackpad.boolValue = EditorGUILayout.Toggle("Diable Room Rotation With Trackpad", disableRoomRotationWithTrackpad.boolValue);

		SerializedProperty teleportHighlight = serializedTeleporter.FindProperty("teleportHighlight");
		teleportHighlight.objectReferenceValue = EditorGUILayout.ObjectField("Teleport Highlight", teleportHighlight.objectReferenceValue, typeof(GameObject), false);

		SerializedProperty roomShape = serializedTeleporter.FindProperty("roomShape");
		roomShape.objectReferenceValue = EditorGUILayout.ObjectField("Room Highlight", roomShape.objectReferenceValue, typeof(GameObject), false);

		SerializedProperty onlyLandOnFlat = serializedTeleporter.FindProperty("onlyLandOnFlat");
		onlyLandOnFlat.boolValue = EditorGUILayout.Toggle("Only land on flat", onlyLandOnFlat.boolValue);
		if (onlyLandOnFlat.boolValue)
		{
			SerializedProperty slopeLimit = serializedTeleporter.FindProperty("slopeLimit");
			slopeLimit.floatValue = EditorGUILayout.FloatField("Slope limit", slopeLimit.floatValue);
		}

		SerializedProperty onlyLandOnTag = serializedTeleporter.FindProperty("onlyLandOnTag");
		onlyLandOnTag.boolValue = EditorGUILayout.Toggle("Only land on tagged", onlyLandOnTag.boolValue);

		if (onlyLandOnTag.boolValue)
		{
			tagsFoldout = EditorGUILayout.Foldout(tagsFoldout, "Tags");
			if (tagsFoldout)
			{
				EditorGUI.indentLevel++;
				tagsSize = EditorGUILayout.IntField("Size", tagsSize);

				SerializedProperty tags = serializedTeleporter.FindProperty("tags");
				if (tagsSize != tags.arraySize) tags.arraySize = tagsSize;

				for (int i=0 ; i<tagsSize ; i++)
				{
					SerializedProperty tagName = tags.GetArrayElementAtIndex(i);
					tagName.stringValue = EditorGUILayout.TextField("Element "+i, tagName.stringValue);
				}
				EditorGUI.indentLevel--;
			}
		}

		raycastLayerFoldout = EditorGUILayout.Foldout(raycastLayerFoldout, "Raycast Layers");
		if (raycastLayerFoldout)
		{
			EditorGUI.indentLevel++;
			raycastLayersSize = EditorGUILayout.IntField("Size", raycastLayersSize);

			SerializedProperty raycastLayers = serializedTeleporter.FindProperty("raycastLayer");
			if (raycastLayersSize != raycastLayers.arraySize) raycastLayers.arraySize = raycastLayersSize;
			for(int i=0 ; i<raycastLayersSize ; i++)
			{
				SerializedProperty raycastLayerName = raycastLayers.GetArrayElementAtIndex(i);
				raycastLayerName.stringValue = EditorGUILayout.TextField("Element "+i, raycastLayerName.stringValue);
			}
			EditorGUILayout.HelpBox("Leave raycast layers empty to collide with everything", MessageType.Info);
			if (raycastLayers.arraySize > 0)
			{
				SerializedProperty ignoreRaycastLayers = serializedTeleporter.FindProperty("ignoreRaycastLayers");
				ignoreRaycastLayers.boolValue = EditorGUILayout.Toggle("Ignore raycast layers", ignoreRaycastLayers.boolValue);
				EditorGUILayout.HelpBox("Ignore raycast layers True: Ignore anything on the layers specified. False: Ignore anything on layers not specified", MessageType.Info);
			}
			EditorGUI.indentLevel--;
		}

		serializedTeleporter.ApplyModifiedProperties();
	}
}
