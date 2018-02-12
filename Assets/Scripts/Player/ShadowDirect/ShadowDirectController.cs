using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDirectController : MonoBehaviour {

    [SerializeField]
    private GameObject shadowDirect;
    [SerializeField]
    private float rayCastDistance = 30.0f;

    private GameObject clone;
    private Vector3 position;
    private float maxSize;
    private float minSize;

    RaycastHit hit;

    private void Start() {
        maxSize = 1.0f;
        minSize = 0.1f;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayCastDistance)) {
            position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
            clone = Instantiate(shadowDirect, position, transform.rotation);
        }
    }

    private void Update() {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayCastDistance)) {
            position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
            clone.transform.position = position;

            float distance = Vector3.Distance(transform.position, hit.point);
            float size = maxSize * distance / rayCastDistance;
            size = 1f - Mathf.Clamp(size, minSize, maxSize);
            Vector3 scale = new Vector3(maxSize, maxSize, maxSize) * size;
            clone.transform.localScale = scale;
            //Debug.Log(size);
        }
    }
}
