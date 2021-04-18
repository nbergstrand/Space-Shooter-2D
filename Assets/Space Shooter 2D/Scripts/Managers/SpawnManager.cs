using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject _enemyPrefab;

    //Private array for the power up prefabs
    [SerializeField]
    GameObject[] _powerups;

   /* [SerializeField]
    float _timeBetweenEnemies;*/
        
    //Private  trasnform for the enemy parent 
    [SerializeField]
    Transform _enemyParent;

    //Private  trasnform for the enemy parent 
    [SerializeField]
    Transform _powerUpParent;


    //Private  trasnform for the enemy parent 
    [SerializeField]
    bool _isPlayerAlive = true;


    /**********************Framework****************/
    [SerializeField]
    GameObject[] _superPowerups;
    /************************************************/

    /*************Phase 2**********************/
    UIManager _uiManager;

    [SerializeField]
    SpawnWave[] waves;

    int currentWave = 0;
    /************************************************/


    private void Start()
    {
        _uiManager = GetComponent<UIManager>();

        if (_uiManager == null)
            Debug.Log("No UIManager found for SpawnManager");
    }

    public void StartSpawning()
    {

        _uiManager.ShowWaveText(1);
        Debug.Log(waves.Length);
        StartCoroutine(SpawnEnemyRoutine());
                
        StartCoroutine(SpawnPowerUpRoutine());
    }

    

    IEnumerator SpawnEnemyRoutine()
    {
        int enemyInWave = 0;

        while(_isPlayerAlive && currentWave < waves.Length )
        {
            
            //**********WAVE SYSTEM*******************************//
            yield return new WaitForSeconds(waves[currentWave].spawnTime);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            enemyInWave++;
            int randomEnemy = Random.Range(0, waves[currentWave].enemies.Length);

            if (_isPlayerAlive)
            {
                GameObject enemy = Instantiate(waves[currentWave].enemies[randomEnemy], randomPos, Quaternion.identity);
                enemy.transform.parent = _enemyParent;

            }

            if(enemyInWave >= waves[currentWave].amountOfEnemiesInWave)
            {
                currentWave++;
                yield return new WaitForSeconds(10f);
                _uiManager.ShowWaveText(currentWave + 1);
                enemyInWave = 0;
            }



        }
        _uiManager.ShowWaveText(100);
        /************************************************/

    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_isPlayerAlive)
        {
            float randomTime = Random.Range(3, 7);

            yield return new WaitForSeconds(randomTime);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            int randomPowerup = Random.Range(0, _powerups.Length);

            /******Secondary Fire Powerup****/
            int superPowerupChance = Random.Range(0, 15);
            int randomSuperPowerup = Random.Range(0, _superPowerups.Length);
            if (superPowerupChance == 10)
            {
                GameObject superPowerUp = Instantiate(_superPowerups[randomSuperPowerup], randomPos, Quaternion.identity);

                superPowerUp.transform.parent = _powerUpParent;

            }
            else
            {
                GameObject powerUp = Instantiate(_powerups[randomPowerup], randomPos, Quaternion.identity);

                powerUp.transform.parent = _powerUpParent;
            }
            /*******************************************/
                       
            
        }

    }

    public void OnPlayerDeath()
    {
        _isPlayerAlive = false;
    }
}
