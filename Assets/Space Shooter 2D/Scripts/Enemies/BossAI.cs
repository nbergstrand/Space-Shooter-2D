using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossAI : MonoBehaviour
{
    //Private variable for enemy speed
    [SerializeField]
    float _speed;

    [SerializeField]
    int _scoreAmount;

    [SerializeField]
    GameObject[] _projectiles;

    [SerializeField]
    GameObject _explosionEffect;

    [SerializeField]
    Transform[] _sideProjectileSpawnPos;
    [SerializeField]
    Transform[] _burstProjectileSpawnPos;
    [SerializeField]
    Transform[] _frontProjectileSpawnPos;

    float _timeToNextShot;
    float _nextBurstTime;
    float _timeToNextShotSide;

    int _lives = 20;

    bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
    }

    Transform _projectileParent;
    Transform _playerTransform;
    GameManager _gameManager;
    Animator _animator;
    AudioManager _audioManager;
    UIManager _uiManager;

    


    private void Start()
    {


        _animator = GetComponent<Animator>();

        _projectileParent = GameObject.Find("Projectiles_parent").GetComponent<Transform>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _uiManager = GameObject.Find("Game_Manager").GetComponent<UIManager>();

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
                    
            RaycastHit2D hitLeft = Physics2D.Raycast((Vector2)_frontProjectileSpawnPos[0].position + Vector2.down, Vector2.down);
            RaycastHit2D hitRight = Physics2D.Raycast((Vector2)_frontProjectileSpawnPos[1].position + Vector2.down, Vector2.down);
            
            if (hitLeft.collider != null && hitLeft.collider.tag == "Player")
            {

                Shoot(_frontProjectileSpawnPos[0], (int)ProjectilesObject.large);

            }

             if (hitRight.collider != null && hitRight.collider.tag == "Player")
             {
                Debug.Log("Hit Right");

                Shoot(_frontProjectileSpawnPos[1], (int)ProjectilesObject.large);
                
             }

            ShootBurst();
            ShootTowardsPlayer();
            BossMovement();
        }
    }



        void BossMovement()
        {
            if(transform.position.y > 0f)
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        private void Shoot(Transform firePoint, int projectile)
        {

            if (Time.time > _timeToNextShot)
            {

                _timeToNextShot = Time.time + 0.25f;



                GameObject laser = Instantiate(_projectiles[projectile], firePoint.position, Quaternion.identity);

                laser.transform.parent = _projectileParent;


                _audioManager.PlayLaserSound();

            }
        }

        private void ShootBurst()
        {
            if(Time.time > _nextBurstTime)
            {
                _nextBurstTime = Time.time + Random.Range(3f, 8f);
                int randomPos = Random.Range(0, 2);
                GameObject laser = Instantiate(_projectiles[(int)ProjectilesObject.burst], _burstProjectileSpawnPos[randomPos].position, Quaternion.identity);
                laser.transform.parent = _projectileParent;
            
                 _audioManager.PlayLaserSound();
             }
        }

    private void ShootTowardsPlayer()
    {
       

        if (Time.time > _timeToNextShotSide)
        {
           
            _timeToNextShotSide = Time.time + Random.Range(2f, 4f);



            if (_playerTransform.position.x < 0f)
            {
                GameObject laser1 = Instantiate(_projectiles[(int)ProjectilesObject.laser], _sideProjectileSpawnPos[0].position, Quaternion.FromToRotation(Vector3.up, _sideProjectileSpawnPos[0].position - _playerTransform.position));
                laser1.transform.parent = _projectileParent;

                GameObject laser2 = Instantiate(_projectiles[(int)ProjectilesObject.laser], _sideProjectileSpawnPos[1].position, Quaternion.FromToRotation(Vector3.up, _sideProjectileSpawnPos[1].position - _playerTransform.position));
                laser2.transform.parent = _projectileParent;
            }
            else
            {
                GameObject laser1 = Instantiate(_projectiles[(int)ProjectilesObject.laser], _sideProjectileSpawnPos[2].position, Quaternion.FromToRotation(Vector3.up, _sideProjectileSpawnPos[2].position - _playerTransform.position));
                laser1.transform.parent = _projectileParent;

                GameObject laser2 = Instantiate(_projectiles[(int)ProjectilesObject.laser], _sideProjectileSpawnPos[3].position, Quaternion.FromToRotation(Vector3.up, _sideProjectileSpawnPos[3].position - _playerTransform.position));
                laser2.transform.parent = _projectileParent;
                
            }
            

            _audioManager.PlayLaserSound();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.tag == "Player" && !_isDead)
            {

                if (other.GetComponent<Player>() != null)
                {
                    other.GetComponent<Player>().DamagePlayer();
                }
                

            }

            if (other.tag == "Projectile" && !_isDead)
            {
                Destroy(other.gameObject);
                _lives--;
                StartCoroutine(Hurt());
            
            }

            if(_lives <= 0 && !_isDead)
            {
                _isDead = true;
                StartCoroutine(DyingRoutine());
            }
                
    }

    IEnumerator Hurt()
    {
        Color originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        GetComponent<Renderer>().material.color = originalColor;


    }

    IEnumerator DyingRoutine()
    {
        GameObject explosionGO1 = Instantiate(_explosionEffect, _sideProjectileSpawnPos[0].position, Quaternion.identity);
        Destroy(explosionGO1, 2f);
        yield return new WaitForSeconds(1);

        GameObject explosionGO2 = Instantiate(_explosionEffect, _sideProjectileSpawnPos[1].position, Quaternion.identity);
        Destroy(explosionGO2, 2f);
        yield return new WaitForSeconds(1f);

        GameObject explosionGO3 = Instantiate(_explosionEffect, _sideProjectileSpawnPos[2].position, Quaternion.identity);
        Destroy(explosionGO3, 2f);
        yield return new WaitForSeconds(1f);

        GameObject explosionGO4 = Instantiate(_explosionEffect, _sideProjectileSpawnPos[3].position, Quaternion.identity);
        Destroy(explosionGO1, 2f);
        yield return new WaitForSeconds(1f);

        GameObject explosionGO5 = Instantiate(_explosionEffect, _burstProjectileSpawnPos[0].position, Quaternion.identity);
        Destroy(explosionGO5, 2f);
        yield return new WaitForSeconds(1f);

        GameObject explosionGO6 = Instantiate(_explosionEffect, _burstProjectileSpawnPos[1].position, Quaternion.identity);
        Destroy(explosionGO6, 2f);
        yield return new WaitForSeconds(1f);

        GameObject explosionGO7 = Instantiate(_explosionEffect, _frontProjectileSpawnPos[0].position, Quaternion.identity);
        Destroy(explosionGO7, 2f);
        yield return new WaitForSeconds(1f);

        GameObject explosionGO8 = Instantiate(_explosionEffect, _frontProjectileSpawnPos[1].position, Quaternion.identity);
        Destroy(explosionGO8, 2f);
        yield return new WaitForSeconds(1f);
               
        _uiManager.ShowWaveText(100);
    }

}

public enum ProjectilesObject
{
    laser,
    burst,
    large
}
