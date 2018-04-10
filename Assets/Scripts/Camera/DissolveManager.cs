using UnityEngine;

[ExecuteInEditMode]
public class DissolveManager : MonoBehaviour {

	[Header("Dissolve")]
	[Space(10)]
	public Color dissoColor;
	[Range(0,5)]
	public float interpolation;
	[Range(0,5)]
	public float offset;
	[Range(0,1)]
	public float frequency;
	[Range(0,5)]
	public float speed;
	[Space(20)]

	[Header("Lantern")]
	[Space(10)]
	public Transform[] lanterns;
	private Vector4[] lanternsPos;
	public float[] distances;
	[Space(20)]

	public Material[] materials;

	void Update () {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        lanternsPos = new Vector4[lanterns.Length];
		for(int i = 0; i < lanterns.Length; i++) {
			lanternsPos[i] = lanterns[i].position;
		}

		foreach (Material mat in materials) {
			mat.SetColor("_ColorDisso", dissoColor);
			mat.SetFloat("_Interpolation", interpolation);
			mat.SetFloat("_Offset", offset);
			mat.SetFloat("_Freq", frequency);
			mat.SetFloat("_Speed", speed);
			
			mat.SetInt("_LanternCount", lanterns.Length);
			mat.SetFloatArray("_Distances", distances);
			mat.SetVectorArray("_Centers", lanternsPos);
		}
	}
}
