using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRespawnable
{
    // Anything that is respawnable
    void TakeDamage(int currentshields, int currenthull);
    void SetSpawn(Vector3 spawnPoint);
    void ReturnToBuildQueue(GameObject parent);
}
