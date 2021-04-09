using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid : MonoBehaviour
{
    [SerializeField]
    float _rotationSpeed;

    [SerializeField]
    GameObject explosion;

    SpawnManager spawnManager;

    private void Start()
    {
        spawnManager = GameObject.Find("Game_Manager").GetComponent<SpawnManager>();

        if(spawnManager == null)
        {
            Debug.Log("No spawn manager found");
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, -1f) * _rotationSpeed * Time.deltaTime);
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Projectile")
        {
            GameObject explosionGO = Instantiate(explosion, transform.position, Quaternion.identity);
            spawnManager.StartSpawning();
            Destroy(collision.gameObject);
            Destroy(explosionGO, 1.7f);
            Destroy(gameObject, 1f);
        }
    }
}
