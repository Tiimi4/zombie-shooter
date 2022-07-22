using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private float _timeSurvived = 0f;
    public float difficultyIncreaseInterval;
    private float _timeSincePreviousDifficultyIncrease = 0f;
    private float _timeSincePreviusEnemySpawn = 0f;

    public float enemySpawnInterval;
    public float minSpawnInterval;

    private int _minSpawnAmount = 1;
    private int _maxSpawnAmount = 3;
    private int _spawnAmount = 1;

    public GameObject enemyPrefab;

    public Transform[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timeSurvived += Time.deltaTime;
        _timeSincePreviusEnemySpawn += Time.deltaTime;
        _timeSincePreviousDifficultyIncrease += Time.deltaTime;
        
       
        if(_timeSincePreviusEnemySpawn > enemySpawnInterval ) SpawnEnemy();
        if(_timeSincePreviousDifficultyIncrease > difficultyIncreaseInterval) IncreaseDifficultyOverTime();
        
    }

    private void IncreaseDifficultyOverTime()
    {
        _timeSincePreviousDifficultyIncrease = 0f;
        
        //_maxSpawnAmount += 1;
        if (enemySpawnInterval > minSpawnInterval)
        { 
            enemySpawnInterval *= 0.9f;
        }


    }

    private void SpawnEnemy()
    {
        _spawnAmount = Random.Range(_minSpawnAmount, _maxSpawnAmount);
        for (int i = 0; i < _spawnAmount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[randomIndex]);
            _timeSincePreviusEnemySpawn = 0f;
        }
      
    }
}

