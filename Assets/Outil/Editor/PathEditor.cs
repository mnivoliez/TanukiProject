using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {

	Vector3 offset;

	int index;

	PathCreator creator;
	PathPlatform Path {
		get { return creator.path; }
	}

	const float segmentSelectDistanceThreshold = .1f;
	int selectedSegmentIndex = -1;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();

		if(GUILayout.Button("Add")){
			Undo.RecordObject(creator, "Add Segment");
			Path.AddSegmentAtLast();
		}

		index = EditorGUILayout.IntField ("Index", index);
		index = Mathf.Clamp(index, 0, Path.NumSegments);

		if(GUILayout.Button("Insert")){
			Undo.RecordObject(creator, "Insert Segment");
			Path.SplitAfterSegment(index);
		}

		if(GUILayout.Button("Remove")){
			Undo.RecordObject(creator, "Remove Segment");
			Path.DeleteSegment(index*3);
		}

		bool isClosed = GUILayout.Toggle(Path.IsClosed, "Closed");
		if(isClosed != Path.IsClosed){
			Undo.RecordObject(creator, "Toggle Closed");
			Path.IsClosed = isClosed;
		}

		bool autoSetControlPoints = GUILayout.Toggle(Path.AutoSetControPoints, "Auto Set Control Points");
		if(autoSetControlPoints != Path.AutoSetControPoints) {
			Undo.RecordObject(creator, "Toggle Auto Set Controls");
			Path.AutoSetControPoints = autoSetControlPoints;
		}
		if(GUILayout.Button("Reset")){
			Undo.RecordObject(creator, "Reset");
			creator.CreatePath();
		}

		if(EditorGUI.EndChangeCheck()) {
			SceneView.RepaintAll();
		}
	}

	void OnSceneGUI() {
		offset = (Application.isPlaying) ? creator.startPos : creator.transform.position;
		//Input();
		Draw();
	}

	void Input() {
		Event guiEvent = Event.current;
		Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

		if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
			if(selectedSegmentIndex != -1) {
				Undo.RecordObject(creator, "Split Segment");
				Path.SplitSegment(mousePos, selectedSegmentIndex);
			} else if(!Path.IsClosed){
				Undo.RecordObject(creator, "Add Segment");
				Path.AddSegment(mousePos);
			}
		}

		if(guiEvent.type == EventType.MouseDown && guiEvent.button == 1) {
			float minDstToAnchor = creator.anchorDiameter *.5f;
			int closestAnchorIndex = -1;

			for (int i = 0; i < Path.NumPoints; i+=3) {
				float dst = Vector3.Distance(mousePos, Path[i]);
				if(dst < minDstToAnchor) {
					minDstToAnchor = dst;
					closestAnchorIndex = i;
				}
			}
			if(closestAnchorIndex != -1) {
				Undo.RecordObject(creator, "Delete Segment");
				Path.DeleteSegment(closestAnchorIndex);
			}
		}

		if(guiEvent.type == EventType.MouseMove) {
			float minDstToSegment = segmentSelectDistanceThreshold;
			int newSelectedSegmentIndex = -1;

			for (int i = 0; i < Path.NumSegments; i++) {
				Vector3[] point = Path.GetPointsInSegment(i);
				float dst = HandleUtility.DistancePointBezier(mousePos, point[0], point[3], point[1], point[2]);
				if(dst < minDstToSegment) {
					minDstToSegment = dst;
					newSelectedSegmentIndex = i;
				}
			}

			if(newSelectedSegmentIndex != selectedSegmentIndex) {
				selectedSegmentIndex = newSelectedSegmentIndex;
				HandleUtility.Repaint();
			}
		}
	}

	void Draw() {
		for (int i = 0; i < Path.NumSegments; i++) {
			Vector3[] points = Path.GetPointsInSegment(i);
			if(creator.displayControlPoints) {
				Handles.color = Color.black;
				Handles.DrawDottedLine(LocalToWorld(points[1]), LocalToWorld(points[0]), 4f);
				Handles.DrawDottedLine(LocalToWorld(points[2]), LocalToWorld(points[3]), 4f);
			}
			Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentColor : creator.segmentColor;
			Handles.DrawBezier(LocalToWorld(points[0]), LocalToWorld(points[3]), LocalToWorld(points[1]), LocalToWorld(points[2]), segmentCol, null, 2);
		}

		Handles.color = Color.red;
		for (int i = 0; i < Path.NumPoints; i++) {
			if(i%3 == 0 || creator.displayControlPoints) {
				Handles.color = (i%3==0)?creator.anchorColor:creator.controlColor;
				float handleSize = (i%3==0)?creator.anchorDiameter:creator.controlDiamater;
				Vector3 newPos;
				if(creator.displayTranslateHandles)
					newPos = Handles.PositionHandle(LocalToWorld(Path[i]), Quaternion.identity);
				else
					newPos = Handles.FreeMoveHandle(LocalToWorld(Path[i]), Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
				if(LocalToWorld(Path[i]) != newPos) {
					Undo.RecordObject(creator, "Move Point");
					Path.MovePoint(i, creator.transform.InverseTransformDirection(newPos - offset));
				}
			}
		}
	}

	void OnEnable() {
		creator = (PathCreator)target;
		if(creator.path == null) {
			creator.CreatePath();
		}
	}

	Vector3 LocalToWorld(Vector3 pos) {
		return creator.transform.TransformDirection(pos) + offset;
	}
}
