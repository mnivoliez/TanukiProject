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
    public AudioSource fxYokaiAkashitaEffect;
    public AudioSource fxYokaiBazekoriEffect;
    public AudioSource fxYokaiSesshosekiEffect;
    public AudioSource fxLanternSource;
    public AudioSource fxLanternEffect;
    public AudioSource fxEnvironnementFireflies;
    public AudioSource fxEnvironnementEffectQuick;
    public AudioSource fxEnvironnementEffectLong;

    [Header("THEMES")]
    [Space(10)]
    [SerializeField] private AudioClip themeDarkZ1;
    [SerializeField] private AudioClip themeLightZ1;
    [SerializeField] private AudioClip themePreBoss;
    [SerializeField] private AudioClip themeBossZ1;
    [SerializeField] private AudioClip themeDarkZ2;
    [SerializeField] private AudioClip themeLightZ2;
    [SerializeField] private AudioClip themeBossZ2;

    [Header("HUD")]
    [Space(10)]
    [SerializeField] private AudioClip HUDMainMenuTheme;
    [SerializeField] private AudioClip HUDPauseOpenClose;
    [SerializeField] private AudioClip HUDPauseNavigate;
    [SerializeField] private AudioClip HUDPauseAccept;
    [SerializeField] private AudioClip HUDTutoPictureClose;

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
    [SerializeField] private AudioClip kodaFootStepWater4;

    private AudioClip[] allKodaJumpSound;
    private AudioClip[] allKodaFootStepGroundSound;
    private AudioClip[] allKodaFootStepWaterSound;

    [SerializeField] private AudioClip kodaGlide;
    [SerializeField] private AudioClip kodaFallGround;
    [SerializeField] private AudioClip kodaFallWater;
    [SerializeField] private AudioClip kodaAbsorption;
    [SerializeField] private AudioClip kodaHurt;

    [Header("LEAF")]
    [Space(10)]
    [SerializeField] private AudioClip leafUnfoldGlide;
    [SerializeField] private AudioClip leafThrow;
    [SerializeField] private AudioClip leafVanish;
    [SerializeField] private AudioClip leafCC;

    [Header("YOKAI")]
    [Space(10)]
    [SerializeField] private AudioClip yokaiAbsorbed;
    [SerializeField] private AudioClip yokaiKO;
    [SerializeField] private AudioClip yokaiHurt;

    [Header("YOKAI - AKASHITA")]
    [Space(10)]
    [SerializeField] private AudioClip yokaiAkashitaAttack;

    [Header("YOKAI - BAZEKORI")]
    [Space(10)]
    [SerializeField] private AudioClip yokaiBazekoriAttack;

    [Header("YOKAI - SESSHOSEKI")]
    [Space(10)]
    [SerializeField] private AudioClip yokaiSesshosekiDigIn;
    [SerializeField] private AudioClip yokaiSesshosekiDigOut;

    [Header("LANTERN")]
    [Space(10)]
    [SerializeField] private AudioClip lanternCatch;
    [SerializeField] private AudioClip lanternFallWater;
    [SerializeField] private AudioClip lanternDome;
    public bool lanterClosestFoundPlay = false;

    [Header("FIREFLIES")]
    [Space(10)]
    [SerializeField] private AudioClip envFireflies;

    [Header("ENVIRONMENT QUICK")]
    [Space(10)]
    [SerializeField] private AudioClip envAirStream;
    [SerializeField] private AudioClip envBumper;
    [SerializeField] private AudioClip envPressPlate;
    [SerializeField] private AudioClip envSwitch;
    [SerializeField] private AudioClip envKaki;
    [SerializeField] private AudioClip envVineWallRetracting;
    [SerializeField] private AudioClip envFireWallExtinguished;

    [Header("ENVIRONMENT LONG")]
    [Space(10)]
    [SerializeField] private AudioClip envCorruptedWater;
    [SerializeField] private AudioClip envFireWall;
    [SerializeField] private AudioClip envWind;
    [SerializeField] private AudioClip envWaterFallClose;
    [SerializeField] private AudioClip envWaterFallMedium;
    [SerializeField] private AudioClip envWaterFallFar;

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
        SelectTHEME();
    }

    public void SelectTHEME() {
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            StopTheme();
            StopHUD();
            PlayHUDTheme(HUDMainMenuTheme, true);
        }

        if (SceneManager.GetActiveScene().name == "Boss1" && !Game.playerData.lightBoss1) {
            StopTheme();
            StopHUD();
            StartCoroutine(PlayThemeBoss(themeBossZ1, true));
        }

        if (SceneManager.GetActiveScene().name == "Boss1" && Game.playerData.lightBoss1) {
            StopTheme();
            StopHUD();
            PlayTheme(themeLightZ1, true);
        }

        if (SceneManager.GetActiveScene().name == "Boss2" && !Game.playerData.lightBoss2) {
            StopTheme();
            StopHUD();
            StartCoroutine(PlayThemeBoss(themeBossZ2, true));
        }

        if (SceneManager.GetActiveScene().name == "Boss2" && Game.playerData.lightBoss2) {
            StopTheme();
            StopHUD();
            PlayTheme(themeLightZ2, true);
        }

        if (SceneManager.GetActiveScene().name == "Zone Tuto") {
            StopTheme();
            StopHUD();
            PlayTheme(themeLightZ1, true);
            SelectENVLong("Wind");
        }

        if (SceneManager.GetActiveScene().name == "Z1-P1-complete" || SceneManager.GetActiveScene().name == "Z1-P2-complete" || SceneManager.GetActiveScene().name == "Z1-P3-complete") {
            StopTheme();
            StopHUD();
            PlayTheme(themeDarkZ1, true);
        }

        if ((SceneManager.GetActiveScene().name == "Z1-P1-complete" || SceneManager.GetActiveScene().name == "Z1-P2-complete" || SceneManager.GetActiveScene().name == "Z1-P3-complete") && Game.playerData.lightBoss1) {
            StopTheme();
            StopHUD();
            PlayTheme(themeLightZ1, true);
        }

        if (SceneManager.GetActiveScene().name == "Z2-P1-complete" || SceneManager.GetActiveScene().name == "Z2-P2-complete" || SceneManager.GetActiveScene().name == "Z2-P3-complete") {
            StopTheme();
            StopHUD();
            PlayTheme(themeDarkZ2, true);
        }

        if ((SceneManager.GetActiveScene().name == "Z2-P1-complete" || SceneManager.GetActiveScene().name == "Z2-P2-complete" || SceneManager.GetActiveScene().name == "Z2-P3-complete") && Game.playerData.lightBoss2) {
            StopTheme();
            StopHUD();
            PlayTheme(themeLightZ2, true);
        }
    }

    private void PlayTheme(AudioClip clip, bool loop) {
        fxThemeSource.clip = clip;
        fxThemeSource.Play();
        fxThemeSource.loop = loop;
    }

    private IEnumerator PlayThemeBoss(AudioClip clip, bool loop) {
        fxThemeSource.clip = themePreBoss;
        fxThemeSource.Play();
        yield return new WaitForSeconds(14.0f);
        PlayTheme(clip, loop);
    }

    public IEnumerator FadeOnExitTheme() {
        while (fxThemeSource.volume > 0.0f) {
            fxThemeSource.volume = fxThemeSource.volume - 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
        StopTheme();
        fxThemeSource.volume = 1.0f;
        SelectTHEME();
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

            case "PauseEnabled":
                AdjustAllMusicVolumeLow(0.3f);
                break;

            case "PauseDisabled":
                AdjustAllMusicVolumeHigh(1.0f);
                break;

            case "PauseOpenClose":
                PlayHUDEffect(HUDPauseOpenClose);
                break;

            case "PauseNavigate":
                PlayHUDEffect(HUDPauseNavigate);
                break;

            case "PauseAccept":
                PlayHUDEffect(HUDPauseAccept);
                break;

            case "TutoPictureExit":
                PlayHUDEffect(HUDTutoPictureClose);
                break;

            default:
                break;
        }        
    }

    private void PlayHUDTheme(AudioClip clip, bool loop) {
        fxHUD_Source.clip = clip;
        //fxHUD_Source.volume = 0.2f;
        fxHUD_Source.Play();
        fxHUD_Source.loop = loop;
    }

    private void PlayHUDEffect(AudioClip clip) {
        fxHUD_Effect.clip = clip;
        fxHUD_Effect.Play();
    }

    private void AdjustAllMusicVolumeLow (float volumeLevel) {
        SelectHUD("PauseOpenClose");

        fxThemeSource.volume = volumeLevel;
        fxHUD_Source.volume = volumeLevel;
        fxLanternSource.volume = volumeLevel;

        //fxHUD_Effect.volume = 0.0f;
        fxKodaEffect.volume = 0.0f;
        fxLeafEffect.volume = 0.0f;
        fxYokaiEffect.volume = 0.0f;
        fxLanternEffect.volume = 0.0f;
        fxEnvironnementEffectQuick.volume = 0.0f;
        fxEnvironnementEffectLong.volume = 0.0f;
    }

    private void AdjustAllMusicVolumeHigh(float volumeLevel) {
        fxThemeSource.volume = volumeLevel;
        fxHUD_Source.volume = volumeLevel;
        fxLanternSource.volume = volumeLevel;

        //fxHUD_Effect.volume = 1.0f;
        fxKodaEffect.volume = 1.0f;
        fxLeafEffect.volume = 1.0f;
        fxYokaiEffect.volume = 1.0f;
        fxLanternEffect.volume = 1.0f;
        fxEnvironnementEffectQuick.volume = 1.0f;
        fxEnvironnementEffectLong.volume = 1.0f;

        SelectHUD("PauseOpenClose");
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

            case "Hurt":
                PlayKodaEffect(kodaHurt);
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

            case "CloseCombat":
                PlayLeafEffect(leafCC);
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

            case "KO":
                PlayYokaiEffect(yokaiKO);
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

    public void SelectYOKAIAKASHITA(string YOKAI) {
        switch (YOKAI) {
            case "Attack":
                PlayYokaiAkashitaEffect(yokaiAkashitaAttack);
                break;

            default:
                break;
        }
    }

    private void PlayYokaiAkashitaEffect(AudioClip clip) {
        fxYokaiAkashitaEffect.clip = clip;
        fxYokaiAkashitaEffect.Play();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectYOKAIBAZEKORI(string YOKAI) {
        switch (YOKAI) {
            case "Attack":
                PlayYokaiBazekoriEffect(yokaiBazekoriAttack);
                break;

            default:
                break;
        }
    }

    private void PlayYokaiBazekoriEffect(AudioClip clip) {
        fxYokaiBazekoriEffect.clip = clip;
        fxYokaiBazekoriEffect.Play();
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectYOKAISESSHOSEKI(string YOKAI) {
        switch (YOKAI) {
            case "DigIn":
                PlayYokaiSesshosekiEffect(yokaiSesshosekiDigIn);
                break;

            case "DigOut":
                PlayYokaiSesshosekiEffect(yokaiSesshosekiDigOut);
                break;

            default:
                break;
        }
    }

    private void PlayYokaiSesshosekiEffect(AudioClip clip) {
        fxYokaiSesshosekiEffect.clip = clip;
        fxYokaiSesshosekiEffect.Play();
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
            lanterClosestFoundPlay = false;
        }
    }

    public void AdjustLanternSource(float setVol) {
        fxLanternSource.volume = setVol;
    }

    //===================================================================================================================================================================================
    //===================================================================================================================================================================================

    public void SelectFIREFLIES(string FIREFLIE) {
        switch (FIREFLIE) {
            case "Fireflies":
                PlayFireflieEffect(envFireflies);
                break;

            default:
                break;
        }
    }

    private void PlayFireflieEffect(AudioClip clip) {
        fxEnvironnementFireflies.clip = clip;
        fxEnvironnementFireflies.Play();
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

            case "VineWallRetracting":
                PlayEnvironnementEffectQuick(envVineWallRetracting);
                break;

            case "FireWallExtinguished":
                PlayEnvironnementEffectQuick(envFireWallExtinguished);
                break;

            case "Kaki":
                PlayEnvironnementEffectQuick(envKaki);
                break;

            case "PressPlate":
                PlayEnvironnementEffectQuick(envPressPlate);
                break;

            case "Switch":
                PlayEnvironnementEffectQuick(envSwitch);
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

            case "FireWall":
                PlayEnvironnementEffectLong(envFireWall);
                break;

            case "Wind":
                PlayEnvironnementEffectLong(envWind);
                break;

            case "WaterFallClose":
                PlayEnvironnementEffectLong(envWaterFallClose);
                break;

            case "WaterFallMedium":
                PlayEnvironnementEffectLong(envWaterFallMedium);
                break;

            case "WaterFallFar":
                PlayEnvironnementEffectLong(envWaterFallFar);
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
