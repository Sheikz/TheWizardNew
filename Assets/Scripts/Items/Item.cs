﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Collider2D))]
public abstract class Item : MonoBehaviour
{
    public bool canBePickedUp = true;
    public bool autoPickup = false;

    [HideInInspector]
    public int level = 1;

    private Inventory looter;
    private enum ItemState { DoNothing, MoveToLooter};
    private ItemState state = ItemState.DoNothing;

    protected void Awake()
    {
        GetComponent<Collider2D>().isTrigger = false;
    }

    protected void Start()
    {
        gameObject.layer = LayerManager.itemLayersInt;
        if (GameManager.instance.map)
            transform.SetParent(GameManager.instance.map.itemHolder);
    }

    public virtual void initialize(CharacterStats looter)
    {
        level = looter.level;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        canBePickedUp = true;
    }

    // If it's looted, it should move to the looter
    void FixedUpdate()
    {
        if (state == ItemState.DoNothing)
            return;

        if (looter == null)
        {
            state = ItemState.DoNothing;
            return;
        }

        Vector3 lineToLooter = looter.transform.position - transform.position;
        if (lineToLooter.sqrMagnitude <= looter.sqrDistanceLoot)
        {
            isPickedUpBy(looter);
            return;
        }
        transform.position += lineToLooter.normalized * looter.vacuumSpeed * Time.fixedDeltaTime;
    }

    public abstract void isPickedUpBy(Inventory looter);

    /// <summary>
    /// Avoid infinite picking if an item is dropped at the time it's picked up
    /// </summary>
    protected void deactivatePickupUntilLeftCollider()
    {
        canBePickedUp = false;
    }

    public void pullToSelf(ItemLooter itemLooter)
    {
        GetComponent<Collider2D>().isTrigger = true;
        state = ItemState.MoveToLooter;
        looter = itemLooter.inventory;
    }
}