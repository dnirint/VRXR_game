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

        Debug.Log($"BPM analysis: BPM is {currentBPM}, so beat intervals will be {beatInterval} seconds per beat.");
        setVolumeForAnalysisMusic(-80);
        //_audioProcessor.onBeat.AddListener(OnEnterBeatInterval);
        playerAudioSource = PlayerController.Instance.player.GetComponent<AudioSource>();
        playerAudioSource.clip = audioSource.clip;
        PlayAudio();
    }



    private void Update()
    {

        playerAudioSource.time = audioSource.time + timeDifferenceWithBeatDetector;
        //Debug.Log($"CURRENT BPM : {currentBPM}");
        if (lastWindowUpdateTime + clipWindowUpdateCooldown < Time.time)
        {
            lastWindowUpdateTime = Time.time;
            StartCoroutine(UpdateBPM());
        }
    }

    public float clipWindowSizeInSeconds = 7f;
    public float clipWindowUpdateCooldown = 1f;
    private float lastWindowUpdateTime = 0;

    IEnumerator UpdateBPM()
    {
        var startTime = Time.time;
        var curAudioTime = audioSource.time;
        var curAudioClip = MakeSubclip(audioSource.clip, curAudioTime, curAudioTime + clipWindowSizeInSeconds);
        var newBPM = UniBpmAnalyzer.AnalyzeBpm(curAudioClip);
        currentBPM = newBPM;
        currentBPS = 60f / currentBPM;
        beatInterval = currentBPS;
        beatCooldown = beatInterval / 4;
        Debug.Log($"New BPM: {newBPM}");
        var elapsed = Time.time - startTime;
        yield return new WaitForSeconds(timeDifferenceWithBeatDetector - elapsed);
        yield return null;
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
        yield return new WaitForSeconds(timeDifferenceWithBeatDetector);
        playerAudioSource.Play();
    }

    private void PlayAudio()
    {
        //_audioProcessor.onBeat.RemoveListener(PlayAudio);
        Debug.Log($"Starting to play {audioSource.clip.name}");
        StartCoroutine(PlayAudioWithDistance());
        timeToFirstBeat = _audioProcessor.audioSource.time;
    }
}