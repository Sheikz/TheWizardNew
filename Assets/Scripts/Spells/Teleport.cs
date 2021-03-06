﻿using UnityEngine;
using System.Collections;
using System;

public class Teleport : SpellController
{
    public GameObject teleportAnimation;

    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        // Check if there is enough space where to go
        float emitterRadius = 0.5f;
        CircleCollider2D cc = emitter.GetComponent<CircleCollider2D>();
        if (cc)
            emitterRadius = cc.radius;

        if (Physics2D.OverlapCircle(target, emitterRadius, GameManager.instance.layerManager.blockingLayer))
            return null;

        emitter.transform.position = target;
        if (teleportAnimation)
            Instantiate(teleportAnimation, target, Quaternion.identity);
        return this;
    }

    public override bool canCastSpell(SpellCaster spellCaster, Vector3 initialPos, Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.5f, GameManager.instance.layerManager.blockingLayer))
        {
            Debug.Log("Impossible to teleport there");
            return false;
        }
        return true;
    }
}
