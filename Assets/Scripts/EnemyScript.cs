
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


    private GameManager _gameManager;
    
    private float _accelerationMultiplier = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _accelerationMultiplier += _gameManager.GetTimeSurvived() * 0.01f;
        
        HpSystem = new HealthSystem(100);
        HpSystem.OnDeath += Die;
        _player = GameObject.Find("PlayerCapsule");
    }
    
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(_player.transform.position);
        timeSinceLastAttack += Time.deltaTime;
        agent.speed += (Time.deltaTime * _accelerationMultiplier);
        
    }
   
    private void OnTriggerStay(Collider other)
    {
        if (timeSinceLastAttack < attackTimeout) return;
        if (other.CompareTag("Player"))
        {
            FirstPersonController playerRef = other.GetComponent<FirstPersonController>();
            playerRef.HpSystem.Damage(20);
            //Debug.Log(playerRef.HpSystem.GetHealth());
            timeSinceLastAttack = 0f;
        }
    }



    private void Die()
    {
        _gameManager.AddEnemyKilled();
        Destroy(gameObject);
    }

}
