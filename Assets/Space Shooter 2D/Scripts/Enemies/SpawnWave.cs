using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnWave
{
    public GameObject[] enemies;
    
    public int amountOfEnemiesInWave;

    public float spawnTime;
}
