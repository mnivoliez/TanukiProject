using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianaController : MonoBehaviour {

    [SerializeField]
    private float step = 0.7f;
    [SerializeField]
    private List<GameObject> leafs;

    private void Start() {
        Vector3 position = transform.position;
        GameObject obj_old = new GameObject();
        GameObject obj_new = new GameObject();
        for (int i = 0; i < leafs.Count; i++) {
            obj_old = obj_new;
            obj_new = Instantiate(leafs[i], position, leafs[i].transform.rotation );
            obj_new.transform.parent = transform;
            position.y = position.y - step;
            if (i != 0) {
                obj_new.GetComponent<SpringJoint>().connectedBody = obj_old.GetComponent<Rigidbody>();
            }
        }
    }
}
