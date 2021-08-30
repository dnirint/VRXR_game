using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject endGameUI;
    public Text endGameTextCounter;
    public Text endGameScore;
    public Text scoreTextLeft;
    public Text scoreTextRight;

    public static UIScript Instance { get; private set; } = null;

    public int curScore { get; private set; } = 0;
    void OnSuccessDrumHit()
    {
        curScore++;
        scoreTextLeft.text = $"Score: {curScore}";
        scoreTextRight.text = $"Score: {curScore}";
        
    }

    private void Start()
    {
        BossToPlayerInteractions.Instance.OnSuccessfulDrumhit.AddListener(OnSuccessDrumHit);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowEndgameUI(int secondsCountdown)
    {
        endGameUI.SetActive(true);
        StartCoroutine(StartCountdownUpdateText(secondsCountdown));


    }

    public IEnumerator StartCountdownUpdateText(int seconds)
    {
        endGameScore.text = $"Total Score: {curScore}";
        Debug.Log($"Waiting for {seconds} seconds before exiting...");
        while (seconds > -0.1)
        {
            endGameTextCounter.text = $"Exiting in {seconds} seconds...";
            yield return new WaitForSeconds(1);
            seconds--;
        }
        yield return null;
    }
}
