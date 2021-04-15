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

    [SerializeField]
    GameObject _projectile;

    [SerializeField]
    Vector3 _projectileSpawnPos;
        
    float _timeToNextShot;

    bool isDead = false;

    Transform _projectileParent;

    GameManager _gameManager;

    Animator _animator;

    AudioManager audioManager;

    private void Start()
    {

        _timeToNextShot = Time.time + Random.Range(3, 6);

        _animator = GetComponent<Animator>();

        _projectileParent = GameObject.Find("Projectiles_parent").GetComponent<Transform>();

        if (_animator == null)
        {
            Debug.Log("No Animator found");
        }


        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        

        if (_gameManager == null)
        {
            Debug.Log("GameManager not found");

        }

        audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();

        if (audioManager == null)
        {
            Debug.Log("No AudioManager found");
        }


    }

    void Update()
    {
        if(!isDead)
            Shoot();

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

    private void Shoot()
    {

        if (Time.time > _timeToNextShot)
        {
            //Reset the cooldown timer to the time since game started plus fire rate
            _timeToNextShot = Time.time + Random.Range(2 , 4);
                        
            GameObject laser = Instantiate(_projectile, transform.position + _projectileSpawnPos, Quaternion.identity);

            laser.transform.parent = _projectileParent;
                          

            audioManager.PlayLaserSound();

        }
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
            audioManager.PlayExplosionSound();
            GetComponent<BoxCollider2D>().enabled = false;
            isDead = true;
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;

            Destroy(gameObject, 3f);
        }

        if (other.tag == "Projectile")
        {
            audioManager.PlayExplosionSound();

            _gameManager.IncreaseScore(_scoreAmount);
            _animator.SetTrigger("OnEnemyDeath");
            isDead = true;
            _speed = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(other.gameObject);
            Destroy(gameObject, 3f);
        }
    }

}
