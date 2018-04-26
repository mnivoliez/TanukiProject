using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbRotation : MonoBehaviour {
	
	public float speed = 1f;
	private static bool toggle;

	void Update () {
		if(!toggle) return;

		transform.Rotate(0,0,speed*Time.deltaTime);
	}

	public static void Toggle(bool _toggle) {
		toggle = _toggle;
	}
}
