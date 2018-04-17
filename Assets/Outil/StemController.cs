using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class StemController : MonoBehaviour {
	
	private PathCreator creator;
	private PathPlatform path;
	private float percentBetweenWaypoints = 0;
	private int fromWaypointIndex = 0;
	// Travel time between two waypoints in second
	private float waypointPeriod = 7f;
	[SerializeField] private GameObject prefabStem1;
	[SerializeField] private GameObject prefabStem2;
	[SerializeField] private GameObject prefabStem3;
	[SerializeField] private GameObject prefabStem4;

	void Start() {
		creator = GetComponent<PathCreator>();
		path = creator.path;
		fromWaypointIndex = 0;
		GenerateStem();
	}

	void GenerateStem() {

		int i = 0;
		while (i <= 7) {
			Debug.Log ("i: " + i);
			percentBetweenWaypoints = i * (1f/waypointPeriod);

			if (percentBetweenWaypoints >= 1 && fromWaypointIndex < path.NumPoints) {
				fromWaypointIndex++;
				Debug.Log ("reset i ! because percentBetweenWaypoints: " + percentBetweenWaypoints);
				i = 0;
				Debug.Log ("i: " + i);
				Debug.Log ("segment: " + fromWaypointIndex);
				percentBetweenWaypoints--;
			}

			Instantiate (prefabStem1);
			prefabStem1.transform.position = GetCurvesVector (fromWaypointIndex, percentBetweenWaypoints);
			prefabStem1.name = "Segment" + fromWaypointIndex + "_Part" + i;
			if (percentBetweenWaypoints < 1 || fromWaypointIndex >= path.NumPoints) {
				i++;
			}
		}
	}

	Vector3 GetCurvesVector (int index, float keyValue) {
		Vector3[] points = path.GetPointsInSegment(index);
		return transform.TransformDirection(Bezier.EvaluateQubic(points[0], points[1], points[2],points[3], keyValue)) + creator.startPos;
	}
}