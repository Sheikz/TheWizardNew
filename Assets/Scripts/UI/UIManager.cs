﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public Canvas canvas;
	public GameObject floatingText;
    public GameObject floatingTextWithBackground;
	public GameObject floatingTextHolder;
	public ScreenMaskController screenMask;
	public GameObject centerText;
	public LevelUpManager levelUpManager;
	public GameObject exitMenu;
	public SpellBook spellBook;
	public SpellWindowByType spellWindowByType;
    public SpellWindowBySet spellWindowBySet;
	public CharacterWindow characterWindow;
	public FPSDisplay fpsDisplay;
	public CastingBar castingBar;
	public FloatingHPBar floatingHPBar;
	public ToAllocateReminder toAllocateReminder;
	public ControlsWindow controlsWindow;
	public GameObject graphicsWindow;
	public GameObject soundsWindow;
	public Tooltip tooltipPrefab;
	public GameObject[] spellIcons;
	public Sprite emptyIcon;
	public InventoryItemIcon inventoryItemIcon;
    public BuffBar buffBar;
    public InitialSpellSelection initialSpells;
	public Color[] elementColors;
    public Sprite[] elementIcons;

	[HideInInspector]
	public Image[] coolDownImages;
    private Image[] iconImages;
    private Color originalScreenMaskColor;
	public GameObject deathAnimation;
    private Text centerTextText;
    private SpellCaster hero;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		name = "UIManager";
        centerTextText = centerText.GetComponent<Text>();
	}

	public void setupUI()
	{
		screenMask.gameObject.SetActive(false);
		Color newColor = screenMask.GetComponent<Image>().color;
		newColor.a = (float)120 / 255;
		screenMask.GetComponent<Image>().color = newColor;
		centerText.SetActive(false);
		showMenu(false);
		spellBook.initialize();
		spellBook.close();
		spellWindowByType.initialize();
		spellWindowByType.close();
        spellWindowBySet.initialize();
        spellWindowBySet.close();
        linkIcons();
		refreshIconsDescription();
		characterWindow.initialize();
        initialSpells.initialize();
	}

    private void Update()
    {
        updateIconsAccordingToMana();
    }

    void updateIconsAccordingToMana()
    {
        if (!hero)
            hero = GameManager.instance.hero.GetComponent<SpellCaster>();
        if (!hero)
            return;

        for (int i=0; i < iconImages.Length; i++)
        {
            if (!hero.spellList[i])
                continue;

            Color c = iconImages[i].color;
            c.a = hero.hasEnoughMana(hero.spellList[i]) ? 1.0f : 0.5f;
            iconImages[i].color = c;
        }
    }

    public void switchMenu()
	{
		SoundManager.instance.playSound("ClickEsc");
		showMenu(!exitMenu.activeSelf);
	}

	public void showMenu(bool value)
	{
		closeWindows();
		if (!value)
			controlsWindow.gameObject.SetActive(false);

		exitMenu.SetActive(value);
		GameManager.instance.setPause(value);
	}

	public void setScreenColor(Color black)
	{
		Image screenImage = screenMask.GetComponent<Image>();
		originalScreenMaskColor = screenImage.color;
		screenImage.color = Color.black;
	}

	internal void closeWindows()
	{
		spellBook.close();
		spellWindowByType.close();
        spellWindowBySet.close();
		controlsWindow.gameObject.SetActive(false);
		graphicsWindow.SetActive(false);
		soundsWindow.SetActive(false);
		exitMenu.SetActive(false);
		characterWindow.gameObject.SetActive(false);
	}

	private void linkIcons()
	{
		coolDownImages = new Image[spellIcons.Length];
        iconImages = new Image[spellIcons.Length];
		for (int i = 0; i < spellIcons.Length; i++)
		{
			coolDownImages[i] = spellIcons[i].transform.Find("CooldownFill").GetComponent<Image>();
		}
        for (int i = 0; i < spellIcons.Length; i++)
        {
            iconImages[i] = spellIcons[i].transform.Find("SpellIcon").GetComponent<Image>();
        }
    }

	internal void setIconDescription(InputManager.Command cmd)
	{
		// Only does that for the first 5 commands which are linked to the description of icons
		if ((int)cmd > 4)
			return;

		spellIcons[(int)cmd].transform.Find("Description").GetComponent<Text>().text = InputManager.instance.getIconDescription(cmd);
	}

	public void refreshIconsDescription()
	{
		for (int i = 0; i < spellIcons.Length; i++)
		{
			spellIcons[i].transform.Find("Description").GetComponent<Text>().text = InputManager.instance.getIconDescription((InputManager.Command)i);
		}
	}

	public void resetCooldownImages()
	{
		for (int i = 0; i < coolDownImages.Length; i++)
		{
			coolDownImages[i].fillAmount = 0;
		}
	}
	
	public void setScreenColorOriginal()
	{
		setScreenColor(originalScreenMaskColor);
	}

    /// <summary>
    /// Instantiate a floating text above the gameObject
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public void createFloatingText(string text, Color color, GameObject gameObject)
    {
        FloatingText xpText = (Instantiate(floatingText) as GameObject).GetComponent<FloatingText>();
        xpText.initialize(gameObject, text);
        xpText.setColor(color);
    }

    /// <summary>
    /// Instantiate a floating text at the specified position
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public void createFloatingText(string text, Color color, Vector3 position)
    {
        FloatingText xpText = (Instantiate(floatingText) as GameObject).GetComponent<FloatingText>();
        xpText.initialize(position, text);
        xpText.setColor(color);
    }

    public void setCenterText(string text, Color? color = null)
    {
        if (color == null)
            color = Color.white;

        centerTextText.text = text;
        centerText.SetActive(true);
    }

    public float getFPS()
	{
		return fpsDisplay.getFPS();
	}

	public void openControlsWindow(bool value)
	{
		controlsWindow.gameObject.SetActive(value);
	}

	public void openGraphicsWindow(bool value)
	{
		graphicsWindow.SetActive(value);
	}

	public void openSoundsWindow(bool value)
	{
		soundsWindow.SetActive(value);
	}

	public void refreshControlWindow()
	{
		controlsWindow.refresh();
	}

	public void refreshUI()
	{
		toAllocateReminder.refresh();
	}
}
