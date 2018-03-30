using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public AudioSource fxKodaSource;
    public AudioSource fxLeafSource;
    public AudioSource fxYokaiSource;
    public AudioSource fxLanternSource;
    public AudioSource fxLanternEffect;
    public AudioSource fxOtherSource;
    public AudioSource musicSource;
    public static SoundController instance;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    private void Awake() {
        SoundController[] allInstance;
        allInstance = (SoundController[])FindObjectsOfType(typeof(SoundController));

        if (allInstance.Length > 1) {
            Destroy(gameObject);
        }
        else {

            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this) {
                Destroy(gameObject);
            }
           
        }
    }

    public void PlayKodaSingle(AudioClip clip) {
        fxKodaSource.clip = clip;
        fxKodaSource.Play();
    }

    public void PlayLeafSingle(AudioClip clip) {
        fxLeafSource.clip = clip;
        fxLeafSource.Play();
    }

    public void PlayYokaiSingle(AudioClip clip) {
        fxYokaiSource.clip = clip;
        fxYokaiSource.Play();
    }

    public void PlayLanternSingle(AudioClip clip) {
        fxLanternEffect.clip = clip;
        fxLanternEffect.Play();
        //fxLanternSource.volume();
    }

    public void PlayLanternSource(AudioClip clip) {
        fxLanternSource.clip = clip;
        fxLanternSource.Play();
    }

    public void StopLanternSource() {
        fxLanternSource.Stop();
    }

    public void AdjustLanternSource (float setVol) {
        fxLanternSource.volume = setVol;
    }

    public void PlaySingle(AudioClip clip) {
        fxOtherSource.clip = clip;
        fxOtherSource.Play();
    }

    public void StopSingle() {
        fxOtherSource.Stop();
    }

    public void RandomizeFX(params AudioClip [] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        fxKodaSource.pitch = randomPitch;
        fxKodaSource.clip = clips[randomIndex];
        fxKodaSource.Play();
    }

    public void PlayMusic(AudioClip musicClip, bool loop) {
        musicSource.clip = musicClip;
        musicSource.Play();
        musicSource.loop = loop;
    }

}
