using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Private variable for enemy speed
    [SerializeField]
    float _speed;
    
    void Start()
    {
        Respawn();
    }

    
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        //While active move the enemy downwards
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
                Respawn();
    }

    void Respawn()
    {
        //Set a random number between -9 and 9
        float randomX = Random.Range(-9f, 9f);

        //Start at random position 
        transform.position = new Vector3(randomX, 8f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(other.GetComponent<Player>() != null)
                other.GetComponent<Player>().DamagePlayer();

            Destroy(gameObject);
        }

        if(other.tag == "Projectile")
        {
            Debug.Log("Hit by laser");
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
