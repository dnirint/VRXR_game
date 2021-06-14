using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossToPlayerInteractions : MonoBehaviour
{
    public static BossToPlayerInteractions Instance { get; private set; } = null;

    public GameObject projectilePrefab;
    public GameObject[] bossTargets;
    public Transform projectileParent;

    public bool isAttacking = false;
    public float attackCooldownFactor = 5f;

    private float lastAttackTime = 0f;
    private GameObject boss;

    private int currentTargetIndex = 0;
    private float timeBetweenSwitches = 10;
    private bool shouldSwitchTargets = true;


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
        //TODO: Move this from start, should be handled by a game manager.
        StartCoroutine(SwitchTargets()); 
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

    public void AttackToBeat()
    {
//        AttackRandomTarget();
        AttackTarget();
    }

    void AttackRandomTarget()
    {
        int randomIndex = Random.Range(0, bossTargets.Length);
        Debug.Log($"Targeting {randomIndex}/{bossTargets.Length}");
        var newProjectileGO = Instantiate(projectilePrefab, projectileParent);
        Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
        newProjectile.origin = boss.transform.position;
        newProjectile.targetGO = bossTargets[randomIndex];
    }

    void AttackTarget()
    {
        Debug.Log($"Targeting {currentTargetIndex}/{bossTargets.Length}");
        var newProjectileGO = Instantiate(projectilePrefab, projectileParent);
        Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
        newProjectile.origin = boss.transform.position;
        newProjectile.targetGO = bossTargets[currentTargetIndex];
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
        int randomIndex = Random.Range(0, bossTargets.Length);
        while (randomIndex == currentTargetIndex)
        {
            randomIndex = Random.Range(0, bossTargets.Length);
        }

        currentTargetIndex = randomIndex;
    }
}