using UnityEngine;

[ExecuteInEditMode]
public class PlatformController : MonoBehaviour {

	public Vector3[] localWaypoints = new Vector3[2];

	public AnimationCurve curveX = new AnimationCurve();
	public AnimationCurve curveY = new AnimationCurve();
	public AnimationCurve curveZ = new AnimationCurve();

	public bool isCyclic;
	
	public bool isPlayingBackwards = false;
	
	public bool isMirror = false;
	public bool mirrorX = true;
	public bool mirrorY = false;
	public bool mirrorZ = true;

	[Range(0.1f, 10)] [Tooltip("Travel time between two waypoints in second")]
	public float waypointPeriod = 1f;

	[Range(0, 10)] [Tooltip("Seconds to wait between loops")]
	public float waitTime;

	public bool isHandlesDrawing = true;
	public bool isPathDrawing = true;
	public Color gizmosStartColor = Color.blue;
	public Color gizmosEndColor = Color.red;
	public float gizmosSize = 0.5f;

	private int fromWaypointIndex = 0;
	private float percentBetweenWaypoints = 0;
	private float nextMoveTime;
	private bool isReverse = false;

	[HideInInspector]
	public Vector3 startPos;

	public bool isRunning = true;
	private bool isInit = false;

	void Update() {
		if (Application.isPlaying) {
			if (isRunning)
				transform.position = CalculatePlatformMovement ();
			else {
				if (isInit) 
					return;
				transform.position = GetGlobalCurvesVector (0);
				isInit = true;
			}
		} else {
			startPos = transform.position;
			RemoveUnusedWaypoints ();
			AddWaypoints ();
			HandleMirror ();
			HandleBackwards ();
			UpdateWaypoints ();
			UpdateCyclic ();
		}
	}

	Vector3 CalculatePlatformMovement() {

		if (Time.time < nextMoveTime)
			return Vector3.zero;

		percentBetweenWaypoints += Time.deltaTime * (1f/waypointPeriod);

		float keyValue = fromWaypointIndex + (isReverse ? -percentBetweenWaypoints : percentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			fromWaypointIndex += (isReverse ? -1 : 1);
			
			percentBetweenWaypoints--;
			
			if (fromWaypointIndex >= localWaypoints.Length + ((isCyclic) ? 0 : -1)) {
				if (isCyclic) {
					fromWaypointIndex = 0;
				} else {
					isReverse = true;
				}
				UpdateWaitTime ();
			} else if (fromWaypointIndex <= 0) {
				isReverse = false;
				UpdateWaitTime ();
			}
		}

		return GetGlobalCurvesVector (keyValue);
	}

	void UpdateWaitTime() {
		nextMoveTime = Time.time + waitTime;
	}

	void RemoveUnusedWaypoints() {
		for (int i = localWaypoints.Length; i < curveX.length - (isCyclic?1:0); i++) {
			CurvesRemoveKey (i);
		}
	}

	void AddWaypoints () {
		for (int i = curveX.length; i < localWaypoints.Length; i++) {
			CurvesAddKey (i, localWaypoints[i]);
		}
	}
		
	void UpdateCyclic () {
		if (isCyclic) {
			CurvesAddKey (localWaypoints.Length, localWaypoints[0]);
			CurvesMoveKey (localWaypoints.Length, localWaypoints[0], 0);
		} else {
			if (curveX.length == localWaypoints.Length + 1) {
				CurvesRemoveKey (localWaypoints.Length);
			}
		}
	}

	void UpdateWaypoints() {
		for (int i = 0; i < localWaypoints.Length; i++) {
			CurvesMoveKey (i, localWaypoints[i]);
		}
	}

	void HandleBackwards () {
		if (!isPlayingBackwards)
			return;
		
		isPlayingBackwards = false;

		int length = localWaypoints.Length-1;

		int offset = 0;
		if(isCyclic) {
			CurvesMoveKey (0, length+1, -1);
			offset = 1;
		}
		for (int i = 0; i < (length+1)/2; i++) {
			Vector3 tmp = localWaypoints [i+offset];
			localWaypoints [i+offset] = localWaypoints [length - i];
			localWaypoints [length - i] = tmp;

			Keyframe tmpKeyX = curveX.keys [i+offset];
			Keyframe tmpKeyY = curveY.keys [i+offset];
			Keyframe tmpKeyZ = curveZ.keys [i+offset];

			CurvesMoveKey (i+offset, length-i, -1);

			curveX.MoveKey (length - i, new Keyframe(length - i, tmpKeyX.value, -tmpKeyX.inTangent, -tmpKeyX.outTangent));
			curveY.MoveKey (length - i, new Keyframe(length - i, tmpKeyY.value, -tmpKeyY.inTangent, -tmpKeyY.outTangent));
			curveZ.MoveKey (length - i, new Keyframe(length - i, tmpKeyZ.value, -tmpKeyZ.inTangent, -tmpKeyZ.outTangent));
		}
		if (length%2 == 0) {
			CurvesMoveKey (length/2, length/2, -1);
		}
	}

	void HandleMirror () {
		if (!isMirror)
			return;

		isMirror = false;

		for (int i = 0; i < localWaypoints.Length; i++) {
			if(mirrorX) curveX.MoveKey (i, new Keyframe(i, localWaypoints[i].x, -curveX.keys[i].inTangent, -curveX.keys[i].outTangent));
			if(mirrorY) curveY.MoveKey (i, new Keyframe(i, localWaypoints[i].y, -curveY.keys[i].inTangent, -curveY.keys[i].outTangent));
			if(mirrorZ) curveZ.MoveKey (i, new Keyframe(i, localWaypoints[i].z, -curveZ.keys[i].inTangent, -curveZ.keys[i].outTangent));
			localWaypoints [i] = new Vector3 (localWaypoints [i].x * (mirrorX ? -1 : 1), localWaypoints [i].y * (mirrorY ? -1 : 1), localWaypoints [i].z * (mirrorZ ? -1 : 1));
		}
	}

	void CurvesAddKey (int time, Vector3 value) {
		curveX.AddKey (new Keyframe(time, value.x));
		curveY.AddKey (new Keyframe(time, value.y));
		curveZ.AddKey (new Keyframe(time, value.z));
	}

	void CurvesMoveKey (int fromTime, int toTime = -1, float mul = 1) {
		if (toTime == -1)
			toTime = fromTime;
		
		curveX.MoveKey (fromTime, new Keyframe(fromTime, curveX[toTime].value, curveX.keys[toTime].inTangent * mul, curveX.keys[toTime].outTangent * mul));
		curveY.MoveKey (fromTime, new Keyframe(fromTime, curveY[toTime].value, curveY.keys[toTime].inTangent * mul, curveY.keys[toTime].outTangent * mul));
		curveZ.MoveKey (fromTime, new Keyframe(fromTime, curveZ[toTime].value, curveZ.keys[toTime].inTangent * mul, curveZ.keys[toTime].outTangent * mul));
	}
	
	void CurvesMoveKey (int fromTime, Vector3 value, int toTime = -1, float mul = 1) {
		if (toTime == -1)
			toTime = fromTime;
		
		curveX.MoveKey (fromTime, new Keyframe(fromTime, value.x, curveX.keys[toTime].inTangent * mul, curveX.keys[toTime].outTangent * mul));
		curveY.MoveKey (fromTime, new Keyframe(fromTime, value.y, curveY.keys[toTime].inTangent * mul, curveY.keys[toTime].outTangent * mul));
		curveZ.MoveKey (fromTime, new Keyframe(fromTime, value.z, curveZ.keys[toTime].inTangent * mul, curveZ.keys[toTime].outTangent * mul));
	}
	
	void CurvesRemoveKey (int time) {
		curveX.RemoveKey (time);
		curveY.RemoveKey (time);
		curveZ.RemoveKey (time);
	}

	Vector3 GetGlobalCurvesVector (float keyValue) {
		return transform.TransformDirection(new Vector3 (curveX.Evaluate(keyValue),curveY.Evaluate(keyValue), curveZ.Evaluate(keyValue))) + startPos;
	}

	void OnDrawGizmos () {
		if (localWaypoints != null && isPathDrawing) {
			
			Gizmos.color = Color.white;
			for (int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypoint = transform.TransformDirection (localWaypoints [i]) + startPos;
				Gizmos.DrawLine (globalWaypoint - Vector3.up * gizmosSize, globalWaypoint + Vector3.up * gizmosSize);
				Gizmos.DrawLine (globalWaypoint - Vector3.right * gizmosSize, globalWaypoint + Vector3.right * gizmosSize);
				Gizmos.DrawLine (globalWaypoint - Vector3.forward * gizmosSize, globalWaypoint + Vector3.forward * gizmosSize);
			}

			Vector3 pos;
			Vector3 nextPos = GetGlobalCurvesVector (0);
			int gizMul = 10;
			for (int i = 0; i < (localWaypoints.Length-(isCyclic?0:1))*gizMul; i++) {
				Gizmos.color = Color.Lerp (gizmosStartColor, gizmosEndColor, (i+1f)/(gizMul*(localWaypoints.Length-(isCyclic?0:1))));
				pos = nextPos;
				float curveTime = (i+1f)/gizMul;
				nextPos = GetGlobalCurvesVector (curveTime);
				Gizmos.DrawLine (pos, nextPos);
			}
		}
	}

}