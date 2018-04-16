using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterfallCreator))]
public class RoadEditor : Editor {

	WaterfallCreator creator;

	void OnSceneGUI() {
		if(creator.autoUpdate && Event.current.type == EventType.Repaint) {
			creator.UpdateWaterfall();
		}
	}

	void OnEnable() {
		creator = (WaterfallCreator)target;
	}
}
