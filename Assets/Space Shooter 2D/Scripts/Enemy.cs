using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Private variable for enemy speed
    [SerializeField]
    float _speed;

    [SerializeField]
    int _scoreAmount;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (gameManager == null)
            Debug.Log("GameManager not found");

    }

    void Update()
    {

        EnemyMovement();
    }

    void EnemyMovement()
    {
        //While active move the enemy downwards
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //If the enemy is outside of the screen move it back to the top
        if (transform.position.y < -6f)
                Respawn();
    }

    void Respawn()
    {
         //Move to random position on top of the screen
        transform.position = new Vector3(Random.Range(-9f, 9f), 10f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the other collider has the tag Player then access the Player script attached to the object
        if(other.tag == "Player")
        {
            //To avoid errors check if player object has the Player script attached
            if(other.GetComponent<Player>() != null)
            {
                other.GetComponent<Player>().DamagePlayer();
            }

            Destroy(gameObject);
        }

        //If other collider has the tag Projectile then first destroy the other object first and then destroy this object
        if (other.tag == "Projectile")
        {

            gameManager.IncreaseScore(_scoreAmount);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

}
