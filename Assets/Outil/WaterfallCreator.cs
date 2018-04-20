using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class WaterfallCreator : MonoBehaviour {

	[HideInInspector]
	public int numTris;

	public float spacingU = 1;
	public int divisionV = 2;
	public float maxWaterfallWidth = 5;
	public bool autoUpdate;
	
	[Header("UV")]
	public float tilingU = 1;
	public float tilingV = 1;

	[Range(0,1)]
	public float waterfallSpeed = 0;

	public LayerMask raycastLayer = 1<<8;

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

		float waterfallMul = 0;
		float waterfallMulLerp = 0;
		float uvProgress = 0;

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

			waterfallMul = (forward.y >= 0) ? 0 : -forward.y;
			waterfallMulLerp = Mathf.Lerp(waterfallMulLerp, waterfallMul, waterfallSpeed);

			float uvOffset = 1f/(points.Length-1f) * tilingV;

			uvProgress += uvOffset - (uvOffset * waterfallMulLerp);

			float leftDist = maxWaterfallWidth/2f;
			float rightDist = maxWaterfallWidth/2f;

			RaycastHit hit;
			if(Physics.Raycast(transform.TransformPoint(points[i]), transform.TransformDirection(left), out hit, leftDist, raycastLayer)) {
				leftDist = hit.distance;
			}
			if(Physics.Raycast(transform.TransformPoint(points[i]), transform.TransformDirection(-left), out hit, rightDist, raycastLayer)) {
				rightDist = hit.distance;
			}

			leftDist += 2;
			rightDist += 2;

			for (int v = 0; v < divisionV+1; v++) {

				float divPercent = v/(divisionV+1f);
				verts[vertIndex] = points[i] + (left*leftDist) - (left*rightDist*2*divPercent);
				uvs[vertIndex] = new Vector2(divPercent*tilingU, uvProgress);
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

			verts[vertIndex] = points[i] - (left*rightDist);
			uvs[vertIndex] = new Vector2(1*tilingU, uvProgress);
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