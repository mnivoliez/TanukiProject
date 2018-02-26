using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.
using UnityEngine.Audio; // Required when messing with the MasterVolume

public class Options : MonoBehaviour {

    public Slider mainSlider;
    public int Size_Aspect;
    public int Quality;
    public int height;
    public int width;

    public AudioMixer audioMixer;

    Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;

    public float GammaCorrection;

    private void Start() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i< resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Quality = QualitySettings.GetQualityLevel();
        qualityDropdown.value = Quality;
        qualityDropdown.RefreshShownValue();
    }

    public void SetVolume (float Volume) {
        audioMixer.SetFloat("MasterVolume", Volume);
    }

    public void DisplaySize (int resolutionIndex) {

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, 60);
    }

    public void SetGraphics(int Quality) {
        QualitySettings.SetQualityLevel(Quality);
    }

    public void SetFullscreen(bool isckecked) {
        Screen.fullScreen = isckecked;
    }

    public void SetGammaLight (float GammaCorrection) {
        RenderSettings.ambientLight = new Color(GammaCorrection, GammaCorrection, GammaCorrection, 1.0f);
    }
}
