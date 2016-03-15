﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a hole that kill a monster if it falls in
/// </summary>
public class Hole : MonoBehaviour
{
    public float damageRatio = 0.2f; // Damage % falling into this hole inflicts
    private Collider2D[] colliders;

    private List<MovingCharacter> charactersInside;

    void Awake()
    {
        colliders = GetComponents<Collider2D>();
        charactersInside = new List<MovingCharacter>();
    }

    void Update()
    {
        for (int i = charactersInside.Count - 1; i >= 0; i --)
        {
            if (!charactersInside[i])
            {
                charactersInside.RemoveAt(i);
                continue;
            }
            foreach (Collider2D col in colliders)
            {
                if (col.bounds.Contains(charactersInside[i].transform.position))
                {
                    charactersInside[i].startFalling(damageRatio);
                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        MovingCharacter character = collision.gameObject.GetComponent<MovingCharacter>();
        if (!charactersInside.Contains(character))
            charactersInside.Add(character);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        MovingCharacter character = collision.gameObject.GetComponent<MovingCharacter>();
        charactersInside.Remove(character);
    }
}
