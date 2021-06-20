using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject boss = null;
    public static BossController Instance { get; private set; } = null;
    private Transform player;
    private bool followPlayer = false;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        player = PlayerController.Instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            boss.transform.LookAt(player);
        }
        else
        {
            boss.transform.LookAt(BossToPlayerInteractions.Instance.GetCurrentTarget().transform);
        }
    }
}
