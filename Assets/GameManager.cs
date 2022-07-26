using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private float _timeSurvived = 0f;
    private int _timeSurvivedInt = 0;
    public float difficultyIncreaseInterval;
    private float _timeSincePreviousDifficultyIncrease = 0f;
    private float _timeSincePreviusEnemySpawn = 0f;

    public float enemySpawnInterval;
    public float minSpawnInterval;

    private int _minSpawnAmount = 1;
    private int _maxSpawnAmount = 1;
    private int _spawnAmount = 1;

    public GameObject enemyPrefab;

    private int _enemiesKilled = 0;

    private TextMeshProUGUI _enemiesKilledText;
    private TextMeshProUGUI _timeSurvivedText;
    public Transform[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        _enemiesKilledText = GameObject.Find("enemiesKilled").GetComponent<TextMeshProUGUI>();
        _timeSurvivedText = GameObject.Find("TimeSurvived").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeSurvived();
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

    private void UpdateTimeSurvived()
    {
        _timeSurvived += Time.deltaTime;
        if (_timeSurvived - _timeSurvivedInt > 1)
        {
            _timeSurvivedInt += 1;
            _timeSurvivedText.text = "Time survived " + _timeSurvivedInt + "s";
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

    public void AddEnemyKilled()
    {
        _enemiesKilled += 1;
        _enemiesKilledText.text = "Enemies killed " + _enemiesKilled;
        
    }

    public float GetTimeSurvived()
    {
        return _timeSurvived;
    }
}

