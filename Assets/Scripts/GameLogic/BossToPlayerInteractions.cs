using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossToPlayerInteractions : MonoBehaviour
{
    public static BossToPlayerInteractions Instance { get; private set; } = null;


    public GameObject[] projectilePrefabs;
    public GameObject projectilePrefab;
    public GameObject[] bossTargets;
    public List<List<GameObject>> platformTargets;
    public Transform projectileParent;

    public bool isAttacking = true;
    public float attackCooldown = 0.25f;
    private float lastAttackTime = 0f;
    private GameObject boss;


    private float switchTargetAttackCooldown = 1;
    private float timeBetweenSwitches = 10;
    private bool shouldSwitchTargets = true;

    private int curTargetIndex = 0;
    /*
     * We have 3 platforms
     *  each platform has 4 drums
     *      each drum is a target
     * 
     * We need a list of plaforms and for each platform we will hold a list of targets
     * List<List<GameObject>>  -> target pointers
     * 
     * For each target, we need to hold a queue of projectiles, so:
     * List<List<Queue<GameObject>>> -> projectiles
     * 
     * HashSet<GameObject, Queue<Projectile>>
     * 
     */

    //private List<List<HashSet<Projectile>>> targetQueues;
    private Dictionary<GameObject, Queue<Projectile>> drumToProjectileQueue;
    // list of list of hashsets

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

        boss = BossController.Instance.boss;
        //TODO: Move this from start, should be handled by a game manager.
        StartCoroutine(SwitchTargets());
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

        //targetQueues = new List<List<HashSet<Projectile>>>();
        //// for each group of drums
        //foreach (var cluster in platformTargets)
        //{
        //    List<HashSet<Projectile>> clusterList = new List<HashSet<Projectile>>();
        //    // for each drum in the group
        //    foreach (var target in cluster)
        //    {
        //        // create a hashset for that drums lane
        //        clusterList.Add(new HashSet<Projectile>());
        //    }

        //    // Add the list of hash sets
        //    targetQueues.Add(clusterList);
        //}
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


    void AttackTarget()
    {
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
            newProjectile.timeToTarget = TimeSignatureController.Instance.AudioTimeOffset;
            var targetGO = platformTargets[startTargetIndex][newTarget];
            newProjectile.targetGO = targetGO;
            drumToProjectileQueue[targetGO].Enqueue(newProjectile);
            //targetQueues[startTargetIndex][newTarget].Add(newProjectile);
            newProjectile.TargetHit.AddListener(() => { OnTargetHit(newProjectile); });
        }
    }


    void DestroyProjectileInQueue(GameObject target)
    {
        if (drumToProjectileQueue[target].Count > 0)
        {
            var projectile = drumToProjectileQueue[target].Dequeue();

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
        DestroyProjectileInQueue(target);
    }


    IEnumerator SwitchTargets()
    {
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
        int new_target_index = Random.Range(0, bossTargets.Length);
        while (curTargetIndex == new_target_index)
        {
            new_target_index = Random.Range(0, bossTargets.Length);
        }

        curTargetIndex = new_target_index;
        specificTargetInPlatformIndex = 0;
    }
}