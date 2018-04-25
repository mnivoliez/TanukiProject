using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PathCreator))]
public class PlatformController : MonoBehaviour {

	private PathCreator creator;
	private PathPlatform path;
	
	public bool isPlayingBackwards = false;

	private List<Transform> passengers = new List<Transform>();

	[Header("Movement")]
	[Range(0.1f, 10)] [Tooltip("Travel time between two waypoints in second")]
	public float waypointPeriod = 1f;

	[Range(0, 10)] [Tooltip("Seconds to wait between loops")]
	public float waitTime;

	[Header("Rotation")]
	public float rotateSpeed = 0;

	private int fromWaypointIndex = 0;
	private float percentBetweenWaypoints = 0;
	private float nextMoveTime;
	private bool isReverse = false;

	private Vector3 prevPos;

	void Start() {
		creator = GetComponent<PathCreator>();
		path = creator.path;
		if(isPlayingBackwards) {
			fromWaypointIndex = path.NumSegments-1;
			isReverse = true;
		}
	}

	void FixedUpdate() {
		transform.Rotate(0, rotateSpeed*Time.fixedDeltaTime, 0);
		CalculatePlatformMovement ();
		MovePassengers();
	}

	void CalculatePlatformMovement() {

		if (Time.time < nextMoveTime)
			return;

		percentBetweenWaypoints += Time.fixedDeltaTime * (1f/waypointPeriod);

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
				UpdateWaitTime();
			} else if (fromWaypointIndex < 0) {
				isReverse = false;
				fromWaypointIndex = 0;
				UpdateWaitTime();
			}
		}

		prevPos = transform.position;
		transform.position = GetCurvesVector (fromWaypointIndex, isReverse?1-percentBetweenWaypoints:percentBetweenWaypoints);
	}

	void MovePassengers() {
		float rotation = rotateSpeed*Time.fixedDeltaTime;
		Vector3 platformMvt = transform.position - prevPos;
		foreach (Transform passenger in passengers) {
			passenger.position = (Quaternion.Euler(0, rotation, 0) * (passenger.position - transform.position) + transform.position) + platformMvt;
			passenger.Rotate(0, rotation, 0);
		}
	}

	void UpdateWaitTime() {
		nextMoveTime = Time.time + waitTime;
	}

    Vector3 GetCurvesVector (int index, float keyValue) {
		Vector3[] points = path.GetPointsInSegment(index);
		return creator.startTransform.TransformPoint(Bezier.Evaluate(points[0], points[1], points[2],points[3], keyValue));
	}

    void OnCollisionEnter(Collision col) {
        passengers.Add(col.transform);
    }

    void OnCollisionExit(Collision col) {
        passengers.Remove(col.transform);
    }
}