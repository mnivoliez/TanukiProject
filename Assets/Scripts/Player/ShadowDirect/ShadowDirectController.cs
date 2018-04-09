using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDirectController : MonoBehaviour {

    [SerializeField]
    private GameObject shadowDirect;
    [SerializeField]
	private float rayCastDistance = 30.0f;
	[SerializeField]
	private LayerMask ignoredLayerMask;

	private GameObject clone;
	private Vector3 position;
	private Vector3 position_offset = new Vector3(0, 0.05f, 0);
    private float maxSize;
	private float minSize;
    private float size;
    private float initialSize = 0.5f;
    // all layers are = 0xFFFFFFFF => -1
    private int layerAll = -1;

    RaycastHit hit;

    private void Start() {
        maxSize = 1.0f;
        minSize = 0.1f;
		if (Physics.Raycast(transform.position + position_offset, -Vector3.up, out hit, rayCastDistance, layerAll - ignoredLayerMask.value)) {
			position = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z);
        } else {
            position = transform.position;
        }
        clone = Instantiate(shadowDirect, position, transform.rotation);
    }

    private void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (Physics.Raycast(transform.position + position_offset, -Vector3.up, out hit, rayCastDistance, layerAll - ignoredLayerMask.value)) {
			position = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z);
            
            float distance = Vector3.Distance(transform.position, hit.point);
            size = maxSize * distance / rayCastDistance;
            size = 1f - Mathf.Clamp(size, minSize, maxSize);
			Vector3 scale = new Vector3 (size, 0.01f, size);
            clone.transform.localScale = scale;
        } else {
            position = transform.position;
        }
        clone.transform.position = position;
    }

    public void ResizeShadow(bool resizeTiny, float coefResize) {
        if (resizeTiny) {
            minSize = minSize / coefResize;
            maxSize = maxSize / coefResize;
            size = size / coefResize;
            rayCastDistance = rayCastDistance / coefResize;
            clone.transform.GetChild(0).transform.localScale = new Vector3(size, 0.01f, size);
        }
        else {
            minSize = minSize * coefResize;
            maxSize = maxSize * coefResize;
            rayCastDistance = rayCastDistance * coefResize;
            clone.transform.GetChild(0).transform.localScale = new Vector3(initialSize, 0.01f, initialSize);
        }
        
    } 
}
