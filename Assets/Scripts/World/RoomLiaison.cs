﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomLiaison : MonoBehaviour 
{

    public void refresh()
    {
        foreach (MeshCreator meshCreator in GetComponentsInChildren<MeshCreator>())
        {
            meshCreator.BuildMesh();
        }
    }
}
