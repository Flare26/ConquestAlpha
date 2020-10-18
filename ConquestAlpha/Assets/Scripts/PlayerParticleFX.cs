using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleFX : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject boostFXPrefab;
    public GameObject bulletHitFX;
    public GameObject shieldPopFX;
    public float lifetime = 3f;
    // Update is called once per frame
    public GameObject initBoostFX()
    {
        var instance = Instantiate(boostFXPrefab);
        return instance;
    }

    public void BulletHitFX()
    {
        var instance = Instantiate(bulletHitFX, gameObject.transform);
        Destroy(instance, lifetime);
    }
    public void PopShield()
    {
        var instance = Instantiate(shieldPopFX, gameObject.transform);
        Destroy(instance, lifetime);
    }

    public void RegenShield()
    {
        var instance = Instantiate(shieldPopFX, gameObject.transform);
        Destroy(instance, lifetime);
    }
}
