using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class SteleBehavior : MonoBehaviour {

    [SerializeField] private GameObject nextStele;
    [SerializeField] private bool lastStele;
    private PathCreator creator;
	private PathPlatform path;

    [SerializeField] private float pathPointsSpacing = 1f;
    [SerializeField] private float pathResolution = 1f;

    void Start() {
        if(nextStele) {
            creator = GetComponent<PathCreator>();
		    path = creator.path;
        }
	}

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            HitodamaController hitodama = GameObject.FindGameObjectWithTag("Hitodama").GetComponent<HitodamaController>();
            if (lastStele) {
                hitodama.SetIsGuiding(false);
            }
            else {
                hitodama.SetIsGuiding(true);
                hitodama.SetTargetStele(nextStele);
                Vector3[] points = path.CalculateEvenlySpacedPoints(pathPointsSpacing, pathResolution);
                for(int i = 0; i < points.Length; ++i)
                {
                    points[i] = transform.TransformDirection(points[i]) + transform.position;
                }
                hitodama.SetPath(points);
            }
        }

    }
}
