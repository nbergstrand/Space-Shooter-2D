using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

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

    int _currentWave = 0;
    public int CurrentWave
    {
        get { return _currentWave; }
    }


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
        
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    

    IEnumerator SpawnEnemyRoutine()
    {
        int enemyInWave = 0;

        while(_isPlayerAlive && _currentWave < waves.Length )
        {

            /************Balanced Spawning*************/
            yield return new WaitForSeconds(waves[_currentWave].spawnTime);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            enemyInWave++;
            int randomEnemy = 0;

            int spawnChance = Random.Range(1, 101);

            //increase the chance of higher difficult enemies each wave
            spawnChance += _currentWave * 2;

            if (spawnChance > 100)
                spawnChance = 100;
            
            if (spawnChance <= 60)
            {
                randomEnemy = (int)EnemyType.SideToSide;
            }
            else if (spawnChance > 50 && spawnChance <= 70)
            {
                if(_currentWave > 1)
                {
                    randomEnemy = (int)EnemyType.StraightShooter;

                }
                else
                {
                    randomEnemy = (int)EnemyType.SideToSide;
                }

            }
            else if (spawnChance > 80 && spawnChance <= 90)
            {
                if(_currentWave > 2)
                {
                    randomEnemy = (int)EnemyType.Burster;
                }                   
                else
                {
                    randomEnemy = (int)EnemyType.SideToSide;
                }

            }
            else if (spawnChance > 90 && spawnChance <= 100)
            {
                if (_currentWave > 3)
                {
                    if(spawnChance > 90 && spawnChance <= 95)
                        randomEnemy = (int)EnemyType.Kamikaze;
                    else if(spawnChance > 95 && spawnChance <= 100)
                    {
                        randomEnemy = (int)EnemyType.SmartShooter;
                    }
                }
                else
                {
                    randomEnemy = (int)EnemyType.SideToSide;
                }

            }


            if (_isPlayerAlive)
            {
                if (randomEnemy != (int)EnemyType.SmartShooter)
                {
                    GameObject enemy = Instantiate(waves[_currentWave].enemies[randomEnemy], randomPos, Quaternion.identity);
                    enemy.transform.parent = _enemyParent;
                }
                else
                {
                    Vector3 sideSpawn = new Vector3(-11.89f, -3f, 0);
                    GameObject enemy = Instantiate(waves[_currentWave].enemies[randomEnemy], sideSpawn, Quaternion.identity);
                    enemy.transform.parent = _enemyParent;
                }
                

            }

            if(enemyInWave >= waves[_currentWave].amountOfEnemiesInWave)
            {
                _currentWave++;
                yield return new WaitForSeconds(10f);
                _uiManager.ShowWaveText(_currentWave + 1);
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
            float randomTime = Random.Range(10, 15);

            yield return new WaitForSeconds(randomTime);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            /************Balanced Spawning*************/
            int spawnChance = Random.Range(1, 101);
            int randomPowerup = 0;

            if (spawnChance <= 30)
            {
                randomPowerup = (int)PowerupType.ammo;
            }
            else if (spawnChance > 30 && spawnChance <= 50 )
            {
                randomPowerup = (int)PowerupType.speedBoost;
            }
            else if (spawnChance > 50 && spawnChance <= 65)
            {
                randomPowerup = (int)PowerupType.health;
            }
            else if (spawnChance > 65 && spawnChance <= 75)
            {
                randomPowerup = (int)PowerupType.reverse;
            }
            else if (spawnChance > 75 && spawnChance <= 85)
            {
                randomPowerup = (int)PowerupType.shield;
            }
            else if (spawnChance > 85 && spawnChance <= 93)
            {
                randomPowerup = (int)PowerupType.tripleShot;
            }
            else if (spawnChance > 93 && spawnChance <= 100)
            {
                randomPowerup = (int)PowerupType.homing;
            }
            /*******************************************/

           

            GameObject powerUp = Instantiate(_powerups[randomPowerup], randomPos, Quaternion.identity);
            powerUp.transform.parent = _powerUpParent;
            
                       
            
        }

    }

    public void OnPlayerDeath()
    {
        _isPlayerAlive = false;
    }
}
