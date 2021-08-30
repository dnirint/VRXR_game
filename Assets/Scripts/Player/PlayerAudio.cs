using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{

    public AudioClip audioClip;
    public AudioClip beep;
    public AudioSource lowAudioSource;
    public AudioLowPassFilter lowPassFilter;
    public AudioSource highAudioSource;
    public AudioHighPassFilter highPassFilter;
    public AudioSource soundEffects;

    public  float FILTER_COMMON_AGREED_THRESHOLD = 3000f;
    public  float POOP_HIGH_PASS_THRESHOLD = 3000f;
    public  float POOP_LOW_PASS_THRESHOLD = 200f;

    public static PlayerAudio Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    void SetPlayerClip(AudioClip newClip)
    {
        lowAudioSource.clip = newClip;
        highAudioSource.clip = newClip;
        soundEffects.volume = 0.1f;
        soundEffects.clip = beep;
    }

    public void PlayBeep()
    {
        soundEffects.time = 0;
        soundEffects.Play();
    }

    void SetFilterParams()
    {
        SetHighPassThreshold(FILTER_COMMON_AGREED_THRESHOLD);
        SetLowPassThreshold(FILTER_COMMON_AGREED_THRESHOLD);

        
    }

    public void SetHighPassThreshold(float threshold=0)
    {
        highPassFilter.cutoffFrequency = threshold;
    }
    public void SetLowPassThreshold(float threshold=22000)
    {
        lowPassFilter.cutoffFrequency = threshold;
    }

    public void StartAudibleMusic()
    {
        lowAudioSource.Play();
        highAudioSource.Play();
    }

    void Start()
    {
        SetPlayerClip(audioClip);
        SetFilterParams();
        beatDuration = TimeSignatureController.Instance.timeBetweenBeats / 2;
        TimeSignatureController.Instance.CriticalBeatEnd.AddListener(PoopBeat);
    }

    private float beatDuration;
    public bool shouldPoopNextBeat = true;
    private bool shouldUnpoop = true;
    private float nextUnpoopTime;

    void Update()
    {

        if (shouldUnpoop && Time.time > nextUnpoopTime)
        {
            UnpoopBeat();
        }
    }


    private void PoopBeat()
    {
        if (!shouldPoopNextBeat)
        {
            return;
        }
        beatDuration = TimeSignatureController.Instance.timeBetweenBeats * 0.7f;
        nextUnpoopTime = Time.time + beatDuration;
        shouldUnpoop = true;
        SetHighPassThreshold(POOP_HIGH_PASS_THRESHOLD);
        SetLowPassThreshold(POOP_LOW_PASS_THRESHOLD);
    }

    private void UnpoopBeat()
    {
        shouldUnpoop = false;
        shouldPoopNextBeat = true;
        SetFilterParams();
    }


}
