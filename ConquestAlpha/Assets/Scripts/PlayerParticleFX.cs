using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleFX : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject boostFXPrefab;
    

    // Update is called once per frame
    public GameObject initBoostFX()
    {
        var instance = Instantiate(boostFXPrefab);
        return instance;
    }
}
