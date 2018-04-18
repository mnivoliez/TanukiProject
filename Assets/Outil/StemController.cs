using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class StemController : MonoBehaviour {
	
	private PathCreator creator;
	private PathPlatform path;
	// Travel time between two waypoints in second
	[SerializeField] private GameObject prefabStem1;
	[SerializeField] private GameObject prefabStem2;
	[SerializeField] private GameObject prefabStem3;
	[SerializeField] private GameObject prefabStem4;

	void Start() {
		creator = GetComponent<PathCreator>();
		path = creator.path;
		GenerateStem();
	}

	void GenerateStem() {

        GameObject previousStem = null;
        float waypointPeriod = 7f;
        float percentBetweenWaypoints = 0;
        int nbSegment = path.NumSegments;
        Vector3 positionStem = new Vector3();
        GameObject stem = null;
        float sizeYStem = prefabStem1.GetComponent<MeshRenderer>().bounds.size.y;

        for (int i = 0; i < nbSegment; i++) {
            float lenghtArcBezier = GetLengthArcBezier(i);
            int nbPartStem = (int)(lenghtArcBezier / sizeYStem);
            waypointPeriod = nbPartStem;
            for (int j = 0; j <= nbPartStem; j++) {
                if (j < nbPartStem) {
                    percentBetweenWaypoints = j * (1f / waypointPeriod);
                    stem = Instantiate(prefabStem1);
                    positionStem = GetCurvesVector(i, percentBetweenWaypoints);
                    stem.transform.position = positionStem;
                    stem.name = "Segment" + i + "_Part" + j;
                } else {
                    positionStem = previousStem.transform.position;
                }

                if (previousStem != null) {
                    Vector3 vecDir = (stem.transform.position - previousStem.transform.position);
                    Vector3 vecDirDegree = (180.0f / Mathf.PI) * vecDir;
                    //previousStem.transform.LookAt(vecDir + previousStem.transform.up);
                    Debug.Log(vecDirDegree);
                    //previousStem.transform.rotation = Quaternion.FromToRotation(previousStem.transform.position, vecDir);
                    previousStem.transform.rotation = Quaternion.Euler(vecDirDegree);
                }
                if (j < nbPartStem - 1) {
                    previousStem = stem;
                }
            }
        }
	}

    float GetLengthArcBezier(int index) {
        Vector3[] points = path.GetPointsInSegment(index);
        float chord = (points[3] - points[0]).magnitude;
        float cont_net = (points[0] - points[1]).magnitude + (points[2] - points[1]).magnitude + (points[3] - points[2]).magnitude;
        float lenghtArcBezier = (cont_net + chord) / 2;
        return lenghtArcBezier;
    }

	Vector3 GetCurvesVector (int index, float keyValue) {
		Vector3[] points = path.GetPointsInSegment(index);
		return transform.TransformDirection(Bezier.EvaluateQubic(points[0], points[1], points[2],points[3], keyValue)) + creator.startPos;
	}
}