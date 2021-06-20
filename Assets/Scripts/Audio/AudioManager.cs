using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioProcessor _audioProcessor;
    [SerializeField] private int currentBPM = 160;
    public AudioSource playerAudioSource;
    public float timeToFirstBeat = 0.2f;
    public float beatInterval = 0.5f;
    public AudioSource audioSource;
    public UnityEvent OnBeatStart;
    public UnityEvent OnBeatEnd;
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
        //_audioProcessor.onBeat.AddListener(PlayAudio);
        currentBPM = getBPM();
        beatInterval = 60f / currentBPM;
        Debug.Log($"BPM analysis: BPM is {currentBPM}, so beat intervals will be {beatInterval} seconds per beat.");
        setVolumeForAnalysisMusic(-80);
        _audioProcessor.onBeat.AddListener(OnEnterBeatInterval);
        playerAudioSource = PlayerController.Instance.player.GetComponent<AudioSource>();
        playerAudioSource.clip = audioSource.clip;
        PlayAudio();
    }

    private void Update()
    {
        playerAudioSource.time = audioSource.time + timeDifferenceWithBeatDetector;
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
            StartCoroutine(EnterBeatInterval());
        }
    }

    IEnumerator EnterBeatInterval()
    {
        Debug.Log($"BEAT_START");
        isInsideInterval = true;
        OnBeatStart.Invoke();
        yield return new WaitForSeconds(beatInterval);
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