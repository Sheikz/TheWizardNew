﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

/// <summary>
/// Script used to generate animations clip and animation override controller from a structured spritesheet
/// </summary>
public class AnimationCreator : EditorWindow
{
    private RuntimeAnimatorController animatorController;
    private Sprite spriteSheet;
    private string spriteName = "";

    private string currentState = "";
    private string currentDirection = "";

    [MenuItem("Window/Animation Creator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationCreator), false, "Animation Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Spritesheet of character", EditorStyles.boldLabel);
        GUILayout.Label("Name of the folder where the monster is located", EditorStyles.wordWrappedMiniLabel);
        spriteName = GUILayout.TextField(spriteName);

        animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Controller to override", animatorController, typeof(RuntimeAnimatorController), false);

        if (GUILayout.Button("Create animations"))
        {
            createAnimation();
        }
    }

    void createAnimation()
    {
        string[] states = { "Idle", "Walking", "Attacking", "Dying" };
        string[] directions = { "Down", "Top", "Left", "Right" };
        Debug.Log("sprite name: " + spriteName);
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteName + "/");
        Debug.Log(sprites.Length);
        if (sprites.Length != 64)
        {
            Debug.LogError("Invalid sprite sheet " + spriteName+". Length should be 64, is " + sprites.Length);
            return;
        }
        AnimationClip[] clips = new AnimationClip[20];  // 16 for the usual ones, 4 for PrepareAttack

        for (int row = 0; row < 16; row ++)
        {
            clips[row] = createAnimationClip(sprites, row, states[row % 4], directions[row / 4]);
            AssetDatabase.CreateAsset(clips[row], "Assets/Animations/Animations/GeneratedAnimations/" + clips[row].name + ".anim");
        }
        for (int i= 0; i < 4; i++)
        {
            clips[16+i] = createAnimationClip(sprites, i, "PrepareAttack", directions[i]);
            AssetDatabase.CreateAsset(clips[16 + i], "Assets/Animations/Animations/GeneratedAnimations/" + clips[16 + i].name + ".anim");
        }
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = animatorController;
        setupClips(overrideController, clips);
        overrideController.name = spriteName + "Controller";
        AssetDatabase.CreateAsset(overrideController, "Assets/Animations/Animations/GeneratedControllers/" + overrideController.name + ".controller");
    }

    AnimationClip createAnimationClip(Sprite[] sprites, int row, string currentState, string currentDirection)
    {     
        AnimationClip clip = new AnimationClip();
        if (currentState == "Idle" || currentState == "Walking")
        {
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
        }
        
        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.propertyName = "m_Sprite";
        ObjectReferenceKeyframe[] keyFrames;
        // Need to create a 2-sprites animation in this case
        if (currentState == "PrepareAttack")
        {
            keyFrames = new ObjectReferenceKeyframe[2];

            for (int i = 0; i < 2; i++)
            {
                ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
                kf.time = i / 12f;
                kf.value = sprites[row * 16 + 8 + i];
                keyFrames[i] = kf;
            }
        }
        else
        {
            keyFrames = new ObjectReferenceKeyframe[4];

            for (int i = 0; i < 4; i++)
            {
                ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
                kf.time = i / 12f;
                kf.value = sprites[row * 4 + i];
                keyFrames[i] = kf;
            }
        }
        clip.name = spriteName + currentState + currentDirection;

        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

        return clip;
    }

    /// <summary>
    /// Setup the clips in the correct position
    /// </summary>
    /// <param name="overrideController"></param>
    /// <param name="clips"></param>
    private void setupClips(AnimatorOverrideController overrideController, AnimationClip[] clips)
    {
        overrideController["M3IdleDown"] = clips[0];
        overrideController["M3IdleLeft"] = clips[8];
        overrideController["M3IdleTop"] = clips[4];
        overrideController["M3IdleRight"] = clips[12];
        overrideController["M3AttackingDown"] = clips[2];
        overrideController["M3AttackingLeft"] = clips[10];
        overrideController["M3AttackingTop"] = clips[6];
        overrideController["M3AttackingRight"] = clips[14];
        overrideController["M3WalkingDown"] = clips[1];
        overrideController["M3WalkingLeft"] = clips[9];
        overrideController["M3WalkingTop"] = clips[5];
        overrideController["M3WalkingRight"] = clips[13];
        overrideController["M3DyingDown"] = clips[3];
        overrideController["M3DyingLeft"] = clips[11];
        overrideController["M3DyingTop"] = clips[7];
        overrideController["M3DyingRight"] = clips[15];
        overrideController["PrepareToAttackDown"] = clips[16];
        overrideController["PrepareToAttackLeft"] = clips[18];
        overrideController["PrepareToAttackTop"] = clips[17];
        overrideController["PrepareToAttackRight"] = clips[19];
    }
}