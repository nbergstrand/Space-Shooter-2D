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

    bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
    }

    Transform _projectileParent;
    GameManager _gameManager;
    Animator _animator;
    AudioManager audioManager;


    /*****************New Enemy Movement***************/
    bool moveRight, moveLeft;
    /**************************************************/

    private void Start()
    {
        /*****************New Enemy Movement***************/
        int randomStartDirection = Random.Range(0, 2);

        if(randomStartDirection == 0)
        {
            moveLeft = false;
            moveRight = true;
        }
        else
        {
            moveLeft = true;
            moveRight = false;
        }
        /**************************************************/
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
        if(!_isDead)
            Shoot();

        EnemyMovement();
    }

    void EnemyMovement()
    {

        /*****************New Enemy Movement***************/
        if (transform.position.x < -9f)
        {
            moveLeft = false;
            moveRight = true;
        }

        if(transform.position.x > 11f)
        {
            moveLeft = true;
            moveRight = false;
        }

        if(moveRight)
            transform.Translate((Vector3.down + Vector3.right) * _speed * Time.deltaTime);

        if(moveLeft)
            transform.Translate((Vector3.down + Vector3.left) * _speed * Time.deltaTime);

        /**************************************************/

        if (transform.position.y < -6f && !_isDead)
                Destroy(gameObject);
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

    /*void Respawn()
    {
         //Move to random position on top of the screen
        transform.position = new Vector3(Random.Range(-9f, 9f), 10f, 0f);
    }*/

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
            _isDead = true;
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;

            Destroy(gameObject, 3f);
        }

        if (other.tag == "Projectile")
        {
            audioManager.PlayExplosionSound();

            _gameManager.IncreaseScore(_scoreAmount);
            _animator.SetTrigger("OnEnemyDeath");
            _isDead = true;
            _speed = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(other.gameObject);
            Destroy(gameObject, 3f);
        }
    }

    

}
