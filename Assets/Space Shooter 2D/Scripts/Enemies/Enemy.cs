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
    AudioManager _audioManager;


    /*****************Phase 1***************/
    bool _moveRight, _moveLeft;
    /**************************************************/


    /*****************Phase 2***************/
    [SerializeField]
    EnemyType _enemyType;

    float startPosX;

    [SerializeField]
    float _waveMovementSpeed;
    [SerializeField]
    float _waveMovementAmount;
    [SerializeField]
    GameObject explosion;

    [SerializeField]
    GameObject _shieldGO;

    bool _shieldEnabled;
    SpawnManager spawnManager;

    /**************************************************/


    private void Start()
    {
        /*****************New Enemy Movement***************/
        int randomStartDirection = Random.Range(0, 2);

        if(randomStartDirection == 0)
        {
            _moveLeft = false;
            _moveRight = true;
        }
        else
        {
            _moveLeft = true;
            _moveRight = false;
        }
        /**************************************************/


        /*********************New Enemy Type***********/
        startPosX = transform.position.x;

        /**************************************************/

        /*********************Enemy Shields***********/
        spawnManager = GameObject.Find("Game_Manager").GetComponent<SpawnManager>();

        if(spawnManager.CurrentWave > 3)
        {
            int randomChance = Random.Range(1, 3);

            if(randomChance == 1)
            {
                EnableShield();
            }
            
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

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();

        if (_audioManager == null)
        {
            Debug.Log("No AudioManager found");
        }


    }

    void Update()
    {
        if(!_isDead)
        {
            if(_enemyType == EnemyType.StraightShooter || _enemyType == EnemyType.Burster)
                Shoot();
        }

        EnemyMovement();
    }

    void EnemyMovement()
    {

        switch(_enemyType)
        {
            case EnemyType.SideToSide:
                if (transform.position.x < -9f)
                {
                    _moveLeft = false;
                    _moveRight = true;
                }

                if (transform.position.x > 11f)
                {
                    _moveLeft = true;
                    _moveRight = false;
                }

                if (_moveRight)
                    transform.Translate((Vector3.down + Vector3.right) * _speed * Time.deltaTime);

                if (_moveLeft)
                    transform.Translate((Vector3.down + Vector3.left) * _speed * Time.deltaTime);

                break;

            case EnemyType.StraightShooter:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;

            case EnemyType.Burster:
                transform.Translate(Vector3.down  * _speed * Time.deltaTime);
                transform.localPosition = new Vector3(startPosX + WaveMovement(_waveMovementSpeed, _waveMovementAmount), transform.localPosition.y, transform.localPosition.z);
               
                break;

        }



        if (transform.position.y < -8f && !_isDead)
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
                          

            _audioManager.PlayLaserSound();

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
            
            if (other.GetComponent<Player>() != null)
            {
                other.GetComponent<Player>().DamagePlayer();
            }

            if (_shieldEnabled == true)
            {
                _shieldGO.SetActive(false);
                _shieldEnabled = false;

                return;
            }


            if (_enemyType == EnemyType.StraightShooter || _enemyType == EnemyType.SideToSide)
            {
                _animator.SetTrigger("OnEnemyDeath");
                Destroy(gameObject, 3f);
            }
            else
            {
                //GetComponent<Renderer>().enabled = false;
                GameObject explosionGO = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(explosionGO, 2f);
               
            }


            _speed = 0;
            _audioManager.PlayExplosionSound();
            GetComponent<BoxCollider2D>().enabled = false;
            _isDead = true;


        }

        if (other.tag == "Projectile")
        {
            Destroy(other.gameObject);

            if (_shieldEnabled == true)
            {
                _shieldGO.SetActive(false);
                _shieldEnabled = false;

                return;
            }

            _gameManager.IncreaseScore(_scoreAmount);
            if (_enemyType == EnemyType.StraightShooter || _enemyType == EnemyType.SideToSide)
            {
                _animator.SetTrigger("OnEnemyDeath");
            }
            else
            {
                GetComponent<Renderer>().enabled = false;
                GameObject explosionGO = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(explosionGO, 2f);
                

            }
            _audioManager.PlayExplosionSound();
            _isDead = true;
            _speed = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            
            Destroy(gameObject, 3f);
        }
    }


    float WaveMovement(float speed, float amount)
    {
        return (((Mathf.Cos((Time.time * speed) + 1) / 2) * amount));
    }


    private void EnableShield()
    {
        _shieldGO.SetActive(true);
        _shieldEnabled = true;
    }


}


public enum EnemyType
{
    SideToSide,
    StraightShooter,
    Burster

}