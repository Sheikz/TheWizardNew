﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeSkill : Skill
{
    public MagicElement magicSchool;
    public float multiplier = 1.25f;

    void Start()
    {
        GetComponent<Image>().sprite = SpellManager.instance.elementIcons[(int)magicSchool];
    }

    public override void applySkill(GameObject hero)
    {
        hero.GetComponent<PlayerStats>().multiplyMastery(multiplier, magicSchool);
    }

    public override void initializeSkill()
    {
        base.initializeSkill();
    }

    public override string getDescription()
    {
        string result = base.getDescription();
        int ratioInPercents = Mathf.RoundToInt((multiplier - 1.0f) * 100f);
        result = result.Replace("<multiplier>", "<color=orange>" + ratioInPercents + "%</color>");
        return result;
    }
}
