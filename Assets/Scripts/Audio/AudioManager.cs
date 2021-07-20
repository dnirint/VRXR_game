using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioProcessor _audioProcessor;
    [SerializeField] public int currentBPM = 60;
    [SerializeField] public float currentBPS = 1f;
    public AudioSource playerAudioSource;
    public float timeToFirstBeat = 0.2f;
    public float beatInterval = 0.5f;
    public float beatCooldown = 0;
    public AudioSource audioSource;
    public UnityEvent OnBeatStart;
    public UnityEvent OnBeatEnd;
    public UnityEvent OnActualBeatStart;
    public UnityEvent OnActualBeatEnd;
    private bool isInsideInterval = false;
    public float timeDifferenceWithBeatDetector { get; private set; } = 3f;
    public AudioMixer audioMixer;
    public static AudioManager Instance { get; private set; } = null;

    public int hardCodedBPM = 92;
    
    
    public bool realTimeAnalysis = true;
    private SpectralFluxAnalyzer analyzer;
    private float[] spectrum;
    private int sampleRate;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBPM = getBPM();
        currentBPS = 60f / currentBPM;
        
        Debug.Log($"BPM analysis: BPM is {currentBPM}, so beat intervals will be {currentBPS} seconds per beat.");
        setVolumeForAnalysisMusic(-80);
        //_audioProcessor.onBeat.AddListener(OnEnterBeatInterval);
        playerAudioSource = PlayerController.Instance.player.GetComponent<AudioSource>();
        playerAudioSource.clip = audioSource.clip;
        SongController.Instance.OnProcessingEnded.AddListener(PlayAudio);
        if (realTimeAnalysis)
        {
            spectrum = new float[1024];
            analyzer = new SpectralFluxAnalyzer();
            sampleRate = AudioSettings.outputSampleRate;
        }
//        else
//        {
//            int indexToPlot = getIndexFromTime(audioSource.time) / 1024;
//            ShootToBeat(analyzer.spectralFluxSamples, indexToPlot);
//        }
        


        PlayAudio();
//        StartCoroutine(UpdateBPM());
    }
    float clipLength;
    int numTotalSamples;

    private int getIndexFromTime(float curTime)
    {
        float lengthPerSample = clipLength / (float) numTotalSamples;

        return Mathf.FloorToInt(curTime / lengthPerSample);
    }
    private void Update()
    {

        playerAudioSource.time = audioSource.time + timeDifferenceWithBeatDetector;
        
        if (realTimeAnalysis)
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            analyzer.analyzeSpectrum(spectrum, audioSource.time);
            ShootToBeat(analyzer.spectralFluxSamples);
        }
        else
        {
            if (playingMusic)
            {
                int indexToPlot = getIndexFromTime(audioSource.time) / 1024;
                ShootToBeat(SongController.Instance.preProcessedSpectralFluxAnalyzer.spectralFluxSamples, indexToPlot);
            }
        }
        
        
        //Debug.Log($"CURRENT BPM : {currentBPM}");
    }
    public int displayWindowSize = 300;

    private void ShootToBeat(List<SpectralFluxInfo> pointInfo, int curIndex = -1)
    {
        int windowStart = 0;
        int windowEnd = 0;

        if (curIndex > 0)
        {
            windowStart = Mathf.Max(0, curIndex - displayWindowSize / 2);
            windowEnd = Mathf.Min(curIndex + displayWindowSize / 2, pointInfo.Count - 1);
        }
        else
        {
            windowStart = Mathf.Max(0, pointInfo.Count - displayWindowSize - 1);
            windowEnd = Mathf.Min(windowStart + displayWindowSize, pointInfo.Count);
        }

        for (int i = windowStart; i < windowEnd; i++)
        {
            if (pointInfo[i].isPeak)
            {
                OnBeatStart.Invoke();
                return;
            }
        }
    }
    public float clipWindowSizeInSeconds = 30f;
    //public float clipWindowUpdateCooldown = 1f;
    private float lastWindowUpdateTime = 0;

    IEnumerator UpdateBPM()
    {
        while (true)
        {
            float elapsed = 0;
            if (audioSource.isPlaying)
            {
                var startTime = Time.time;
                var startQuietAudioTime = audioSource.time;
                var startPlayerAudioTime = playerAudioSource.time;
                var endPlayerAudioTime = startPlayerAudioTime + clipWindowSizeInSeconds;
                var curAudioClip = MakeSubclip(audioSource.clip, startQuietAudioTime, startQuietAudioTime + clipWindowSizeInSeconds);
                var newBPM = UniBpmAnalyzer.AnalyzeBpm(curAudioClip);
                currentBPM = newBPM;
                currentBPS = 60f / currentBPM;
                beatInterval = currentBPS;
                beatCooldown = beatInterval / 4;
                var curPlayerAudioTime = playerAudioSource.time;
                var realDiff = endPlayerAudioTime - curPlayerAudioTime;
                //StartCoroutine(LogAfterSeconds(realDiff, $"New BPM: {newBPM} (calculation time: {realDiff})"));
                yield return null;
            }
            
        }
        
        //yield return new WaitForSeconds(timeDifferenceWithBeatDetector - elapsed);
        //yield return null;
    }

    IEnumerator LogAfterSeconds(float seconds, string logMessage)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log(logMessage);
    }

    private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
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
        return newClip;
    }

    public float TimeToActualBeat()
    {
        return timeDifferenceWithBeatDetector;
    }



    int getBPM()
    {
        return UniBpmAnalyzer.AnalyzeBpm(audioSource.clip);
    }

    void setVolumeForAnalysisMusic(float vol)
    {
        audioMixer.SetFloat("analMusicVol", vol);
    }

    void OnEnterBeatInterval()
    {
        if (!isInsideInterval)
        {
            isInsideInterval = true;
            StartCoroutine(EnterBeatInterval());
            StartCoroutine(EnterActualBeatInterval(timeDifferenceWithBeatDetector));
        }
    }


    IEnumerator EnterActualBeatInterval(float waitBeforeInvokingSeconds)
    {
        yield return new WaitForSeconds(waitBeforeInvokingSeconds);
        Debug.Log($"ACTUAL BEAT!");
        OnActualBeatStart.Invoke();
        yield return new WaitForSeconds(beatCooldown);
        OnActualBeatEnd.Invoke();
    }

    IEnumerator EnterBeatInterval()
    {
        Debug.Log($"BEAT_START");
        OnBeatStart.Invoke();
        yield return new WaitForSeconds(beatCooldown);
        OnBeatEnd.Invoke();
        isInsideInterval = false;
        Debug.Log($"BEAT_END");
    }



    private IEnumerator PlayAudioWithDistance()
    {
        audioSource.Play();
        playingMusic = true;
        yield return new WaitForSeconds(timeDifferenceWithBeatDetector);
        playerAudioSource.Play();
    }


    private bool playingMusic = false;

    private void PlayAudio()
    {
        //_audioProcessor.onBeat.RemoveListener(PlayAudio);
        Debug.Log($"Starting to play {audioSource.clip.name}");
        StartCoroutine(PlayAudioWithDistance());
        timeToFirstBeat = _audioProcessor.audioSource.time;
    }
}