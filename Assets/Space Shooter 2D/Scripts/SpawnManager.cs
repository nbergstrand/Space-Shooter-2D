using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject _enemyPrefab;

    [SerializeField]
    GameObject _trippleShotPrefab;

    [SerializeField]
    float _timeBetweenEnemies;
        
    //Private  trasnform for the enemy parent 
    [SerializeField]
    Transform _enemyParent;

    //Private  trasnform for the enemy parent 
    [SerializeField]
    Transform _powerUpParent;


    //Private  trasnform for the enemy parent 
    [SerializeField]
    bool _isPlayerAlive = true;


    void Start()
    {
        //At game start run the coroutine for the spawning enemies
        StartCoroutine(SpawnEnemyRoutine());

        //At game start run the coroutine for the spawning powerups
        StartCoroutine(SpawnPowerUpRoutine());

    }

    

    IEnumerator SpawnEnemyRoutine()
    {
        while(_isPlayerAlive)
        {
            //Wait amount in seconds before continuing 
            yield return new WaitForSeconds(_timeBetweenEnemies);
            //Set random X position
            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            //Spawn the enemy
            GameObject enemy = Instantiate(_enemyPrefab, randomPos, Quaternion.identity);

            //Set the parent of the game object to be the parent transform
            enemy.transform.parent = _enemyParent;

           
        }
        
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_isPlayerAlive)
        {
            float randomTime = Random.Range(3, 7);

            //Wait amount in seconds before continuing 
            yield return new WaitForSeconds(1);

            //Set random X position
            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            //Spawn the enemy
            GameObject powerUp = Instantiate(_trippleShotPrefab, randomPos, Quaternion.identity);

            //Set the parent of the game object to be the parent transform
            powerUp.transform.parent = _powerUpParent;
            
            
        }

    }

    public void OnPlayerDeath()
    {
        _isPlayerAlive = false;
    }
}
