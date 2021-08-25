using System;
using System.Linq;
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
    public float AudioTimeOffset = 3;
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
        averageBPS = averageBPM / 60;
        //ComputeWindowSpeedModifiers();
        ComputeWindowSpeedModifiersAuto();
        float preprocessEndTime = Time.time;
        float totalPreprocessTime = preprocessEndTime - preprocessStartTime;
        Debug.Log($"BPM: {averageBPM}. Total time it took to preprocess = {totalPreprocessTime}");
        StartCoroutine(StartTrackAndTrackSignature());
        yield return new WaitForSeconds(AudioTimeOffset);
        PlayerAudio.Instance.StartAudibleMusic();

        yield return null;
    }

    float windowSize = 1f;

    void AddModifier(float start, float end, int modifier)
    {
        windowSpeedModifiers.Add(new Tuple<float, float, int>(start, end, modifier));
    }


    void ComputeWindowSpeedModifiersAuto()
    {
        windowSpeedModifiers = new List<Tuple<float, float, int>>();
        var unnormalizedWindowSpeedModifiers = new List<Tuple<float, float, float>>();
        float totalAvg = 0;
        for (float i = 0; i < audioClip.length;) // i is the window number
        {
            float windowStart = i;
            float windowEnd = i + windowSize;
            i = windowEnd;
            //var subclipData = MakeSubclip(audioClip, windowStart, windowEnd);
            var subclipData = MakeSubclip(audioClip, windowStart, windowEnd);
            AudioClip subclip = subclipData.Item1;
            float[] samples = subclipData.Item2;
            float BPM = UniBpmAnalyzer.AnalyzeBpm(subclip);
            float BPS = BPM / 60;
            float sum = 0;
            for (int si = 0; si < samples.Length / 4; si++)
            {
                sum += Mathf.Abs(samples[si]);
            }
            totalAvg += sum;

            unnormalizedWindowSpeedModifiers.Add(new Tuple<float, float, float>(windowStart, windowEnd, sum));
        }
        totalAvg /= unnormalizedWindowSpeedModifiers.Count;
        float lastSpeedModifier = 0;
        for (int i = 0; i < unnormalizedWindowSpeedModifiers.Count - 1; i++)
        {
            var d = unnormalizedWindowSpeedModifiers[i];
            var curAvgRatio = d.Item3 / totalAvg;
            var next_d = unnormalizedWindowSpeedModifiers[i + 1];
            var nextAvgRatio = next_d.Item3 / totalAvg;
            int curSpeedModifier = 1;
            if (curAvgRatio < 0.5 && nextAvgRatio < 0.5)
            {
                curSpeedModifier = 0;
            }
            else if (lastSpeedModifier < 2 && curAvgRatio < 1 && nextAvgRatio > 1) // music starting to speed up after slow part
            {
                curSpeedModifier = 2;
            }
            else if (lastSpeedModifier < 2 && curAvgRatio > 1 && nextAvgRatio > 1.1) // start speed up (faster) after slow part
            {
                curSpeedModifier = 2;
            }
            else if (lastSpeedModifier > 1 && curAvgRatio > 1.1 && nextAvgRatio <= 1.1) // start slow down after medium-fast part
            {
                curSpeedModifier = 2;
            }
            else if (curAvgRatio <= 1 && nextAvgRatio <= 1) // slow/regular pace
            {
                curSpeedModifier = 1;
            }
            else if (lastSpeedModifier > 1 && curAvgRatio >= 1.1 && nextAvgRatio >= 1.1) // super fast music
            {
                curSpeedModifier = 4;
            }
            Debug.Log($"Speed modifier [{d.Item1}:{d.Item2}] is {curSpeedModifier} caused by {curAvgRatio} -> {nextAvgRatio} avg ({totalAvg})");
            windowSpeedModifiers.Add(new Tuple<float, float, int>(d.Item1, d.Item2, curSpeedModifier));
            lastSpeedModifier = curSpeedModifier;

        }
    }



    private Tuple<AudioClip, float[]> MakeSubclip(AudioClip clip, float start, float stop)
    {
        /* Create a new audio clip */
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);
        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];
        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        /* Return the sub clip */
        return new Tuple<AudioClip, float[]>(newClip, data);
    }

    List<Tuple<float, float, int>> windowSpeedModifiers;
    public bool isTrackingTimeSignature = false;
    public int beatsPerBar = 4;
    public float nextBeatTime;
    public int beatCounter = 0;
    public int criticalBeatNumber = 1;
    public float timeBetweenBeats = 0;
    public float beatDurationForPlayer = 0.05f;
    public float nextBeatDurationForPlayer = float.PositiveInfinity;
    public int currentTimeSpeedModifier = 1;
    public int currentSpeedModifierWindow = 0;
    public float preBeatTime = 0.05f;
    public bool isQuietPart = true;

    private int initial_beatsPerBar;
    private int initial_criticalBeatNumber;
    private float initial_timeBetweenBeats;


    IEnumerator StartTrackAndTrackSignature()
    {
        isTrackingTimeSignature = true;
        nextBeatTime = Time.time;
        timeBetweenBeats = 1 / averageBPS;

        initial_beatsPerBar = beatsPerBar;
        initial_criticalBeatNumber = criticalBeatNumber;
        initial_timeBetweenBeats = timeBetweenBeats;



        while (isTrackingTimeSignature)
        {

            if (Time.time >= nextBeatDurationForPlayer) // this signals that a beat is starting (pre beat period)
            {
                //if (beatCounter == criticalBeatNumber)
                if (beatCounter % initial_beatsPerBar == criticalBeatNumber)
                {
                    if (!isQuietPart)
                    {
                        
                        CriticalBeatStart.Invoke();
                    }
                }
                else
                {
                    if (!isQuietPart)
                    {
                        BeatStart.Invoke();
                    }
                }
                nextBeatDurationForPlayer = float.PositiveInfinity; // only a beat can specify when the next beat starts
            }
            if (Time.time >= nextBeatTime) // this signals that a beat ended (on-beat signal)
            {
                //if (beatCounter == criticalBeatNumber)
                if (beatCounter % initial_beatsPerBar == criticalBeatNumber)
                {
                    if (!isQuietPart)
                    {
                        CriticalBeatEnd.Invoke();
                    }
                }
                else
                {
                    if (!isQuietPart)
                    {
                        BeatEnd.Invoke();
                    }
                }
                beatCounter = (beatCounter + 1) % beatsPerBar;
                nextBeatTime += timeBetweenBeats; // prepare next on-beat time
                nextBeatDurationForPlayer += nextBeatTime - preBeatTime; // prepare next time that a beat starts
            }
            if ((beatCounter + 1) % beatsPerBar == 0) // compute new time signature based on current tempo (we do it before the start of a bar to update the bar's structure)
            {
                int curTimeSecond = (int)Math.Truncate((Decimal)Time.time);
                var curWindow = windowSpeedModifiers[currentSpeedModifierWindow];
                while (curTimeSecond > curWindow.Item1)
                {
                    currentSpeedModifierWindow++;
                    curWindow = windowSpeedModifiers[currentSpeedModifierWindow];
                }
                currentTimeSpeedModifier = curWindow.Item3; // can be 0, 1, 2 or 4
                isQuietPart = currentTimeSpeedModifier == 0;
                Debug.Log($"Current speed modifier is {currentTimeSpeedModifier} for window: {currentSpeedModifierWindow}");
                beatsPerBar = (isQuietPart) ? initial_beatsPerBar : initial_beatsPerBar * currentTimeSpeedModifier;
                //criticalBeatNumber = initial_criticalBeatNumber * currentTimeSpeedModifier;
                timeBetweenBeats = (isQuietPart) ? initial_timeBetweenBeats : initial_timeBetweenBeats / currentTimeSpeedModifier;
            }
            yield return null;

        }
        yield return null;
    }

    void ComputeWindowSpeedModifiers()
    {
        windowSpeedModifiers = new List<Tuple<float, float, int>>();

        AddModifier(0, 17, 1);
        AddModifier(17, 18, 2);
        AddModifier(18, 29, 1);
        AddModifier(29, 30, 2);
        AddModifier(30, 41, 1);
        AddModifier(41, 42, 2);
        AddModifier(41, 42, 2);
        AddModifier(41, 53, 1);
        AddModifier(53, 54, 2);
        AddModifier(54, 59, 4);
        AddModifier(59, 60, 1);
        AddModifier(60, 64, 4);
        AddModifier(64, 67, 4);
        AddModifier(67, 79, 4);
        AddModifier(79, 89, 2);
        AddModifier(89, 90, 4);

        for (int i = 0; i < windowSpeedModifiers.Count; i++)
        {
            var p = windowSpeedModifiers[i];
            Debug.Log($"-- modifier window {p.Item1}:{p.Item2} is {p.Item3}");
        }
    }



}
