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


   
    [SerializeField]
    GameObject[] _superPowerups;
   
    UIManager _uiManager;

    [SerializeField]
    SpawnWave[] waves;

    int _currentWave = 0;
    public int CurrentWave
    {
        get { return _currentWave; }
    }


 


    private void Start()
    {
        _uiManager = GetComponent<UIManager>();

        if (_uiManager == null)
            Debug.Log("No UIManager found for SpawnManager");
    }

    public void StartSpawning()
    {

        _uiManager.ShowWaveText(_currentWave + 1);
        
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    

    IEnumerator SpawnEnemyRoutine()
    {
        int enemyInWave = 0;

        while(_isPlayerAlive && _currentWave < 10 )
        {

            yield return new WaitForSeconds(waves[_currentWave].spawnTime);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 8f, 0f);

            enemyInWave++;

            int randomEnemy = 0;

            int spawnChance = Random.Range(1, 101);
            

            //increase the chance of higher difficulty enemies each wave
            spawnChance += _currentWave * 2;

            if (spawnChance > 100)
                spawnChance = 100;
            Debug.Log(spawnChance);

            if (spawnChance <= 50)
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
            else if (spawnChance > 70 && spawnChance <= 90)
            {
                if(_currentWave > 2)
                {
                    if (spawnChance > 70 && spawnChance <= 80)
                        randomEnemy = (int)EnemyType.Burster;
                    else if (spawnChance > 81 && spawnChance <= 90)
                    {
                        randomEnemy = (int)EnemyType.Dodger;
                    }
                }                   
                else
                {
                    randomEnemy = (int)EnemyType.SideToSide;
                }

            }
            else if (spawnChance > 90 && spawnChance <= 100)
            {
                if (_currentWave >= 3)
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
                GameObject enemy;

                //If Boss Wave
                if (CurrentWave == 9)
                {
                    enemy = Instantiate(waves[_currentWave].enemies[0], new Vector3(0, 7.15f, 0f), Quaternion.identity);
                    enemy.transform.parent = _enemyParent;
                    
                }
                else
                {
                    if (randomEnemy != (int)EnemyType.SmartShooter)
                    {

                        if (randomEnemy == (int)EnemyType.Kamikaze)
                            enemy = Instantiate(waves[_currentWave].enemies[randomEnemy], randomPos, Quaternion.Euler(0f, 0f, 180f));
                        else
                        {
                            enemy = Instantiate(waves[_currentWave].enemies[randomEnemy], randomPos, Quaternion.identity);
                        }
                        enemy.transform.parent = _enemyParent;
                    }
                    else
                    {
                        Vector3 sideSpawn = new Vector3(-11.89f, -3f, 0);
                        enemy = Instantiate(waves[_currentWave].enemies[randomEnemy], sideSpawn, Quaternion.identity);
                        enemy.transform.parent = _enemyParent;
                    }
                }
                    
              

                
                

            }

            if(enemyInWave >= waves[_currentWave].amountOfEnemiesInWave)
            {
                _currentWave++;
                yield return new WaitForSeconds(10f);

                if(CurrentWave < 10)
                    _uiManager.ShowWaveText(_currentWave + 1);

                enemyInWave = 0;
            }
            
        }

       


       

    }

    IEnumerator SpawnPowerUpRoutine()
    {
       
        while (_isPlayerAlive)
        {
            float randomTime = Random.Range(5, 10);

            yield return new WaitForSeconds(randomTime);

            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            /************Balanced Spawning*************/
            int spawnChance = Random.Range(1, 101);
            int randomPowerup = 0;

            if (spawnChance <= 40)
            {
                randomPowerup = (int)PowerupType.ammo;
            }
            else if (spawnChance > 40 && spawnChance <= 55 )
            {
                randomPowerup = (int)PowerupType.speedBoost;
            }
            else if (spawnChance > 55 && spawnChance <= 70)
            {
                randomPowerup = (int)PowerupType.health;
            }
            else if (spawnChance > 70 && spawnChance <= 80)
            {
                randomPowerup = (int)PowerupType.reverse;
            }
            else if (spawnChance > 80 && spawnChance <= 90)
            {
                randomPowerup = (int)PowerupType.shield;
            }
            else if (spawnChance > 90 && spawnChance <= 93)
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
