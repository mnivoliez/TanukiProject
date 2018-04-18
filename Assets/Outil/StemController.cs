using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class StemController : MonoBehaviour {

    static System.Random rnd = new System.Random();
    private PathCreator creator;
	private PathPlatform path;
	[SerializeField] private GameObject[] prefabsStem;

	void Start() {
		creator = GetComponent<PathCreator>();
		path = creator.path;
		GenerateStem();
	}

	void GenerateStem() {

        GameObject previousStem = null;
        Vector3 vecDir;
        Vector3 downPreviousStem;
        Vector3 vectoriel;
        float angle;
        int nbSegment = path.NumSegments;
        GameObject stem = null;
        int r = 0;

        float sizeYStem = prefabsStem[0].GetComponent<MeshRenderer>().bounds.size.y;

        float partSizeStemMesh = sizeYStem * 0.20f;

        Vector3[] points = path.CalculateEvenlySpacedPoints(sizeYStem - partSizeStemMesh);

        foreach (Vector3 point in points) {

            int rTemp = r;
            if (prefabsStem.Length > 1) {
                while (rTemp == r) {
                    rTemp = rnd.Next(prefabsStem.Length);
                }
            }
            else {
                rTemp = rnd.Next(prefabsStem.Length);
            }

            r = rTemp;

            int rotationY = rnd.Next(360);

            stem = Instantiate(prefabsStem[r]);
            stem.transform.position = transform.position + point;

            if (previousStem != null) {
                vecDir = (stem.transform.position - previousStem.transform.position);
                downPreviousStem = -previousStem.transform.up;
                vectoriel = Vector3.Cross(downPreviousStem, vecDir);

                angle = Vector3.SignedAngle(downPreviousStem, vecDir, vectoriel);

                previousStem.transform.RotateAround(previousStem.transform.position, vectoriel, angle);
                previousStem.transform.Rotate(transform.up, rotationY);
            }

            if (points[points.Length-1] != point) {
                previousStem = stem;
            }
        }

        vecDir = (previousStem.transform.position - stem.transform.position);
        downPreviousStem = stem.transform.up;
        vectoriel = Vector3.Cross(downPreviousStem, vecDir);

        angle = Vector3.SignedAngle(downPreviousStem, vecDir, vectoriel);

        stem.transform.RotateAround(stem.transform.position, vectoriel, angle);
        
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