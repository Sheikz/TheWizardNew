﻿using UnityEngine;
using System.Collections;

public enum StatusEffectType { Slow, Root, Stun, Burning, Knockback, Freeze};

public abstract class StatusEffect : MonoBehaviour 
{
    public abstract void applyBuff(BuffsReceiver receiver);
}
