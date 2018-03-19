using System;
using UnityEngine;
using UnityEditor;

public class LanternShaderGUI : ShaderGUI {

	private enum BlendMode {
		Simple,
		Lantern
	}

	private enum ShaderType {
		Base,
		Water
	}

	private string shaderName = "Custom/Lantern/WorldBlending";

	private MaterialProperty _Mode = null;

	private MaterialProperty _IsAlpha = null;
	GUIContent isAlphaGUI = new GUIContent ("Alpha is Mask ?", "Take (A) as a mask ?");

	private MaterialProperty _FirstTexture = null;
	GUIContent firstTextureGUI = new GUIContent ("Light", "Albedo (RGB) and Alpha Cutout (A)");
	GUIContent firstTextureWaterGUI = new GUIContent ("Light", "Albedo (RGB)");
	private MaterialProperty _FirstLColor = null;
	GUIContent firstLColorGUI = new GUIContent ("Bright", "Bright Color (RGB) and Emissive (A)");
	GUIContent firstColorWaterGUI = new GUIContent ("Water", "Albedo (RGB) and Transparency (A)");
	private MaterialProperty _FirstDColor = null;
	GUIContent firstDColorGUI = new GUIContent ("Dim", "Dim Color (RGB) and Emissive (A)");


	private MaterialProperty _SecondTexture = null;
	GUIContent secondTextureGUI = new GUIContent ("Dark", "Albedo (RGB) and Alpha Cutout (A)");
	GUIContent secondTextureWaterGUI = new GUIContent ("Dark", "Albedo (RGB)");
	private MaterialProperty _SecondLColor = null;
	GUIContent secondLColorGUI = new GUIContent ("Bright", "Bright Color (RGB) and Emissive (A)");
	GUIContent secondColorWaterGUI = new GUIContent ("Water", "Albedo (RGB) and Transparency (A)");
	private MaterialProperty _SecondDColor = null;
	GUIContent secondDColorGUI = new GUIContent ("Dim", "Dim Color (RGB) and Emissive (A)");

	private MaterialProperty _SpecIntensity = null;
	GUIContent specIntensityGUI = new GUIContent ("Intensity", "Specular Intensity");
	private MaterialProperty _SpecPow = null;
	GUIContent specPowerGUI = new GUIContent ("Power", "Specular Power");
	private MaterialProperty _RimIntensity = null;
	GUIContent rimIntensityGUI = new GUIContent ("Intensity", "Rim Intensity");
	private MaterialProperty _RimPow = null;
	GUIContent rimPowerGUI = new GUIContent ("Power", "Rim Power");

	private MaterialProperty _StepCount = null;
	GUIContent stepCountGUI = new GUIContent ("Step", "Number of light step");

	private MaterialProperty _IsMask = null;
	GUIContent isMaskGUI = new GUIContent ("Use Mask ?", "");

	private MaterialProperty _MaskTexture = null;
	GUIContent maskTextureGUI = new GUIContent ("Tex", "Light (R) and Dark (G) masks");
	private MaterialProperty _MaskRColor = null;
	GUIContent maskRGUI = new GUIContent ("R", "Albedo (RGB) and Transparency (A) and Emissive (Slider)");
	private MaterialProperty _MaskREmi = null;
	private MaterialProperty _MaskGColor = null;
	GUIContent maskGGUI = new GUIContent ("G", "Albedo (RGB) and Transparency (A) and Emissive (Slider)");
	private MaterialProperty _MaskGEmi = null;

	private MaterialProperty _FirstFoamColor = null;
	GUIContent firstColorFoamGUI = new GUIContent ("Foam", "Albedo (RGB) and Emissive (A)");
	private MaterialProperty _SecondFoamColor = null;
	GUIContent secondColorFoamGUI = new GUIContent ("Foam", "Albedo (RGB) and Emissive (A)");
	private MaterialProperty _Depth = null;
	GUIContent depthGUI = new GUIContent ("Depth", "Depth of water detection");
	private MaterialProperty _EdgeIntensity = null;
	GUIContent edgeItensityGUI = new GUIContent ("Edge Intensity", "Intensity of edge detection");
	private MaterialProperty _WaveDir = null;
	GUIContent waveDirGUI = new GUIContent ("Angle", "Direction of wave. 0 is local X+");
	private MaterialProperty _WaveSpeed = null;
	GUIContent waveSpeedGUI = new GUIContent ("Speed", "Speed of wave");
	private MaterialProperty _WaveAmount = null;
	GUIContent waveAmountGUI = new GUIContent ("Amount", "Amount of wave");
	private MaterialProperty _WaveHeight = null;
	GUIContent waveHeightGUI = new GUIContent ("Height", "Height of wave");
	private MaterialProperty _Tessellation = null;
	GUIContent tessellationGUI = new GUIContent ("Tessellation", "Number of mesh division");
	private MaterialProperty _Tess = null;
	private MaterialProperty _NoiseTexture = null;
	GUIContent noiseTextureGUI = new GUIContent ("Tex", "Noise (R)");
	private MaterialProperty _NoiseIntensity = null;
	GUIContent noiseIntensityGUI = new GUIContent ("", "Noise Intensity");

	private int pixelSpace = 8;

	private MaterialEditor editor;
	private Material target;

	private ShaderType shaderType;

	public void FindProperties (MaterialProperty[] props) {
		_Mode = ShaderGUI.FindProperty ("_Mode", props);

		_FirstTexture = FindProperty ("_FirstTexture", props);
		_FirstLColor = FindProperty ("_FirstLColor", props);
		_SecondTexture = FindProperty ("_SecondTexture", props);
		_SecondLColor = FindProperty ("_SecondLColor", props);
		
		if (ShaderType.Base == shaderType) {
			_IsAlpha = ShaderGUI.FindProperty ("_IsAlpha", props);

			_FirstDColor = FindProperty ("_FirstDColor", props);
			_SecondDColor = ShaderGUI.FindProperty ("_SecondDColor", props);

			_SpecIntensity = FindProperty ("_SpecIntensity", props);
			_SpecPow = FindProperty ("_SpecPow", props);
			_RimIntensity = FindProperty ("_RimIntensity", props);
			_RimPow = FindProperty ("_RimPow", props);

			_StepCount = ShaderGUI.FindProperty ("_StepCount", props);

			_IsMask = ShaderGUI.FindProperty ("_IsMask", props);
			
			_MaskTexture = ShaderGUI.FindProperty ("_MaskTexture", props);
			_MaskRColor = ShaderGUI.FindProperty ("_MaskRColor", props);
			_MaskREmi = ShaderGUI.FindProperty ("_MaskREmi", props);
			_MaskGColor = ShaderGUI.FindProperty ("_MaskGColor", props);
			_MaskGEmi = ShaderGUI.FindProperty ("_MaskGEmi", props);
		} else {
			_FirstFoamColor = FindProperty ("_FirstFoamColor", props);
			_SecondFoamColor = FindProperty ("_SecondFoamColor", props);

			_Depth = ShaderGUI.FindProperty ("_Depth", props);
			_EdgeIntensity = ShaderGUI.FindProperty ("_EdgeIntensity", props);

			_WaveDir = ShaderGUI.FindProperty ("_WaveDir", props);
			_WaveSpeed = ShaderGUI.FindProperty ("_WaveSpeed", props);
			_WaveAmount = ShaderGUI.FindProperty ("_WaveAmount", props);
			_WaveHeight = ShaderGUI.FindProperty ("_WaveHeight", props);
			_Tessellation = ShaderGUI.FindProperty ("_Tessellation", props);
			_Tess = ShaderGUI.FindProperty ("_Tess", props);

			_NoiseTexture = ShaderGUI.FindProperty ("_NoiseTexture", props);
			_NoiseIntensity = ShaderGUI.FindProperty ("_NoiseIntensity", props);
		}
	}

	public override void OnGUI (MaterialEditor editor, MaterialProperty[] properties) {
		this.editor = editor;
		this.target = editor.target as Material;

		if (target.shader.name.Equals (shaderName))
			shaderType = ShaderType.Base;
		else
			shaderType = ShaderType.Water;

		FindProperties (properties);

		DoBase ();
	}

	void DoBase() {
		EditorGUI.BeginChangeCheck ();
		editor.ShaderProperty (_Mode, _Mode.displayName);
		bool hasChanged = EditorGUI.EndChangeCheck ();
		
		if ((BlendMode)_Mode.floatValue == BlendMode.Simple) {
			DoSimpleArea ();
			if (shaderType == ShaderType.Base) {
				DoSpecArea ();
				DoToonArea ();
			} else {
				DoWaterArea ();
			}
			if (hasChanged)	SetKeywords (false, false);
		} else {
			DoLanternArea ();
			if (shaderType == ShaderType.Base) {
				DoSpecArea ();
				DoToonArea ();
				DoMaskStartArea ();
				if (_IsMask.floatValue == 1) {
					DoMaskEndArea ();
					if (hasChanged)	SetKeywords (true, true);
				} else {
					if (hasChanged) SetKeywords (true, false);
				}
			} else {
				DoWaterArea ();
				if (hasChanged) SetKeywords (true, false);
			}
		}
		GUILayout.Space(pixelSpace);
	}

	void DoSimpleArea () {
		GUILayout.Space(pixelSpace);
		if (shaderType == ShaderType.Base) {
			editor.ShaderProperty (_IsAlpha, isAlphaGUI);
			GUILayout.Space(pixelSpace);
		}
		EditorGUILayout.LabelField ("Texture", EditorStyles.boldLabel);
		if (shaderType == ShaderType.Base) {
			firstTextureGUI.text = "Tex";
			TextureInline (_FirstTexture, _FirstLColor, _FirstDColor, firstTextureGUI, firstLColorGUI, firstDColorGUI);
		} else {
			firstTextureWaterGUI.text = "Tex";
			TextureInline (_FirstTexture, _FirstLColor, _FirstFoamColor, firstTextureWaterGUI, firstColorWaterGUI, firstColorFoamGUI);
		}
		
		GUILayout.Space(pixelSpace/2);
		editor.TextureScaleOffsetProperty (_FirstTexture);
	}

	void DoLanternArea () {
		GUILayout.Space(pixelSpace);
		if (shaderType == ShaderType.Base) {
			editor.ShaderProperty (_IsAlpha, isAlphaGUI);
			GUILayout.Space(pixelSpace);
		}

		if (shaderType == ShaderType.Base) {
			EditorGUILayout.LabelField ("Light Texture", EditorStyles.boldLabel);
			TextureInline (_FirstTexture, _FirstLColor, _FirstDColor, firstTextureGUI, firstLColorGUI, firstDColorGUI);
			GUILayout.Space(pixelSpace);
			EditorGUILayout.LabelField ("Dark Texture", EditorStyles.boldLabel);
			TextureInline (_SecondTexture, _SecondLColor, _SecondDColor, secondTextureGUI, secondLColorGUI, secondDColorGUI);
		} else {
			EditorGUILayout.LabelField ("Light Texture", EditorStyles.boldLabel);
			TextureInline (_FirstTexture, _FirstLColor, _FirstFoamColor, firstTextureWaterGUI, firstColorWaterGUI, firstColorFoamGUI);
			GUILayout.Space(pixelSpace);
			EditorGUILayout.LabelField ("Dark Texture", EditorStyles.boldLabel);
			TextureInline (_SecondTexture, _SecondLColor, _SecondFoamColor, secondTextureWaterGUI, secondColorWaterGUI, secondColorFoamGUI);
		}

		GUILayout.Space(pixelSpace/2);
		editor.TextureScaleOffsetProperty (_FirstTexture);
	}

	void DoSpecArea() {
		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Specular", EditorStyles.boldLabel);
		TwoPropertyInline (_SpecIntensity, _SpecPow, specIntensityGUI, specPowerGUI);

		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Rim", EditorStyles.boldLabel);
		TwoPropertyInline (_RimIntensity, _RimPow, rimIntensityGUI, rimPowerGUI);
	}

	void DoToonArea () {
		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Toon", EditorStyles.boldLabel);
		editor.ShaderProperty (_StepCount, stepCountGUI);
	}

	void DoMaskStartArea() {
		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Mask", EditorStyles.boldLabel);
		editor.ShaderProperty (_IsMask, isMaskGUI);
	}

	void DoMaskEndArea () {
		GUILayout.Space(pixelSpace*2);
		Rect rect = editor.TexturePropertySingleLine(maskTextureGUI, _MaskTexture);

		Rect rect1 = new Rect (rect);
		rect1.xMin = rect.width / 2.265f -14;
		rect1.xMax = rect1.xMin + rect.width/5f;
		Rect rect2 = new Rect (rect1);
		rect1.y -= 10;
		rect2.y += 10;
		editor.ShaderProperty (rect1, _MaskRColor, "");
		editor.ShaderProperty (rect2, _MaskGColor, "");
		GUI.Label (new Rect(rect1.x - 15, rect1.y, rect1.width, rect1.height), maskRGUI);
		GUI.Label (new Rect(rect2.x - 15, rect2.y, rect2.width, rect2.height), maskGGUI);
		rect1.x = rect2.x += rect1.width*2f;
		rect1.xMin = rect2.xMin -= rect1.width*0.75f;
		editor.ShaderProperty (rect1, _MaskREmi, "");
		editor.ShaderProperty (rect2, _MaskGEmi, "");

		GUILayout.Space(pixelSpace*2);
		editor.TextureScaleOffsetProperty (_MaskTexture);
	}

	void DoWaterArea() {
		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Edge", EditorStyles.boldLabel);
		editor.ShaderProperty (_Depth, depthGUI);
		editor.ShaderProperty (_EdgeIntensity, edgeItensityGUI);
		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Wave", EditorStyles.boldLabel);
		editor.ShaderProperty (_WaveDir, waveDirGUI);
		editor.ShaderProperty (_WaveSpeed, waveSpeedGUI);
		editor.ShaderProperty (_WaveAmount, waveAmountGUI);
		editor.ShaderProperty (_WaveHeight, waveHeightGUI);
		editor.ShaderProperty (_Tessellation, tessellationGUI);
		_Tess.floatValue = _Tessellation.floatValue * 2 - 1;

		GUILayout.Space(pixelSpace);
		EditorGUILayout.LabelField ("Noise", EditorStyles.boldLabel);
		TextureInline (_NoiseTexture, _NoiseIntensity, noiseTextureGUI, noiseIntensityGUI);
		GUILayout.Space(pixelSpace/2);
		editor.TextureScaleOffsetProperty (_NoiseTexture);
	}

	void TextureInline(MaterialProperty propTex, MaterialProperty propLCol, MaterialProperty propDCol, GUIContent tex, GUIContent lCol, GUIContent dCol) {
		Rect rect = editor.TexturePropertySingleLine(tex, propTex);
		if (propLCol != null) {
			Rect rect1 = new Rect (rect);
			rect1.xMin = rect.width / 2.265f -14;
			rect1.xMax = rect1.xMin + rect.width / 5f;
			editor.ShaderProperty (rect1, propLCol, "");
			rect1.x -= 40;
			GUI.Label (rect1, lCol);
		}

		if (propDCol != null) {
			Rect rect2 = new Rect (rect);
			rect2.xMin = rect2.xMax - rect.width / 5f;
			editor.ShaderProperty (rect2, propDCol, "");
			rect2.x -= 40;
			GUI.Label (rect2, dCol);
		}
	}

	void TextureInline(MaterialProperty propTex, MaterialProperty propCol, GUIContent tex, GUIContent col) {
		Rect rect = editor.TexturePropertySingleLine(tex, propTex);

		rect.xMin = rect.width / 2.265f -14;
		editor.ShaderProperty (rect, propCol, "");
		rect.x -= 40;
		GUI.Label (rect, col);
	}

	void TwoPropertyInline(MaterialProperty prop1, MaterialProperty prop2, GUIContent prop1Text, GUIContent prop2Text) {
		Rect rect = EditorGUILayout.GetControlRect ();

		Rect rect1 = new Rect (rect);
		rect1.xMin += 60;
		rect1.xMax = rect.width / 2f+14;
		editor.ShaderProperty (rect1, prop1, "");
		rect1.x -= 60;
		GUI.Label (rect1, prop1Text);

		Rect rect2 = new Rect (rect);
		rect2.xMin = rect.width / 2f+14 + 60;
		editor.ShaderProperty (rect2, prop2, "");
		rect2.x -= 45;
		GUI.Label (rect2, prop2Text);
	}

	void SetKeyword (string keyword, bool state) {
		if (state)
			target.EnableKeyword (keyword);
		else
			target.DisableKeyword (keyword);
	}

	void SetKeywords (bool isLantern, bool isMask) {
		SetKeyword ("_SIMPLE", !isLantern);
		SetKeyword ("_LANTERN", isLantern);
		SetKeyword ("_ISMASK_ON", isMask);
	}
}
