﻿using UnityEngine;

public class StunEffect : StatusEffect
{
    public float stunDuration;

    public override void inflictStatus(StatusEffectReceiver dmg)
    {
        Debug.Log("stunning " + dmg.name + " for " + stunDuration + " sec");
        dmg.stunFor(stunDuration);
    }
}
