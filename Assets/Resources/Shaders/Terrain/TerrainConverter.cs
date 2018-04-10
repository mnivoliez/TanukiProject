using UnityEngine;

[ExecuteInEditMode]
public class TerrainConverter : MonoBehaviour {

	public TerrainData data;
	public bool update = false;

	void Update () {
		if(update && data != null) {
			update = false;
			Texture2D tex = data.alphamapTextures[0];
			for(int i = 0;  i< tex.width; i++) {
				for(int j = 0;  j< tex.width; j++) {
					Color col = tex.GetPixel(i, j);
					tex.SetPixel(i, j, new Color(col.r,
												 col.b,
												 col.g,
												 col.a));
				}
			}
			tex.Apply();
			data.alphamapTextures[0] = tex;
		}
	}

	private void OnGUI () {
		if(data != null)
			GUI.DrawTexture (new Rect (0, 0, data.alphamapWidth * .75f, data.alphamapHeight * .75f), data.alphamapTextures[0], ScaleMode.ScaleToFit, false);
	}
}
