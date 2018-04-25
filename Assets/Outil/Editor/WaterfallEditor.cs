using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterfallCreator))]
public class RoadEditor : Editor {

	WaterfallCreator creator;

	public override void OnInspectorGUI() {
		EditorGUILayout.IntField("Tris :", creator.numTris, EditorStyles.label);
		EditorGUILayout.Space();
		base.OnInspectorGUI();
	}

	void OnSceneGUI() {
		if(creator.autoUpdate && Event.current.type == EventType.Repaint) {
			creator.UpdateWaterfall();
		}
	}

	void OnEnable() {
		creator = (WaterfallCreator)target;
	}
}
