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

    bool isDead = false;

    GameManager _gameManager;

    Animator _animator;

    private void Start()
    {

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.Log("No Animator found");
        }


        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        

        if (_gameManager == null)
        {
            Debug.Log("GameManager not found");

        }

       
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
        if (transform.position.y < -6f && !isDead)
                Respawn();
    }

    void Respawn()
    {
         //Move to random position on top of the screen
        transform.position = new Vector3(Random.Range(-9f, 9f), 10f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(other.GetComponent<Player>() != null)
            {
                other.GetComponent<Player>().DamagePlayer();
            }

            isDead = true;
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;

            Destroy(gameObject, 3f);
        }

        if (other.tag == "Projectile")
        {

            _gameManager.IncreaseScore(_scoreAmount);
            _animator.SetTrigger("OnEnemyDeath");
            isDead = true;
            _speed = 0;

            Destroy(other.gameObject);
            Destroy(gameObject, 3f);
        }
    }

}
