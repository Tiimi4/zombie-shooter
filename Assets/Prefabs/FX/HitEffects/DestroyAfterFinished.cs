using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterFinished : MonoBehaviour
{
    private ParticleSystem[] _particleSystems;


    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    

    // Update is called once per frame
    void Update()
    {
        bool shouldDelete = true;

        foreach (var pSystem in _particleSystems)
        {
            if (pSystem.IsAlive())
            {
                shouldDelete = false;
            }
            
        }

        if (shouldDelete)
        {
            Destroy(gameObject);
        }
    }
}
