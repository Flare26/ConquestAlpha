using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IRespawnable
{

    public void ReturnToBuildQueue(GameObject parent)
    {
        throw new System.NotImplementedException();
    }

    public void SetSpawn(Vector3 spawnPoint)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int currentshields, int currenthull)
    {
        throw new System.NotImplementedException();
    }
}
