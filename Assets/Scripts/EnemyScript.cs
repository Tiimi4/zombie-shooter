using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    NavMeshAgent agent;
    public HealthSystem HpSystem;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        HpSystem = new HealthSystem(100);
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.transform.position);
        
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
