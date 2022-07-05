using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class HealTest : MonoBehaviour
{
    public HealthSystem HpSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        HpSystem = new HealthSystem(50);
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Playr heal trigger");
            
            FirstPersonController playerRef = other.GetComponent<FirstPersonController>();
            playerRef.HpSystem.Heal(20);
            Debug.Log(playerRef.HpSystem.GetHealth());
        }
    }
}
