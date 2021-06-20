using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossToPlayerInteractions : MonoBehaviour
{
    public static BossToPlayerInteractions Instance { get; private set; } = null;

    public GameObject projectilePrefab;
    public GameObject[] bossTargets;
    public Transform projectileParent;

    public bool isAttacking = true;
    public float attackCooldown = 5f;
    private float lastAttackTime = 0f;
    private GameObject boss;

    private float timeBetweenSwitches = 10;
    private bool shouldSwitchTargets = true;
    private int curTargetIndex = 0;
    private List<HashSet<Projectile>> targetQueues;

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
        //StartCoroutine(SwitchTargets());
        //AudioManager.Instance.OnBeatStart.AddListener(AttackTarget);
        targetQueues = new List<HashSet<Projectile>>();
        foreach (var target in bossTargets)
        {
            targetQueues.Add(new HashSet<Projectile>());
        }
    }

    void Update()
    {
        attackCooldown = AudioManager.Instance.currentBPS;
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
            newProjectile.targetGO = bossTargets[curTargetIndex];
            targetQueues[startTargetIndex].Add(newProjectile);
            newProjectile.TargetHit.AddListener(() => { OnTargetHit(startTargetIndex, newProjectile); });
        }
        foreach (var target in bossTargets)
        {
            Debug.DrawLine(boss.transform.position, target.transform.position);
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
            newProjectile.targetGO = bossTargets[curTargetIndex];
            targetQueues[startTargetIndex].Add(newProjectile);
            newProjectile.TargetHit.AddListener(() => { OnTargetHit(startTargetIndex, newProjectile); });
        }

    }



    void OnTargetHit(int targetIndex, Projectile projectile)
    {
        targetQueues[targetIndex].Remove(projectile);
        Debug.Log($"Target {targetIndex} was hit!");
        Destroy(projectile.gameObject);
    }

    public void DestroyClosestProjectileOnSameLane(GameObject target)
    {
        for (int i=0; i<bossTargets.Length; i++)
        {
            if (bossTargets[i] == target)
            {
                Debug.Log($"Target {i} looking for closest projectile");
                // found the target that the player defended
                Projectile closestProjectile = FindClosestProjectile(i);
                
                if (closestProjectile != null)
                {
                    Debug.Log($"Target found projectile!");
                    targetQueues[i].Remove(closestProjectile);
                    Destroy(closestProjectile.gameObject);
                }
                break;
            }
        }
    }

    private Projectile FindClosestProjectile(int targetIndex)
    {
        Projectile closestProjectile = null;
        foreach (Projectile projectile in targetQueues[targetIndex])
        {
            if (closestProjectile == null || projectile.distanceToTarget < closestProjectile.distanceToTarget)
            {
                closestProjectile = projectile;
            }
        }
        return closestProjectile;
    }



    IEnumerator SwitchTargets()
    {
        
        while (shouldSwitchTargets)
        {
            SetNewTarget();
            yield return new WaitForSeconds(timeBetweenSwitches);
        }
    }

    void SetNewTarget()
    {
        int new_target_index = Random.Range(0, bossTargets.Length);
        while (curTargetIndex == new_target_index)
        {
            new_target_index = Random.Range(0, bossTargets.Length);
        }
        curTargetIndex = new_target_index;

    }
}