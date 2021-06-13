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
    public float attackCooldownFactor = 5f;

    private float lastAttackTime = 0f;
    private GameObject boss;
    private List<HashSet<Projectile>> targetQueues;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        boss = BossController.Instance.boss;
        targetQueues = new List<HashSet<Projectile>>();
        foreach (var target in bossTargets)
        {
            targetQueues.Add(new HashSet<Projectile>());
        }
    }

    void Update()
    {
        foreach (var target in bossTargets)
        {
            Debug.DrawLine(boss.transform.position, target.transform.position);
        }
        if (isAttacking && attackCooldownFactor + lastAttackTime < Time.time)
        {
            lastAttackTime = Time.time;
            AttackRandomTarget();
        }
    }

    void AttackRandomTarget()
    {
        int randomIndex = Random.Range(0, bossTargets.Length);
        Debug.Log($"Targeting {randomIndex}/{bossTargets.Length}");
        var newProjectileGO = Instantiate(projectilePrefab, projectileParent);
        Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
        newProjectile.origin = boss.transform.position;
        newProjectile.targetGO = bossTargets[randomIndex];
        targetQueues[randomIndex].Add(newProjectile);
        newProjectile.TargetHit.AddListener(() => { OnTargetHit(randomIndex, newProjectile); });
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
}
