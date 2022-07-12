using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    private GameObject _player;
    NavMeshAgent agent;
    public HealthSystem HpSystem;
    public Transform spawnPoint;
    public float attackTimeout;

    private float timeSinceLastAttack = 0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        HpSystem = new HealthSystem(100);
        _player = GameObject.Find("PlayerCapsule");
       
        gameObject.transform.position = spawnPoint.position;

    }

   

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(_player.transform.position);
    
        
        if (HpSystem.GetHealth() == 0)
        {
            // move obj and heal, change into death later on
            Respawn();

        }

        timeSinceLastAttack += Time.deltaTime;
    }
   
    private void OnTriggerStay(Collider other)
    {
        if (timeSinceLastAttack < attackTimeout) return;
        if (other.CompareTag("Player"))
        {
            
            FirstPersonController playerRef = other.GetComponent<FirstPersonController>();
            playerRef.HpSystem.Damage(20);
            Debug.Log(playerRef.HpSystem.GetHealth());
            timeSinceLastAttack = 0f;
        }
    }

    private void Respawn()
    {
        agent.transform.position = spawnPoint.position;
        gameObject.transform.position = spawnPoint.position;
        Instantiate(gameObject);
        Destroy(gameObject);
       
    }

}
