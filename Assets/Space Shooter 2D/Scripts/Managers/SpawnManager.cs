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


    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
                
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

            if(_isPlayerAlive)
            {
                //Spawn the enemy
                GameObject enemy = Instantiate(_enemyPrefab, randomPos, Quaternion.identity);
                //Set the parent of the game object to be the parent transform
                enemy.transform.parent = _enemyParent;

            }




        }
        
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_isPlayerAlive)
        {
            float randomTime = Random.Range(3, 7);

            //Wait amount in seconds before continuing 
            yield return new WaitForSeconds(randomTime);

            //Set random X position
            Vector3 randomPos = new Vector3(Random.Range(-9f, 9f), 10f, 0f);

            //Random number for the power up array
            int randomPowerup = Random.Range(0, 3);

            //Spawn the enemy
            GameObject powerUp = Instantiate(_powerups[randomPowerup], randomPos, Quaternion.identity);

            //Set the parent of the game object to be the parent transform
            powerUp.transform.parent = _powerUpParent;
            
            
        }

    }

    public void OnPlayerDeath()
    {
        _isPlayerAlive = false;
    }
}
