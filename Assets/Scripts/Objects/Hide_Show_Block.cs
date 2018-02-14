using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide_Show_Block : MonoBehaviour {
	//pour que le script marche il faux que les objet qui apparaisse est leur mesh et collider (sur le père) déactiver
	// et pour les objet qui disparaisse il faux que le mesh et le collider du père soit activer
	private Collider boxCollider;
	private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start() {
		boxCollider = transform.parent.GetComponent<Collider>();
		meshRenderer = transform.parent.GetComponent<MeshRenderer>();
		//boxCollider.enabled = false;
		//meshRenderer.enabled = false;
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Lantern") {
			meshRenderer.enabled = !boxCollider.enabled;
			boxCollider.enabled = !boxCollider.enabled;

			print (meshRenderer.enabled.ToString());
		}
	}

	void OnTriggerExit(Collider collider) {
		if (collider.gameObject.tag == "Lantern") {
			meshRenderer.enabled = !boxCollider.enabled;
			boxCollider.enabled = !boxCollider.enabled;
		
			print (meshRenderer.enabled.ToString());
		}
	}

}
