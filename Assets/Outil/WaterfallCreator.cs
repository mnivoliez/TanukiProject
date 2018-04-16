using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class WaterfallCreator : MonoBehaviour {

	[Range(1,20)]
	public float spacing = 1;
	public float waterfallWidth = 1;
	public bool autoUpdate;
	public float tiling = 1;

	public void UpdateWaterfall() {
		PathPlatform path = GetComponent<PathCreator>().path;
		Vector3[] points = path.CalculateEvenlySpacedPoints(spacing);
		GetComponent<MeshFilter>().mesh = CreateWaterfallMesh(points, path.IsClosed);
	}

	Mesh CreateWaterfallMesh(Vector3[] points, bool isClosed) {
		Vector3[] verts = new Vector3[points.Length*2];
		Vector2[] uvs = new Vector2[verts.Length];
		Vector3[] normals = new Vector3[verts.Length];
		int numTris = 2*(points.Length-1) + (isClosed?2:0);
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

			verts[vertIndex] = points[i] + left*waterfallWidth/2f;
			verts[vertIndex+1] = points[i] - left*waterfallWidth/2f;

			float completionPercent = i/(points.Length-1f);
			uvs[vertIndex] = new Vector2(0, completionPercent);
			uvs[vertIndex+1] = new Vector2(1, completionPercent);

			Vector3 up = Vector3.Cross(left, forward);

			normals[vertIndex] = normals[vertIndex+1] = up;

			if(i < points.Length-1 || isClosed) {
				tris[triIndex] = vertIndex;
				tris[triIndex+1] = (vertIndex+2)%verts.Length;
				tris[triIndex+2] = vertIndex+1;

				tris[triIndex+3] = vertIndex+1;
				tris[triIndex+4] = (vertIndex+2)%verts.Length;
				tris[triIndex+5] = (vertIndex+3)%verts.Length;
			}

			vertIndex+=2;
			triIndex +=6;
		}

		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.normals = normals;

		return mesh;
	}
}
