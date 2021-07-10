using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossToPlayerInteractions : MonoBehaviour
{
    public static BossToPlayerInteractions Instance { get; private set; } = null;

    public GameObject projectilePrefab;
    public GameObject[] bossTargets;
    public List<List<GameObject>> bossTargetClusters;
    public Transform projectileParent;

    public bool isAttacking = true;
    public float attackCooldown = 5f;
    private float lastAttackTime = 0f;
    private GameObject boss;

    private float timeBetweenSwitches = 10;
    private bool shouldSwitchTargets = true;
    private int curTargetIndex = 0;


    private List<List<HashSet<Projectile>>> targetQueues;
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
        AudioManager.Instance.OnBeatStart.AddListener(AttackTarget);
        bossTargetClusters = new List<List<GameObject>>();
        foreach (var target in bossTargets)
        {
            bossTargetClusters.Add(target.GetComponent<PlatformTargets>().targetList);
        }

        SetTargetQueues();
    }

    private void SetTargetQueues()
    {
        targetQueues = new List<List<HashSet<Projectile>>>();
        // for each group of drums
        foreach (var cluster in bossTargetClusters)
        {
            List<HashSet<Projectile>> clusterList = new List<HashSet<Projectile>>();
            // for each drum in the group
            foreach (var target in cluster)
            {
                // create a hashset for that drums lane
                clusterList.Add(new HashSet<Projectile>());
            }

            // Add the list of hash sets
            targetQueues.Add(clusterList);
        }
    }


    private int specificTargetInPlatformIndex = 0;

    GameObject GetSpecificTarget()
    {
        var platform = bossTargetClusters[curTargetIndex];
        int ind = (specificTargetInPlatformIndex++) % platform.Count;
        return platform[ind];
    }

    private int GetRandomTargetInCluster(int platformIndex)
    {
        var platform = bossTargetClusters[platformIndex];
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
        attackCooldown = AudioManager.Instance.currentBPS;
//        if (isAttacking && lastAttackTime + attackCooldown < Time.time)
//        //if (isAttacking)
//        {
//            lastAttackTime = Time.time;
//            int startTargetIndex = curTargetIndex;
//            Debug.Log($"Targeting {curTargetIndex}/{bossTargets.Length}");
//            var newProjectileGO = Instantiate(projectilePrefab, projectileParent);
//            Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
//            newProjectile.timeToTarget = AudioManager.Instance.TimeToActualBeat();
//            newProjectile.origin = boss.transform.position;
//            newProjectile.targetGO = GetSpecificTarget();
//            
//            targetQueues[startTargetIndex].Add(newProjectile);
//            newProjectile.TargetHit.AddListener(() => { OnTargetHit(startTargetIndex, newProjectile); });
//        }
        AttackTarget();
        foreach (var platform in bossTargetClusters)
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
            //if (isAttacking)
        {
            lastAttackTime = Time.time;
            int startTargetIndex = curTargetIndex;
            Debug.Log($"Targeting {curTargetIndex}/{bossTargets.Length}");
            var newProjectileGO = Instantiate(projectilePrefab, projectileParent);
            Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
            newProjectile.timeToTarget = AudioManager.Instance.TimeToActualBeat();
            newProjectile.origin = boss.transform.position;
            int newTarget = GetRandomTargetInCluster(curTargetIndex);
            Debug.Log($"Targeting drum {newTarget} in platform {curTargetIndex}");
            newProjectile.targetGO = bossTargetClusters[startTargetIndex][newTarget];
            targetQueues[startTargetIndex][newTarget].Add(newProjectile);
            newProjectile.TargetHit.AddListener(() => { OnTargetHit(startTargetIndex, newTarget, newProjectile); });
        }
    }


    void OnTargetHit(int targetCluster, int targetDrumIndex, Projectile projectile)
    {
        targetQueues[targetCluster][targetDrumIndex].Remove(projectile);
        Debug.Log($"Cluster {targetCluster} in target {targetDrumIndex} was hit!");
        Destroy(projectile.gameObject);
    }

    public void DestroyClosestProjectileOnSameLane(GameObject target)
    {
        Debug.Log($"KILLING");
        // In each Cluster
        for (int i = 0; i < bossTargetClusters.Count; i++)
        {
            // In each drum of that cluster
            for (int j = 0; j < bossTargetClusters[i].Count; j++)
            {
                // If the drum is indeed the target
                if (bossTargetClusters[i][j] == target)
                {
                    Debug.Log($"Target {i} looking for closest projectile");
                    // found the target that the player defended
                    Projectile closestProjectile = FindClosestProjectile(i);

                    if (closestProjectile != null)
                    {
                        Debug.Log($"Target found projectile!");
                        targetQueues[i][j].Remove(closestProjectile);
                        Destroy(closestProjectile.gameObject);
                    }

                    break;
                }
            }
        }
    }

    private Projectile FindClosestProjectile(int targetIndex)
    {
        Projectile closestProjectile = null;
        foreach (var hashSet in targetQueues[targetIndex])
        {
            foreach (var projectile in hashSet)
            {
                if (closestProjectile == null || projectile.distanceToTarget < closestProjectile.distanceToTarget)
                {
                    closestProjectile = projectile;
                }
            }
        }

        return closestProjectile;
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