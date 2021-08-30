using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : MonoBehaviour
{
    public ParticleSystem m_ps;
    float particleEndAfter;
    void Start()
    {
        particleEndAfter = m_ps.startLifetime;
        Debug.Log($"------");
        Debug.Log($"Emitting {particleEndAfter}");
        StartCoroutine(EmitAndDestroy());
    }
    
    IEnumerator EmitAndDestroy()
    {
        m_ps.Play();
        yield return new WaitForSeconds(particleEndAfter);
        Destroy(gameObject);
    }
}
