using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BossToPlayerInteractions : MonoBehaviour
{
    public static BossToPlayerInteractions Instance { get; private set; } = null;

    public GameObject projectileExplosionPrefab;
    public GameObject[] projectilePrefabs;
    public GameObject projectilePrefab;
    public GameObject[] bossTargets;
    public List<List<GameObject>> platformTargets;
    public Transform projectileParent;

    public bool isAttacking = true;
    public float attackCooldown = 0.25f;
    private float lastAttackTime = 0f;
    private GameObject boss;


    private float switchTargetAttackCooldown = 3;
    private float timeBetweenSwitches = 10;
    private bool shouldSwitchTargets = true;

    private int curTargetIndex = 0;
    private Dictionary<GameObject, Queue<Projectile>> drumToProjectileQueue;

    private Queue<Projectile> projectileOrder;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public GameObject GetCurrentTarget()
    {
        return bossTargets[curTargetIndex];
    }

    void Start()
    {
        lastAttackTime = Time.time;
        projectileOrder = new Queue<Projectile>();
        boss = BossController.Instance.boss;
        //TODO: Move this from start, should be handled by a game manager.
        StartCoroutine(SwitchTargets());
        StartCoroutine(ClosestProjectileInteractionHandler());
        //        AudioManager.Instance.OnBeatStart.AddListener(AttackTarget);
        TimeSignatureController.Instance.CriticalBeatEnd.AddListener(AttackTarget);
        platformTargets = new List<List<GameObject>>();
        foreach (var platform in bossTargets)
        {
            platformTargets.Add(platform.GetComponent<PlatformTargets>().targetList);
        }

        SetTargetQueues();
        
    }

    private void SetTargetQueues()
    {
        drumToProjectileQueue = new Dictionary<GameObject, Queue<Projectile>>();
        foreach (var platform in platformTargets)
        {
            foreach (var target in platform)
            {
                drumToProjectileQueue[target] = new Queue<Projectile>();
            }
        }
    }


    private int specificTargetInPlatformIndex = 0;

    GameObject GetSpecificTarget()
    {
        var platform = platformTargets[curTargetIndex];
        int ind = (specificTargetInPlatformIndex++) % platform.Count;
        return platform[ind];
    }

    private int GetRandomTargetInCluster(int platformIndex)
    {
        var platform = platformTargets[platformIndex];
        int totalweight = 0;
        int[] weights = new int[platform.Count];
        for (int i = 0; i < platform.Count; i++)
        {
            var drum = platform[i].GetComponent<BattleDrum>();
            if (drum != null)
            {
                int priority = drum.priority;
                totalweight += priority;
                weights[i] = priority;
            }
        }

        float randomVal = Random.value;
        float startVal = 0;
        for (int i = 0; i < platform.Count; i++)
        {
            startVal += (float) weights[i] / totalweight;
            if (startVal >= randomVal)
            {
                return i;
            }
        }

        return 0;
    }


    void Update()
    {
//        attackCooldown = AudioManager.Instance.currentBPS;
//        AttackTarget();

        foreach (var platform in platformTargets)
        {
            foreach (var target in platform)
            {
                Debug.DrawLine(boss.transform.position, target.transform.position);
            }
        }
    }

    private bool alternatingAttackFlag = false;
    void AttackTarget()
    {
        alternatingAttackFlag = !alternatingAttackFlag;
        if (alternatingAttackFlag)
        {
            return;
        }
        
        if (isAttacking && lastAttackTime + attackCooldown < Time.time)
//            if (isAttacking)
        {
            lastAttackTime = Time.time;
            int startTargetIndex = curTargetIndex;
            Debug.Log($"Targeting {curTargetIndex}/{bossTargets.Length}");
//            int newTarget = GetRandomTargetInCluster(curTargetIndex);
            int newTarget = Random.Range(0, 4);
            Debug.Log($"Targeting drum {newTarget} in platform {curTargetIndex}");
            var newProjectileGO = Instantiate(projectilePrefabs[newTarget], projectileParent);

//            var newProjectileGO = Instantiate(projectilePrefab, projectileParent);

            Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
//            newProjectile.timeToTarget = AudioManager.Instance.TimeToActualBeat();
            newProjectile.origin = boss.transform.position;
            newProjectile.totalFlightTime = TimeSignatureController.Instance.AudioTimeOffset;
            var targetGO = platformTargets[startTargetIndex][newTarget];
            newProjectile.targetGO = targetGO;
            drumToProjectileQueue[targetGO].Enqueue(newProjectile);
            projectileOrder.Enqueue(newProjectile);
            //targetQueues[startTargetIndex][newTarget].Add(newProjectile);
            newProjectile.TargetHit.AddListener(() => { OnTargetHit(newProjectile); });
        }
    }

    
    void UpdateCurrentClosestProjectile()
    {
        if (projectileOrder.Count > 0)
        {
            var oldClosest = projectileOrder.Dequeue();
            oldClosest.targetGO.GetComponent<BattleDrum>().SetInteractable(false);
        }
    }

    private Projectile m_closestProjectile;
    private BattleDrum m_closestBattleDrum;
    IEnumerator ClosestProjectileInteractionHandler()
    {
        while (true)
        {
            if (projectileOrder.Count > 0)
            {
                var nextClosestProjectile = projectileOrder.Peek();
                if (nextClosestProjectile != m_closestProjectile)
                {
                    m_closestProjectile = nextClosestProjectile;
                    m_closestBattleDrum = m_closestProjectile.targetGO.GetComponent<BattleDrum>();
                }
                if (nextClosestProjectile.timeToTarget <= TimeSignatureController.Instance.preBeatTime && !m_closestBattleDrum.isInteractable)
                {
                    nextClosestProjectile.targetGO.GetComponent<BattleDrum>().SetInteractable(true);
                }

            }
            yield return null;
        }
        yield return null;

    }

    void DestroyProjectileInQueue(GameObject target, bool explosion=false)
    {
        UpdateCurrentClosestProjectile();
        if (drumToProjectileQueue[target].Count > 0)
        {
            var projectile = drumToProjectileQueue[target].Dequeue();
            if (explosion)
            {
                Instantiate(projectileExplosionPrefab, projectile.transform.position, projectile.transform.rotation);
            }
            Destroy(projectile.gameObject);
        }
        else
        {
            // bzzt
        }
    }

    void OnTargetHit(Projectile projectile)
    {
        DestroyProjectileInQueue(projectile.targetGO);
    }

    public void DestroyClosestProjectileOnSameLane(GameObject target)
    {
        var closestProjectile = projectileOrder.Peek();
        if (closestProjectile.targetGO != target)
        {
            return;
        }
        DestroyProjectileInQueue(target, explosion: true);
    }

    public UnityEvent SwitchedTargets;
    IEnumerator SwitchTargets()
    {
        curTargetIndex = 0;
        yield return new WaitForSeconds(timeBetweenSwitches);
        while (shouldSwitchTargets)
        {
            if (isAttacking)
            {
                isAttacking = false;
                SetNewPlatform();
                yield return new WaitForSeconds(switchTargetAttackCooldown);
                isAttacking = true;
            }

            yield return new WaitForSeconds(timeBetweenSwitches);
        }
    }

    void SetNewPlatform()
    {
        curTargetIndex = (curTargetIndex + 1) % platformTargets.Count;
        //int new_target_index = Random.Range(0, bossTargets.Length);
        //while (curTargetIndex == new_target_index)
        //{
        //    new_target_index = Random.Range(0, bossTargets.Length);
        //}

        //curTargetIndex = new_target_index;
        specificTargetInPlatformIndex = 0;
    }
}