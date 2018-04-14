using UnityEngine;

[ExecuteInEditMode]
public class TerrainConverter : MonoBehaviour {

	public TerrainData data;
	public bool update = false;

	void Update () {
		if(update && data != null) {
			update = false;
			Texture2D tex = data.alphamapTextures[0];
			Texture2D tex2 = data.alphamapTextures[1];
			for(int i = 0;  i< tex.width; i++) {
				for(int j = 0;  j< tex.width; j++) {
					Color col = tex.GetPixel(i, j);
					tex2.SetPixel(i, j, new Color(col.g,
												0,
												0,
												0));
					tex.SetPixel(i, j, new Color(col.r,
												0,
												col.b,
												col.a));
				}
			}
			tex.Apply();
			tex2.Apply();
			data.alphamapTextures[0] = tex;
			data.alphamapTextures[1] = tex2;
		}
	}

	private void OnGUI () {
		if(data != null)
			GUI.DrawTexture (new Rect (0, 0, data.alphamapWidth * .75f, data.alphamapHeight * .75f), data.alphamapTextures[0], ScaleMode.ScaleToFit, false);
	}
}
