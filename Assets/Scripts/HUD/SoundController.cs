using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private AudioClip Koda_Dark_Theme;
    [SerializeField] private AudioClip Koda_Light_Theme;
    [SerializeField] private AudioClip Koda_Boss_Theme;

    private bool lanterClosestFoundPlay = false;

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

    // called first
    void OnEnable() {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);
        //Debug.Log(SceneManager.GetActiveScene().name);

        if (SceneManager.GetActiveScene().name == "Boss1" || SceneManager.GetActiveScene().name == "Boss2") {
            StopMusic();
            PlayMusic(Koda_Boss_Theme, true);
        }

        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "Zone Tuto") {
            StopMusic();
            PlayMusic(Koda_Light_Theme, true);
        }
        if (SceneManager.GetActiveScene().name == "Z1-P1-complete" || SceneManager.GetActiveScene().name == "Z1-P2-complete" || SceneManager.GetActiveScene().name == "Z1-P3-complete" ||
            SceneManager.GetActiveScene().name == "Z2-P1-complete" || SceneManager.GetActiveScene().name == "Z2-P2-complete" || SceneManager.GetActiveScene().name == "Z2-P3-complete") {
            StopMusic();
            PlayMusic(Koda_Dark_Theme, true);
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
        if (!lanterClosestFoundPlay) {
            lanterClosestFoundPlay = true;
            fxLanternSource.clip = clip;
            fxLanternSource.Play();
        }
    }

    public void StopLanternSource() {
        if (lanterClosestFoundPlay) {
            fxLanternSource.Stop();
        }
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

    public void StopMusic() {
        musicSource.Stop();
    }

}
