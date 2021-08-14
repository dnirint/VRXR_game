using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossToPlayerInteractions : MonoBehaviour
{
    public static BossToPlayerInteractions Instance { get; private set; } = null;

    public GameObject projectilePrefab;
    public GameObject[] bossTargets;
    public List<List<GameObject>> platformTargets;
    public Transform projectileParent;

    public bool isAttacking = true;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;
    private GameObject boss;

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
    private Queue<GameObject> nextTargetQueue;
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
        nextTargetQueue = new Queue<GameObject>();
        platformTargets = new List<List<GameObject>>();
        foreach (var platform in bossTargets)
        {
            platformTargets.Add(platform.GetComponent<PlatformTargets>().targetList);
        }

        SetTargetQueues();
        TimeSignatureController.Instance.CriticalBeatEnd.AddListener(AttackTarget);
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
        //attackCooldown = TimeSignatureController.Instance.averageBPS;
        foreach (var platform in platformTargets)
        {
            foreach (var target in platform)
            {
                Debug.DrawLine(boss.transform.position, target.transform.position);
            }
        }
    }

    public float projectileFlightTime = 3f;
    void AttackTarget()
    {
        if (isAttacking && lastAttackTime + attackCooldown < Time.time)
            //if (isAttacking)
        {
            lastAttackTime = Time.time;
            int startTargetIndex = curTargetIndex;
            Debug.Log($"Targeting {curTargetIndex}/{bossTargets.Length}");
            var newProjectileGO = Instantiate(projectilePrefab, projectileParent);
            Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
            newProjectile.timeToTarget = projectileFlightTime;
            newProjectile.origin = boss.transform.position;
            int newTarget = GetRandomTargetInCluster(curTargetIndex);
            Debug.Log($"Targeting drum {newTarget} in platform {curTargetIndex}");
            var targetGO = platformTargets[startTargetIndex][newTarget];
            newProjectile.targetGO = targetGO;
            drumToProjectileQueue[targetGO].Enqueue(newProjectile);
            //targetQueues[startTargetIndex][newTarget].Add(newProjectile);
            newProjectile.TargetHit.AddListener(() => { OnTargetHit(newProjectile); });
            nextTargetQueue.Enqueue(targetGO);
        }
    }


    void DestroyProjectileInQueue(GameObject target)
    {
        if (drumToProjectileQueue[target].Count > 0)
        {
            var projectile = drumToProjectileQueue[target].Dequeue();
            Destroy(projectile.gameObject);
            nextTargetQueue.Dequeue();
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
        if (nextTargetQueue.Count > 0 && nextTargetQueue.Peek() != target)
        {
            return;
        }
        // check distance from target
        DestroyProjectileInQueue(target);
        PlayerAudio.Instance.shouldPoopNextBeat = false;
    }



    IEnumerator SwitchTargets()
    {
        while (shouldSwitchTargets)
        {
            if (isAttacking)
            {
                SetNewPlatform();
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