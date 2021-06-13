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
    public float timeToFirstBeat = 0.2f;
    public float beatInterval = 0.5f;
    public AudioSource audioSource;
    public UnityEvent OnBeatStart;
    public UnityEvent OnBeatEnd;
    private bool isInsideInterval = false;

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
        setVolumeForAnalysisMusic(-80);
        _audioProcessor.onBeat.AddListener(OnEnterBeatInterval);
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

    IEnumerator PlayAudioFromBeat()
    {
        Debug.Log("Playing Audio from first beat");
        yield return new WaitForSeconds(timeToFirstBeat);
        audioSource.Play();
    }

    private void PlayAudio()
    {
        _audioProcessor.onBeat.RemoveListener(PlayAudio);
        timeToFirstBeat = _audioProcessor.audioSource.time;
        StartCoroutine(PlayAudioFromBeat());
    }
}