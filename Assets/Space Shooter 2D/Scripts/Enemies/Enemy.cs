using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
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


    /*****************Phase 2***************/


    bool _moveRight, _moveLeft;

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

    bool _gotTarget;
    Transform _target;

    float _timeToNextShotOnPowerUp = 0;

    float dodgeDistance = 3;


    /**************************************************/


    private void Start()
    {
        /*****************New Enemy Movement***************/
        int randomStartDirection = Random.Range(0, 2);

        if (randomStartDirection == 0)
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

        if (spawnManager.CurrentWave > 3)
        {
            int randomChance = Random.Range(1, 3);

            if (randomChance == 1)
            {
                EnableShield();
            }

        }

        /**************************************************/

        /*********************Aggressive Enemy Type***********/

        if (_enemyType == EnemyType.Kamikaze)
            InvokeRepeating("LookForTarget", 0f, 0.25f);

        /**************************************************/

        /*********************Dodger Enemy Type***********/

        if (_enemyType == EnemyType.Dodger)
            InvokeRepeating("LookForProjectiles", 0f, 0.15f);

        /**************************************************/


        if (_enemyType == EnemyType.SmartShooter)
        {
            _timeToNextShot = 0f;
        }
        else
        {
            _timeToNextShot = Time.time + Random.Range(3, 6);
        }

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
        if (!_isDead)
        {
            if (_enemyType == EnemyType.StraightShooter || _enemyType == EnemyType.Burster)
                Shoot(true);

            if (_enemyType == EnemyType.SmartShooter)
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.up, Vector2.up);


                if (hit.collider != null)
                {

                    if (hit.collider.tag == "Player")
                        Shoot(false);
                }

            }
            /***************************Enemy Pickups****************************/
            if (_enemyType == EnemyType.StraightShooter || _enemyType == EnemyType.SideToSide)
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.down);


                if (hit.collider != null)
                {

                    if (hit.collider.tag == "Powerup")
                        ShootPowerUp();
                }
            }
            /*********************************************************************/
        }

        EnemyMovement();
    }

    void EnemyMovement()
    {

        switch (_enemyType)
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
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                transform.localPosition = new Vector3(startPosX + WaveMovement(_waveMovementSpeed, _waveMovementAmount), transform.localPosition.y, transform.localPosition.z);
                break;

            /***************************************Aggressive Enemy Type***************************************/

            case EnemyType.Kamikaze:
                if (_target == null)
                {
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
                        transform.Translate((Vector3.up + Vector3.left) * _speed * Time.deltaTime);

                    if (_moveLeft)
                        transform.Translate((Vector3.up + Vector3.right) * _speed * Time.deltaTime);
                }
                else
                {
                    float rotationSpeed = 100;

                    //Rotate towards the player smoothly
                    Quaternion rotatation = Quaternion.FromToRotation(Vector3.up, _target.position - transform.position);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotatation, rotationSpeed * Time.deltaTime);


                    //Move towards target. Relative space needs to be set to self or the object will just move upwards or downwards even if turned towards target
                    transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);
                }
                break;

            /*******************************************************************************************************/
            /***************************************Smart Enemy Type***************************************/

            case EnemyType.SmartShooter:
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
                    transform.Translate(Vector3.right * _speed * Time.deltaTime);

                if (_moveLeft)
                    transform.Translate(Vector3.left * _speed * Time.deltaTime);

                break;
            /*******************************************************************************************************/


            case EnemyType.Dodger:

                transform.Translate(Vector3.down * _speed * Time.deltaTime);

                float distanceToProjectile = 5;

                if (_target != null)
                {
                    distanceToProjectile = Vector2.Distance(transform.position, _target.transform.position);

                }

                if (distanceToProjectile < dodgeDistance && _target.position.y < transform.position.y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, _target.position, -8 * Time.deltaTime);
                    
                }
                    

                break;
        }



        if (transform.position.y < -8f && !_isDead)
            Destroy(gameObject);
    }

    private void Shoot(bool randomCoolDown)
    {

        if (Time.time > _timeToNextShot)
        {
            if (randomCoolDown == false)
            {
                _timeToNextShot = Time.time + 1f;
            }
            else
            {
                _timeToNextShot = Time.time + Random.Range(3, 6);
            }


            GameObject laser = Instantiate(_projectile, transform.position + _projectileSpawnPos, Quaternion.identity);

            laser.transform.parent = _projectileParent;


            _audioManager.PlayLaserSound();

        }
    }
    /***************************Enemy Pickups****************************/
    private void ShootPowerUp()
    {

        if (Time.time > _timeToNextShotOnPowerUp)
        {

            _timeToNextShotOnPowerUp = Time.time + 1f;

            GameObject laser = Instantiate(_projectile, transform.position + _projectileSpawnPos, Quaternion.identity);

            laser.transform.parent = _projectileParent;

            _audioManager.PlayLaserSound();

        }

    }
    /*********************************************************************/



    /*void Respawn()
    {
        //Move to random position on top of the screen
       transform.position = new Vector3(Random.Range(-9f, 9f), 10f, 0f);
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
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
                GetComponent<Renderer>().enabled = false;
                GameObject explosionGO = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(explosionGO, 2f);
                Destroy(gameObject, 1f);
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
    /***************************************Aggressive Enemy Type***************************************/

    private void LookForTarget()
    {
        if (_gotTarget)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        float chaseDistance = 4;


        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < chaseDistance)
        {
            _target = player.transform;
            _gotTarget = true;

        }

    }
    /*******************************************************************************************************/
    private void LookForProjectiles()
    {

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        float shortestDistance = Mathf.Infinity;

        GameObject nearestProjectile = null;

        foreach (GameObject projectile in projectiles)
        {

            float distanceToProjectile = Vector2.Distance(transform.position, projectile.transform.position);

            if (distanceToProjectile < shortestDistance)
            {
                shortestDistance = distanceToProjectile;

                nearestProjectile = projectile;
            }
        }

        //If an enemy has been found set target
        if (nearestProjectile != null)
        {
            _target = nearestProjectile.transform;
            _gotTarget = true;
        }
        else
        {
            _target = null;
        }

    }
}


public enum EnemyType
{
    SideToSide,
    StraightShooter,
    Burster,
    Kamikaze,
    SmartShooter,
    Dodger

}