using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{

    public HealthSystem HpSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        HpSystem = new HealthSystem(50);
        
    }

    private void Update()
    {
        if (HpSystem.GetHealth() == 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Playr damage trigger");
            
            FirstPersonController playerRef = other.GetComponent<FirstPersonController>();
            playerRef.HpSystem.Damage(20);
            Debug.Log(playerRef.HpSystem.GetHealth());
        }
    }
}
