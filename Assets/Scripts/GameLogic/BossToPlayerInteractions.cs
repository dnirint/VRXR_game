using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossToPlayerInteractions : MonoBehaviour
{

    public static BossToPlayerInteractions Instance { get; private set; } = null;

    public GameObject projectilePrefab;
    public GameObject[] bossTargets;

    public bool isAttacking = true;
    public float attackCooldownFactor = 5f;

    private float lastAttackTime = 0f;
    private GameObject boss;
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
        Random r = new Random();
        int randomIndex = Random.Range(0, bossTargets.Length);
        Debug.Log($"Targeting {randomIndex}/{bossTargets.Length}");
        var newProjectileGO = Instantiate(projectilePrefab, boss.transform);
        Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
        newProjectile.origin = boss.transform.position;
        newProjectile.targetGO = bossTargets[randomIndex];
    }
}
