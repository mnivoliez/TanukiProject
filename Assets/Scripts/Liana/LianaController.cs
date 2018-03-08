using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianaController : MonoBehaviour {

    [SerializeField]
    private float step = 0.7f;
    [SerializeField]
    private List<GameObject> leafs;

    private void Start() {
        Vector3 localPosition = new Vector3(0f, 0f, 0f);
        for(int i = 0; i < leafs.Count; i++) {
            GameObject obj = Instantiate(leafs[i], Vector3.zero, leafs[i].transform.rotation);
            obj.transform.parent = transform;
            obj.transform.localPosition = localPosition;
            localPosition.y = localPosition.y - step;
        }
    }

    private void Update() {
        Vector3 localPosition = new Vector3(0f, 0f, 0f);
        Debug.Log(leafs.Count);
        for (int i = 0; i < leafs.Count; i++) {
            leafs[i].transform.localPosition = localPosition;
            localPosition.y = localPosition.y - step;
        }
    }
}
