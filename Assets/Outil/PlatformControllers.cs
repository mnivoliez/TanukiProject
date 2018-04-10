using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class PlatformControllers : MonoBehaviour {

	private PathCreator creator;
	private PathPlatform path;
	
	public bool isPlayingBackwards = false;

	[Range(0.1f, 10)] [Tooltip("Travel time between two waypoints in second")]
	public float waypointPeriod = 1f;

	[Range(0, 10)] [Tooltip("Seconds to wait between loops")]
	public float waitTime;

	private int fromWaypointIndex = 0;
	private float percentBetweenWaypoints = 0;
	private float nextMoveTime;
	private bool isReverse = false;

	void Start() {
		creator = GetComponent<PathCreator>();
		path = creator.path;
		if(isPlayingBackwards) {
			fromWaypointIndex = path.NumSegments-1;
			isReverse = true;
		}
	}

	void Update() {
		CalculatePlatformMovement ();
	}

	void CalculatePlatformMovement() {

		if (Time.time < nextMoveTime)
			return;

		percentBetweenWaypoints += Time.deltaTime * (1f/waypointPeriod);

		if (percentBetweenWaypoints >= 1) {
			fromWaypointIndex += (isReverse ? -1 : 1);
			
			percentBetweenWaypoints--;
			if (fromWaypointIndex >= path.NumSegments) {
				if (path.IsClosed) {
					fromWaypointIndex = 0;
				} else {
					isReverse = true;
					fromWaypointIndex = path.NumSegments-1;
				}
				UpdateWaitTime ();
			} else if (fromWaypointIndex < 0) {
				isReverse = false;
				fromWaypointIndex = 0;
				UpdateWaitTime ();
			}
		}

		transform.position = GetCurvesVector (fromWaypointIndex, isReverse?1-percentBetweenWaypoints:percentBetweenWaypoints);
	}

	void UpdateWaitTime() {
		nextMoveTime = Time.time + waitTime;
	}

    void OnCollisionEnter(Collision col) {

        col.transform.parent = transform.parent;

    }

    void OnCollisionExit(Collision col) {
        col.transform.parent = null;
    }

    Vector3 GetCurvesVector (int index, float keyValue) {
		Vector3[] points = path.GetPointsInSegment(index);
		return transform.TransformDirection(BezierPlatform.EvaluateQubic(points[0], points[1], points[2],points[3], keyValue)) + creator.startPos;
	}

}