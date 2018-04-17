using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class WaterfallCreator : MonoBehaviour {

	[HideInInspector]
	public int numTris;

	public float spacingU = 1;
	public int divisionV = 0;
	public float waterfallWidth = 1;
	public bool autoUpdate;
	
	[Header("UV")]
	public float tilingU = 1;
	public float tilingV = 1;

	public void UpdateWaterfall() {
		PathPlatform path = GetComponent<PathCreator>().path;
		Vector3[] points = path.CalculateEvenlySpacedPoints(spacingU);
		GetComponent<MeshFilter>().mesh = CreateWaterfallMesh(points, path.IsClosed);
	}

	Mesh CreateWaterfallMesh(Vector3[] points, bool isClosed) {
		Vector3[] verts = new Vector3[points.Length*(2+divisionV)];
		Vector2[] uvs = new Vector2[verts.Length];
		Vector3[] normals = new Vector3[verts.Length];
		numTris = 2*(divisionV+1)*(points.Length-1) + (isClosed?2*(divisionV+1):0);
		int[] tris = new int[numTris*3];
		int vertIndex = 0;
		int triIndex = 0;

		for (int i = 0; i < points.Length; i++) {
			Vector3 forward = Vector3.zero;
			if(i < points.Length-1 || isClosed) {
				forward += points[(i+1)%points.Length] - points[i];
			}
			if(i > 0 || isClosed) {
				forward += points[i] - points[(i-1 + points.Length)%points.Length];
			}
			forward.Normalize();
			
			Vector3 left = new Vector3(-forward.z, 0, forward.x);
			left.Normalize();

			Vector3 up = Vector3.Cross(left, forward);

			float completionPercent = i/(points.Length-1f) * tilingV;
			//float v = 1-Mathf.Abs(2*completionPercent-1);

			for (int v = 0; v < divisionV+1; v++) {
				float divPercent = v/(divisionV+1f);
				verts[vertIndex] = points[i] + (left*waterfallWidth/2f) - (left*waterfallWidth*divPercent);
				uvs[vertIndex] = new Vector2(divPercent*tilingU, completionPercent);
				normals[vertIndex] = up;

				if(i < points.Length-1 || isClosed) {
					tris[triIndex] = vertIndex;
					tris[triIndex+1] = (vertIndex+2+divisionV)%verts.Length;
					tris[triIndex+2] = vertIndex+1;

					tris[triIndex+3] = vertIndex+1;
					tris[triIndex+4] = (vertIndex+2+divisionV)%verts.Length;
					tris[triIndex+5] = (vertIndex+3+divisionV)%verts.Length;
				}

				vertIndex+=1;
				triIndex +=6;
			}

			verts[vertIndex] = points[i] - (left*waterfallWidth/2f);
			uvs[vertIndex] = new Vector2(1*tilingU, completionPercent);
			normals[vertIndex] = up;
			vertIndex+=1;
		}

		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.normals = normals;

		return mesh;
	}

	void OnValidate(){
        if(spacingU < 0.01f) spacingU = 0.01f;
		if(divisionV < 0) divisionV = 0;
    }

}