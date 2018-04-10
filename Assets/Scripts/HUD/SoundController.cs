﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour {
    public static SoundController instance;

    [Header("MANAGER")]
    [Space(10)]
    public AudioSource fxThemeSource;
    public AudioSource fxHUD_Source;
    public AudioSource fxHUD_Effect;
    public AudioSource fxKodaEffect;
    public AudioSource fxLeafEffect;
    public AudioSource fxYokaiEffect;
    public AudioSource fxLanternSource;
    public AudioSource fxLanternEffect;
    public AudioSource fxEnvironnementEffectQuick;
    public AudioSource fxEnvironnementEffectLong;

    [Header("THEMES")]
    [Space(10)]
    [SerializeField] private AudioClip themeLightTuto;
    [SerializeField] private AudioClip themeDarkZ1;
    [SerializeField] private AudioClip themeLightZ1;
    [SerializeField] private AudioClip themeBossZ1;
    [SerializeField] private AudioClip themeDarkZ2;
    [SerializeField] private AudioClip themeLightZ2;
    [SerializeField] private AudioClip themeBossZ2;

    [Header("HUD")]
    [Space(10)]
    [SerializeField] private AudioClip HUDMainMenuTheme;
    [SerializeField] private AudioClip HUDPauseTheme;
    [SerializeField] private AudioClip HUDPauseEffect;

    [Header("KODA")]
    [Space(10)]
    [SerializeField] private AudioClip kodaJump1;
    [SerializeField] private AudioClip kodaJump2;
    [SerializeField] private AudioClip kodaJump3;

    [SerializeField] private AudioClip kodaFootStepGround1;
    [SerializeField] private AudioClip kodaFootStepGround2;
    [SerializeField] private AudioClip kodaFootStepGround3;

    [SerializeField] private AudioClip kodaFootStepWater1;
    [SerializeField] private AudioClip kodaFootStepWater2;
    [SerializeField] private AudioClip kodaFootStepWater3;

    private AudioClip[] allKodaJumpSound;
    private AudioClip[] allKodaFootStepGroundSound;
    private AudioClip[] allKodaFootStepWaterSound;

    [SerializeField] private AudioClip kodaGlide;
    [SerializeField] private AudioClip kodaFallGround;
    [SerializeField] private AudioClip kodaFallWater;
    [SerializeField] private AudioClip kodaAbsorption;

    [Header("LEAF")]
    [Space(10)]
    [SerializeField] private AudioClip leafUnfoldGlide;
    [SerializeField] private AudioClip leafThrow;
    [SerializeField] private AudioClip leafVanish;

    [Header("YOKAI")]
    [Space(10)]
    [SerializeField] protected AudioClip yokaiAbsorbed;
    [SerializeField] protected AudioClip yokaiScream;
    [SerializeField] protected AudioClip yokaiHurt;

    [Header("LANTERN")]
    [Space(10)]
    [SerializeField] private AudioClip lanternCatch;
    [SerializeField] private AudioClip lanternFallWater;
    [SerializeField] private AudioClip lanternDome;
    private bool lanterClosestFoundPlay = false;

    [Header("ENVIRONMENT QUICK")]
    [Space(10)]
    [SerializeField] private AudioClip envAirStream;
    [SerializeField] private AudioClip envBumper;
    [SerializeField] private AudioClip envVines;

    [Header("ENVIRONMENT LONG")]
    [Space(10)]
    [SerializeField] private AudioClip envCorruptedWater;
    [SerializeField] private AudioClip envFireWall;
    [SerializeField] private AudioClip envWaterfall;

    [Header("BARE NECECITIES")]
    [Space(10)]
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

    private void Start() {
        allKodaJumpSound = new AudioClip[] { kodaJump1, kodaJump2, kodaJump3 };
        allKodaFootStepGroundSound = new AudioClip[] { kodaFootStepGround1, kodaFootStepGround2, kodaFootStepGround3 };
        allKodaFootStepWaterSound = new AudioClip[] { kodaFootStepWater1, kodaFootStepWater2, kodaFootStepWater3 };
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        if (SceneManager.GetActiveScene().name == "Boss1") {
            StopTheme();
            PlayTheme(themeBossZ1, true);
        }

        if (SceneManager.GetActiveScene().name == "Boss2") {
            StopTheme();
            PlayTheme(themeBossZ2, true);
        }

        if (SceneManager.GetActiveScene().name == "Zone Tuto") {
            StopTheme();
            PlayTheme(themeLightTuto, true);
        }

        if (SceneManager.GetActiveScene().name == "Z1-P1-complete" || SceneManager.GetActiveScene().name == "Z1-P2-complete" || SceneManager.GetActiveScene().name == "Z1-P3-complete") {
            StopTheme();
            PlayTheme(themeDarkZ1, true);
        }

        if (SceneManager.GetActiveScene().name == "Z2-P1-complete" || SceneManager.GetActiveScene().name == "Z2-P2-complete" || SceneManager.GetActiveScene().name == "Z2-P3-complete") {
            StopTheme();
            PlayTheme(themeDarkZ2, true);
        }
    }

    private void PlayTheme(AudioClip clip, bool loop) {
        fxThemeSource.clip = clip;
        fxThemeSource.Play();
        fxThemeSource.loop = loop;
    }

    public void StopTheme() {
        fxThemeSource.Stop();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectHUD(string HUD) {
        switch (HUD) {
            case "MainMenu":
                PlayHUDTheme(HUDMainMenuTheme, true);
                break;

            case "Pause":
                PlayHUDTheme(HUDPauseTheme, true);
                break;

            case "PauseEffect":
                PlayHUDEffect(HUDPauseTheme);
                break;

            default:
                break;
        }        
    }

    private void PlayHUDTheme(AudioClip clip, bool loop) {
        fxHUD_Source.clip = clip;
        fxHUD_Source.Play();
        fxHUD_Source.loop = loop;
    }

    private void PlayHUDEffect(AudioClip clip) {
        fxHUD_Source.clip = clip;
        fxHUD_Source.Play();
    }

    public void StopHUD() {
        fxHUD_Source.Stop();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================
    public void SelectKODA(string KODA) {
        switch (KODA) { 
            case "Jump":
                PlayKodaEffectRandomize(allKodaJumpSound);
                break;

            case "FootStepGround":
                PlayKodaEffectRandomize(allKodaFootStepGroundSound);
                break;

            case "FootStepWater":
                PlayKodaEffectRandomize(allKodaFootStepWaterSound);
                break;

            case "FallGround":
                PlayKodaEffect(kodaFallGround);
                break;

            case "FallWater":
                PlayKodaEffect(kodaFallWater);
                break;

            case "Absorption":
                PlayKodaEffect(kodaAbsorption);
                break;

            case "Glide":
                PlayKodaEffect(kodaGlide);
                break;

            default:
                break;
        }
    }

    private void PlayKodaEffect(AudioClip clip) {
        fxKodaEffect.clip = clip;
        fxKodaEffect.Play();
    }

    private void PlayKodaEffectRandomize(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        fxKodaEffect.pitch = randomPitch;
        fxKodaEffect.clip = clips[randomIndex];
        fxKodaEffect.Play();
    }

    public void StopKoda() {
        fxKodaEffect.Stop();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectLEAF(string LEAF) {
        switch (LEAF) {
            case "UnfoldGlide":
                PlayLeafEffect(leafUnfoldGlide);
                break;

            case "Throw":
                PlayLeafEffect(leafThrow);
                break;

            case "Vanish":
                PlayLeafEffect(leafVanish);
                break;

            default:
                break;
        }
    }

    private void PlayLeafEffect(AudioClip clip) {
        fxLeafEffect.clip = clip;
        fxLeafEffect.Play();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectYOKAI(string YOKAI) {
        switch (YOKAI) {
            case "Hurt":
                PlayYokaiEffect(yokaiHurt);
                break;

            case "Absorbed":
                PlayYokaiEffect(yokaiAbsorbed);
                break;

            case "Scream":
                PlayYokaiEffect(yokaiScream);
                break;

            default:
                break;
        }
    }

    private void PlayYokaiEffect(AudioClip clip) {
        fxYokaiEffect.clip = clip;
        fxYokaiEffect.Play();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectLANTERN(string LANTERN) {
        switch (LANTERN) {
            case "Catch":
                PlayLanternEffect(lanternCatch);
                break;

            case "Dome":
                PlayLanternSource(lanternDome);
                break;

            case "FallWater":
                PlayLanternEffect(lanternFallWater);
                break;

            default:
                break;
        }
    }

    private void PlayLanternEffect(AudioClip clip) {
        fxLanternEffect.clip = clip;
        fxLanternEffect.Play();
    }

    private void PlayLanternSource(AudioClip clip) {
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

    public void AdjustLanternSource(float setVol) {
        fxLanternSource.volume = setVol;
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectENVQuick(string ENV) {
        switch (ENV) {
            case "AirStream":
                PlayEnvironnementEffectQuick(envAirStream);
                break;

            case "Bumper":
                PlayEnvironnementEffectQuick(envBumper);
                break;

            case "Vines":
                PlayEnvironnementEffectQuick(envVines);
                break;

            default:
                break;
        }
    }

    private void PlayEnvironnementEffectQuick(AudioClip clip) {
        fxEnvironnementEffectQuick.clip = clip;
        fxEnvironnementEffectQuick.Play();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectENVLong(string ENV) {
        switch (ENV) {
            case "CorruptedWater":
                PlayEnvironnementEffectLong(envCorruptedWater);
                break;

            case "WaterFall":
                PlayEnvironnementEffectLong(envWaterfall);
                break;

            default:
                break;
        }
    }

    private void PlayEnvironnementEffectLong(AudioClip clip) {
        fxEnvironnementEffectLong.clip = clip;
        fxEnvironnementEffectLong.Play();
    }

    public void StopEnvironnementEffectLong() {
        fxEnvironnementEffectLong.Stop();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================
}
