using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    private GameObject _player;
    NavMeshAgent agent;
    public HealthSystem HpSystem;
   
    public float attackTimeout;

    private float timeSinceLastAttack = 0f;

    private float timeAlive = 0f;

    private GameManager _gameManager;
   
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 1f;

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        
        HpSystem = new HealthSystem(100);
        HpSystem.OnDeath += Die;
        _player = GameObject.Find("PlayerCapsule");
    }
    
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(_player.transform.position);
        timeSinceLastAttack += Time.deltaTime;
        timeAlive += Time.deltaTime;

        agent.speed += (Time.deltaTime * 0.5f);
        
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
        _gameManager.AddEnemyKilled();
        Destroy(gameObject);
    }

}
