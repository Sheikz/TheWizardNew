﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;

public class SpellCaster : MonoBehaviour
{
	public GameObject[] spellList;

    protected bool[] isOnCoolDown;
	private Image[] spellIcons;
    private HashSet<Spray> activeSprays;
    private bool isHero = false;
    private SpellBook spellBook;
    private CharacterStats characterStats;
    [HideInInspector]
    public bool isMonster;
    private List<Companion> companionList;
    private List<CompanionController> followerList;
    [HideInInspector]
    public List<CircleSpell> activeCircleSpells;
    private int maxNumberOfActiveCompanions = 3;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    // Use this for initialization
    void Start ()
	{
		isOnCoolDown = new bool[spellList.Length];
		resetCooldowns();
		spellIcons = new Image[spellList.Length];
        activeSprays = new HashSet<Spray>();
        companionList = new List<Companion>();
        followerList = new List<CompanionController>();

        if (tag == "Hero")
        {
            isMonster = false;
            isHero = true;
        }
        else if (tag == "HeroCompanion")
            isMonster = false;
        else
            isMonster = true;

        if (isHero)
        {
			setIcons();
            spellBook = UIManager.instance.spellBook;
            spellBook.addSpell(spellList);
        }
	}

    public void levelUpFollowers()
    {
        foreach (CompanionController follower in followerList)
        {
            if (follower)
            {
                Debug.Log(follower.name + " levelup !");
                CharacterStats stats = follower.GetComponent<CharacterStats>();
                if (stats)
                    stats.levelUp();
            }
        }
    }

    /// <summary>
    /// Learn a new spell. Equip it and add it to the spellbook
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
	public bool addSpell(SpellController spell)
	{
        equipSpell(spell);
        spellBook.addSpell(spell);
        if (spell.removePrerequisites) // Need to remove the prerequisites from the book
        {
            foreach (GameObject prerequisite in spell.prerequisites)
            {
                spellBook.removeSpell(prerequisite.GetComponent<SpellController>());
            }
        } 
		return true;
	}

    public bool equipSpell(SpellController spell)
    {
        if (spellList[spell.spellType.getInt()] == spell.gameObject)
            return false;


        spellList[spell.spellType.getInt()] = spell.gameObject;
        refreshIcons();
        return true;
    }

	/// <summary>
	/// Start a cooldown
	/// </summary>
	/// <param name="spellIndex"></param>
	/// <returns></returns>
	private IEnumerator startCooldown(int spellIndex)
	{
		isOnCoolDown[spellIndex] = true;
        float cdModifier = characterStats ? characterStats.cooldownModifier : 1;
        float cooldown = spellList[spellIndex].GetComponent<SpellController>().cooldown * cdModifier;
		if (isHero)
		{
			float startingTime = Time.time;
			while (Time.time - startingTime < cooldown)
			{
                UIManager.instance.coolDownImages[spellIndex].fillAmount = Mathf.Lerp(1, 0, (Time.time - startingTime) / cooldown);
				yield return null;
			}
            UIManager.instance.coolDownImages[spellIndex].fillAmount = 0;

		}
		else
			yield return new WaitForSeconds(cooldown);
		isOnCoolDown[spellIndex] = false;
	}

	public void resetCooldowns()
	{
		for (int i = 0; i < isOnCoolDown.Length; i++)
		{
			isOnCoolDown[i] = false;
		}
	}

	public void castSpell(int spellIndex, Vector3 initialPos, Vector3 target)
	{
        if (spellList.Length == 0)
            return;

		if (!spellList[spellIndex])
			return;
		if (!GameManager.instance.isPaused && !isOnCoolDown[spellIndex])
		{
            if (spellList[spellIndex].GetComponent<SpellController>().castSpell(this, initialPos, target))
                StartCoroutine(startCooldown(spellIndex));  // Start the cooldown only if the spell has been launched
        }
	}

	public void castAvailableSpells(Vector3 initialPosition, Vector3 target)
	{
		for (int i = 0; i < spellList.Length; i++)
		{
			castSpell(i, initialPosition, target);
		}
	}

    public GameObject[] getEquippedSpells()
    {
        return spellList;
    }

    public HashSet<SpellController> getKnownSpells()
    {
        return spellBook.getSpells();
    }

    /// <summary>
    /// Set the icons of the spells
    /// </summary>
    private void setIcons()
	{
		spellIcons[0] = GameObject.Find("SpellIconPrimarySpell").transform.FindChild("SpellIcon").GetComponent<Image>();
		spellIcons[1] = GameObject.Find("SpellIconSecondarySpell").transform.FindChild("SpellIcon").GetComponent<Image>();
		spellIcons[2] = GameObject.Find("SpellIconDefensive").transform.FindChild("SpellIcon").GetComponent<Image>();
		spellIcons[3] = GameObject.Find("SpellIconUltimate1").transform.FindChild("SpellIcon").GetComponent<Image>();
        spellIcons[4] = GameObject.Find("SpellIconUltimate2").transform.FindChild("SpellIcon").GetComponent<Image>();
        refreshIcons();
	}

	private void refreshIcons()
	{
		for (int i = 0; i < spellList.Length; i++)
		{
			if (!spellList[i])
			{
				Color c = spellIcons[i].color;
				c.a = 0f;
				spellIcons[i].color = c;
			}
			else
			{
				Color c = spellIcons[i].color;
				c.a = 1f;
				spellIcons[i].color = c;
				spellIcons[i].sprite = spellList[i].GetComponent<SpellController>().icon;
			}
		}
	}

    public void addSpray(Spray newSpray)
    {
        activeSprays.Add(newSpray);
    }

    public void removeSpray(Spray spray)
    {
        activeSprays.Remove(spray);
    }

    public Spray getActiveSpray(string name)
    {
        foreach (Spray sp in activeSprays)
        {
            if (sp == null)
            {
                activeSprays.Remove(sp);
                continue;
            }
            if (sp.name == name)
                return sp;
        }
        return null;
    }
        
    /// <summary>
    /// Check if we did not reach the max number of companions. If not, add it
    /// </summary>
    /// <param name="companion"></param>
    /// <returns></returns>
    public void addCompanion(Companion companion)
    {
        companionList.Add(companion);
        if (companionList.Count > maxNumberOfActiveCompanions)
        {
            Destroy(companionList[0].gameObject);
            companionList.RemoveAt(0);
        }
    }

    public void removeCompanion(Companion companion)
    {
        companionList.Remove(companion);
    }

    public void addFollower(CompanionController companion)
    {
        followerList.Add(companion);
    }

    public void removeFollower(CompanionController companion)
    {
        followerList.Remove(companion);
    }
}
