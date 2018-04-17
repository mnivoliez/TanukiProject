using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

	[HideInInspector]
	public PathPlatform path;
	[HideInInspector]
	public Vector3 startPos;

	[Header("Display")]
	public Color anchorColor = Color.black;
	public Color controlColor = Color.white;
	public Color segmentColor = Color.red;
	public Color selectedSegmentColor = Color.yellow;
	public float anchorDiameter = .1f;
	public float controlDiamater = .075f;
	public bool displayControlPoints = true;
	public bool displayTranslateHandles = false;
	
	[Header("Selection")]
	[Tooltip("Aligned:\tHandles are aligned along anchor\nAuto:\tHandles are mirrored along anchor\nFree:\tHandles are independent")]
	public PathPlatform.PathSelection selection = PathPlatform.PathSelection.Aligned;

	public void Start() {
		startPos = transform.position;
	}

	public void CreatePath() {
		path = new PathPlatform(Vector3.zero);
	}

	void Reset() {
		CreatePath();
	}
}
