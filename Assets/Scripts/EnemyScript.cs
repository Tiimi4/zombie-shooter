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
   
    public float attackTimeout;

    private float timeSinceLastAttack = 0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        HpSystem = new HealthSystem(100);
        HpSystem.OnDeath += Die;
        _player = GameObject.Find("PlayerCapsule");
       
        

    }

   

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(_player.transform.position);
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



    private void Die()
    {
        Destroy(gameObject);
    }

}
