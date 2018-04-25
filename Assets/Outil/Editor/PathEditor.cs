using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {

	Vector3 offset;

	PathCreator creator;
	PathPlatform Path {
		get { return creator.path; }
	}

	const float segmentSelectDistanceThreshold = 10f;
	int selectedSegmentIndex = -1;

	public override void OnInspectorGUI() {
		if(GUILayout.Button("Reset")){
			Undo.RecordObject(creator, "Reset");
			creator.CreatePath();
		}
		
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("Path", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck();

		if(GUILayout.Button("Add Point")){
			Undo.RecordObject(creator, "Add Segment");
			Path.AddSegmentAtLast();
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

		if(GUILayout.Button("Flatten")){
			Undo.RecordObject(creator, "Flatten");
			Path.FlattenAllPoints();
		}

		if(EditorGUI.EndChangeCheck()) {
			SceneView.RepaintAll();
		}
	}

	void OnSceneGUI() {
		offset = (Application.isPlaying) ? creator.startPos : creator.transform.position;
		Input();
		Draw();
	}

	void Input() {
		Event guiEvent = Event.current;
		Vector2 mousePos = guiEvent.mousePosition;

		if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
			if(selectedSegmentIndex != -1) {
				Undo.RecordObject(creator, "Split Segment");
				Vector3 pos = GetCloserPointInSegment(mousePos, selectedSegmentIndex);
				Path.SplitSegment(WorldToLocal(pos), selectedSegmentIndex);
			}/* else if(!Path.IsClosed){
				Undo.RecordObject(creator, "Add Segment");
				Path.AddSegment(mousePos);
			}*/
		}

		if(guiEvent.type == EventType.MouseDown && guiEvent.button == 1) {
			float minDst = float.MaxValue;
			int closestAnchorIndex = -1;
			for (int i = 0; i < Path.NumPoints; i+=3) {
				float dstToAnchor = (creator.anchorDiameter*.5f)/HandleUtility.GetHandleSize(LocalToWorld(Path[i]))*100;
				float dst = Vector2.Distance(mousePos, HandleUtility.WorldToGUIPoint(LocalToWorld(Path[i])));
				if(dst < dstToAnchor && dst < minDst) {
					closestAnchorIndex = i;
					minDst = dst;
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
				Vector3[] points = LocalToWorld(Path.GetPointsInSegment(i));
				float dst = HandleUtility.DistancePointBezier(mousePos, HandleUtility.WorldToGUIPoint(points[0]),
																		HandleUtility.WorldToGUIPoint(points[3]),
																		HandleUtility.WorldToGUIPoint(points[1]),
																		HandleUtility.WorldToGUIPoint(points[2]));
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

		HandleUtility.AddDefaultControl(0);
	}

	void Draw() {
		for (int i = 0; i < Path.NumSegments; i++) {
			Vector3[] points = LocalToWorld(Path.GetPointsInSegment(i));
			if(creator.displayControlPoints) {
				Handles.color = Color.black;
				Handles.DrawDottedLine(points[1], points[0], 4f);
				Handles.DrawDottedLine(points[2], points[3], 4f);
			}
			Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentColor : creator.segmentColor;
			Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
		}

		Handles.color = Color.red;
		for (int i = 0; i < Path.NumPoints; i++) {
			if(i%3 == 0 || creator.displayControlPoints) {
				Handles.color = (i%3==0)?creator.anchorColor:creator.controlColor;
				float handleSize = (i%3==0)?creator.anchorDiameter:creator.controlDiamater;
				Vector3 newPos;
				Vector3 pathId = LocalToWorld(Path[i]);
				if(creator.displayTranslateHandles)
					newPos = Handles.PositionHandle(pathId, Quaternion.identity);
				else
					newPos = Handles.FreeMoveHandle(pathId, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
				if(pathId != newPos) {
					Undo.RecordObject(creator, "Move Point");
					Path.MovePoint(i, WorldToLocal(newPos), creator.selection);
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

	Vector3 WorldToLocal(Vector3 pos) {
		return creator.transform.InverseTransformDirection(pos - offset);
	}

	Vector3 LocalToWorld(Vector3 pos) {
		return creator.transform.TransformDirection(pos) + offset;
	}

	Vector3[] LocalToWorld(Vector3[] points) {
		for (int i = 0; i < points.Length; i++) {
			points[i] = creator.transform.TransformDirection(points[i]) + offset;
		}
		return points;
	}

	Vector3 GetCloserPointInSegment(Vector2 mousePos, int segmentIndex) {
		Vector3[] points = LocalToWorld(Path.GetPointsInSegment(segmentIndex));

		return Bezier.EvaluateQubic(points[0], points[1], points[2], points[3], GetPercentOnBezier(points, mousePos, 0.5f, 0.25f, 10));
	}

	float GetPercentOnBezier(Vector3[] points, Vector2 mousePos, float t, float offset, int count) {
		if(count <= 0) return t;

		Vector2 bezierPosLeft = HandleUtility.WorldToGUIPoint(Bezier.EvaluateQubic(points[0], points[1], points[2], points[3], t-offset));
		Vector2 bezierPosRight = HandleUtility.WorldToGUIPoint(Bezier.EvaluateQubic(points[0], points[1], points[2], points[3], t+offset));
		float dstLeft = Vector2.Distance(mousePos, bezierPosLeft);
		float dstRight = Vector2.Distance(mousePos, bezierPosRight);
		
		return GetPercentOnBezier(points, mousePos, (dstLeft < dstRight) ? t-offset : t+offset, offset/2, --count);
	}
}
