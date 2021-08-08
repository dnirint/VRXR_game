using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeSignatureController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource playerAudioSource;

    public UnityEvent CriticalBeat;
    public UnityEvent BarEnd;
    public UnityEvent Beat;

    public float averageBPM;
    public float averageBPS;
    public float audioPlayerTimeOffset = 0.05f;
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
        playerAudioSource = PlayerController.Instance.player.GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        playerAudioSource.clip = audioSource.clip;
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
        averageBPM = UniBpmAnalyzer.AnalyzeBpm(audioSource.clip);
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
        playerAudioSource.Play();

        yield return null;
    }


    public bool isTrackingTimeSignature = false;
    public int beatsPerBar = 3;
    public float nextBeatTime;
    public int beatCounter = 0;
    public int criticalBeatNumber = 0;
    public float timeBetweenBeats = 0;
    IEnumerator StartTrackAndTrackSignature()
    {
        isTrackingTimeSignature = true;
        //audioSource.Play();
        nextBeatTime = Time.time;
        timeBetweenBeats = 1 / averageBPS;
        while (isTrackingTimeSignature)
        {
            if (Time.time >= nextBeatTime)
            {
                if (beatCounter == criticalBeatNumber)
                {
                    CriticalBeat.Invoke();
                }
                else
                {
                    Beat.Invoke();
                }
                beatCounter = (beatCounter + 1) % beatsPerBar;
                nextBeatTime += timeBetweenBeats;
            }
            yield return null;
            
        }
        yield return null;
    }

    //IEnumerator UpdateBPM()
    //{
    //    while (true)
    //    {
    //        float elapsed = 0;
    //        if (audioSource.isPlaying)
    //        {
    //            var startTime = Time.time;
    //            var startQuietAudioTime = audioSource.time;
    //            var startPlayerAudioTime = playerAudioSource.time;
    //            var endPlayerAudioTime = startPlayerAudioTime + clipWindowSizeInSeconds;
    //            var curAudioClip = MakeSubclip(audioSource.clip, startQuietAudioTime, startQuietAudioTime + clipWindowSizeInSeconds);
    //            var newBPM = UniBpmAnalyzer.AnalyzeBpm(curAudioClip);
    //            currentBPM = newBPM;
    //            currentBPS = 60f / currentBPM;
    //            beatInterval = currentBPS;
    //            beatCooldown = beatInterval / 4;
    //            var curPlayerAudioTime = playerAudioSource.time;
    //            var realDiff = endPlayerAudioTime - curPlayerAudioTime;
    //            //StartCoroutine(LogAfterSeconds(realDiff, $"New BPM: {newBPM} (calculation time: {realDiff})"));
    //            yield return null;
    //        }

    //    }
}
