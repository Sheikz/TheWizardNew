﻿using UnityEngine;
using System.Collections;

public class AdjustRotation : MonoBehaviour
{
	void Start ()
	{
		transform.rotation = Quaternion.identity;
	}

    void OnEnable()
    {
        transform.rotation = Quaternion.identity;
    }
}
