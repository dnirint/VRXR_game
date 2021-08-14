using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeSignatureController : MonoBehaviour
{

    private AudioClip audioClip;

    public UnityEvent CriticalBeatStart;
    public UnityEvent CriticalBeatEnd;
    public UnityEvent BeatStart;
    public UnityEvent BeatEnd;

    public float averageBPM;
    public float averageBPS;
    public static TimeSignatureController Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        audioClip = PlayerAudio.Instance.audioClip;
        StartCoroutine(PreprocessAndStartMusic());
    }

    private void Update()
    {
        //if (playerAudioSource.isPlaying)
        //{
        //    playerAudioSource.time = audioSource.time + audioPlayerTimeOffset;
        //}
    }

    IEnumerator PreprocessAndStartMusic()
    {
        float preprocessStartTime = Time.time;
        averageBPM = UniBpmAnalyzer.AnalyzeBpm(audioClip);
        //averageBPM = 120;
        if (averageBPM > 100)
        {
            //averageBPM = averageBPM / 2;
        }
        //averageBPM = 86f;
        averageBPS = averageBPM / 60;
        float preprocessEndTime = Time.time;
        float totalPreprocessTime = preprocessEndTime - preprocessStartTime;
        Debug.Log($"BPM: {averageBPM}. Total time it took to preprocess = {totalPreprocessTime}");

        

        StartCoroutine(StartTrackAndTrackSignature());
        //yield return new WaitForSeconds(audioPlayerTimeOffset);
        PlayerAudio.Instance.StartAudibleMusic();

        yield return null;
    }


    public bool isTrackingTimeSignature = false;
    public int beatsPerBar = 3;
    public float nextBeatTime;
    public int beatCounter = 0;
    public int criticalBeatNumber = 1;
    public float timeBetweenBeats = 0;
    public float beatDurationForPlayer = 0.05f;
    public float nextBeatDurationForPlayer = float.PositiveInfinity;
    IEnumerator StartTrackAndTrackSignature()
    {
        isTrackingTimeSignature = true;
        //audioSource.Play();
        nextBeatTime = Time.time;
        timeBetweenBeats = 1 / averageBPS;
        while (isTrackingTimeSignature)
        {
            if (Time.time >= nextBeatDurationForPlayer)
            {
                if (beatCounter == criticalBeatNumber)
                {
                    CriticalBeatStart.Invoke();
                }
                else
                {
                    BeatStart.Invoke();
                }
                nextBeatDurationForPlayer = float.PositiveInfinity;
            }
            if (Time.time >= nextBeatTime)
            {
                if (beatCounter == criticalBeatNumber)
                {
                    CriticalBeatEnd.Invoke();
                }
                else
                {
                    BeatEnd.Invoke();
                }
                beatCounter = (beatCounter + 1) % beatsPerBar;
                nextBeatTime += timeBetweenBeats;
                nextBeatDurationForPlayer += nextBeatTime - nextBeatDurationForPlayer;
            }
            yield return null;
            
        }
        yield return null;
    }

}
