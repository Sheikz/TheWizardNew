﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpellController))]
public class ChainSpell : MonoBehaviour 
{
    public float chainRadius = 3f;
    public int numberOfBounces = 5;
    private SpellController spell;
    public bool activated;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }
        
    void Start()
    {
        numberOfBounces--;
    }

    /// <summary>
    /// Instantiate a copy of itself targeted to the closest monster in range
    /// </summary>
    public void bounce(Collider2D col)
    {
        if (!activated)
            return;

        if (numberOfBounces < 0)
            return;

        if (col)
            col.enabled = false;    // Disabling the collider so we don't include it in the overlapCircle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chainRadius, spell.enemyLayer);
        if (col)
            col.enabled = true;

        if (hits.Length <= 0)
            return;

        GameObject closestTarget = getClosest(hits);
        SpellController newSpell = spell.castSpell(spell.emitter, closestTarget.transform.position);
        MovingSpell movingSpell = newSpell.GetComponent<MovingSpell>();
        if (movingSpell)
            movingSpell.initialize(spell.emitter, transform.position, closestTarget.transform.position);    // Workaround because we cannot use the emitter position in this case

        if (col)
            newSpell.ignoredColliders.Add(col); // Avoid colliding with the monster just after creation
        Damageable dmg = closestTarget.GetComponent<Damageable>();
        AutoPilot autoPilot = newSpell.GetComponent<AutoPilot>();
        if (dmg && autoPilot)
        {
            // If autopilot is available and target is a damageable, lock on it
            autoPilot.lockToObject(dmg.transform);
        }
    }

    /// <summary>
    /// Get the closest from the list of hits
    /// </summary>
    /// <param name="hits"></param>
    /// <returns></returns>
    GameObject getClosest(Collider2D[] hits)
    {
        if (hits.Length <= 0)
            return null;
        
        GameObject result = hits[0].gameObject;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < hits.Length; i++)
        {
            float sqrDistance = (hits[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                result = hits[i].gameObject;
            }
        }
        return result;
    }
}
