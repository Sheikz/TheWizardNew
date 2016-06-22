﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VisibleUnit : VisibleObject
{
    [Tooltip("Is this unit always visible?")]
    public bool alwaysVisible = false;
    [Tooltip("Need to be seen once and then is always visible")]
    public bool needToBeSeenOnce = false;
    public bool isVisible = false;
	private bool hasBeenSeenThisFrame = false;
    private Damageable dmg;

    void Awake()
    {
        dmg = GetComponent<Damageable>();
    }

	// Update is called once per frame
	void FixedUpdate()
	{
        if (alwaysVisible)
            return;

        if (isVisible && needToBeSeenOnce)
            return;

        if (dmg && dmg.isDead)
            return;

        if (!isVisible && hasBeenSeenThisFrame)
		{
			setEnabled(true);
			isVisible = true;
		}
		else if (isVisible && !hasBeenSeenThisFrame)
		{
			setEnabled(false);
			isVisible = false;
		}

		hasBeenSeenThisFrame = false;
	}

    public override void setVisible()
	{
		hasBeenSeenThisFrame = true;
        setEnabled(true);
        isVisible = true;
    }
}
