using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Private variable for player speed 
    [SerializeField]
    float _speed = 3.5f;

    //Private bool variable to toggle horizonal wrapping
    [SerializeField]
    bool _horizontalWrapEnabled = false;

    //Private Vector3 for the spawn position of the laser 
    [SerializeField]
    Vector3 _laserSpawnPos;

    //Private float for fire rate / length of cooldown
    [SerializeField]
    float _fireRate = .5f;
    //Private variable to check when one can fire
    float _timeToNextShot;

    //Private variable for the lives of the player
    [SerializeField]
    int _lives = 3;

    //Private variable for the parent object 
    [SerializeField]
    Transform _projectileParent;

    //Private GameObject for the laser prefab
    [SerializeField]
    GameObject _laserPrefab;

    //Private GameObject for the tripple shot prefab
    [SerializeField]
    GameObject _trippleShotPrefab;

    //Private bool for enabling tripple shot
    [SerializeField]
    bool _tripleShotEnabled = false;

    //Private Vector3 for the spawn position of the tripples shot 
    [SerializeField]
    Vector3 _tripleShotSpawnPos;

    //Private float for how long the tripleshot will be active
    [SerializeField]
    float _tripleShotTime;

    //Private bool for enabling speed boost
    [SerializeField]
    bool _speedBoostEnabled = false;

    //Private bool for enabling speed boost
    [SerializeField]
    float _speedBoostAmount;

    //Private float for how long the speed boost will be active
    [SerializeField]
    float _speedBoostTime;

    [SerializeField]
    bool _shieldEnabled;

    [SerializeField]
    GameObject _shieldGO;

    [SerializeField]
    GameObject _rightEngineDamage;

    [SerializeField]
    GameObject _leftEngineDamage;
    
    [SerializeField]
    int _score;
        
    AudioSource audioSource;

    SpawnManager spawnManager;
    UIManager uiManager;
    AudioManager audioManager;
        
    void Start()
    {
        //Reset the player position when game starts
        transform.position = new Vector3(0f, -2.78f, 0f);

        audioSource = GetComponent<AudioSource>();


        spawnManager = GameObject.Find("Game_Manager").GetComponent<SpawnManager>();
        uiManager = GameObject.Find("Game_Manager").GetComponent<UIManager>();
        audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();

        if (spawnManager == null)
            Debug.LogError("No SpawnManager found");

        if (uiManager == null)
            Debug.LogError("No UIManager found");

        if (audioManager == null)
            Debug.LogError("No AudioManager found");

    }

    void Update()
    {
        PlayerMovement();
        ToggleWrapping();
        Shoot();
         
    }

    private void Shoot()
    {
        
        //If space is pushed down and cooldown timer is lower than the time since game start
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _timeToNextShot)
        {
            //Reset the cooldown timer to the time since game started plus fire rate
            _timeToNextShot = Time.time + _fireRate;

            if(_tripleShotEnabled)
            {
                GameObject laser = Instantiate(_trippleShotPrefab, transform.position + _tripleShotSpawnPos, Quaternion.identity);

                laser.transform.parent = _projectileParent;
            }
            else
            {
                //Spawn the laser at the player position plus the offset
                GameObject laser = Instantiate(_laserPrefab, transform.position + _laserSpawnPos, Quaternion.identity);

                laser.transform.parent = _projectileParent;
            }

            audioManager.PlayLaserSound();
            
        }
    }

    private void PlayerMovement()
    {
        Vector3 newPosition;

        if (_speedBoostEnabled)
        {
            
            newPosition = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speedBoostAmount * Time.deltaTime;

        }
        else
        {

            newPosition = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speed * Time.deltaTime;

        }



        //Clamp y beween -3.8 - 0
        newPosition.y = Mathf.Clamp(newPosition.y, -3.8f, 0f);
                        
        //If wrapping is enabled move player to the opposite side of the screen
        if (_horizontalWrapEnabled)
        {
            if (newPosition.x < -11.3)
            {
                newPosition.x = 11.3f;
            }
            else if (newPosition.x > 11.3f)
            {
                newPosition.x = -11.3f;
            }

        }
        else
        {
            //Clamp the horizontal position between -9 and 9
            newPosition.x = Mathf.Clamp(newPosition.x, -9f, 9f);
        }

        //Set player position to new position 
        transform.position = newPosition;
    }


    private void ToggleWrapping()
    {
        //Toggle wrapping when Q is pushed down
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _horizontalWrapEnabled = !_horizontalWrapEnabled;
        }
    }

    public void DamagePlayer()
    {
        if(_shieldEnabled)
        {
            _shieldGO.SetActive(false);
            _shieldEnabled = false;
            return;
        }

        _lives--;

        uiManager.UpdateLivesUI(_lives);
               

        if(_lives <= 0)
        {
            Die();
        }

        ShowDamage(_lives);
    }

    void ShowDamage(int lives)
    {
        switch(lives)
        {
            case 2:
                _rightEngineDamage.SetActive(true);
                _leftEngineDamage.SetActive(false);
                break;

            case 1:
                _leftEngineDamage.SetActive(true);
                break;

            default:
                _rightEngineDamage.SetActive(false);
                _leftEngineDamage.SetActive(false);
                break;
        }
        

    }

    void Die()
    {
        spawnManager.OnPlayerDeath();
        uiManager.ShowGameOver();
        Destroy(gameObject);
    }


    public void EnablePowerup(PowerupType powerupType)
    {
        switch (powerupType)
        {
            case PowerupType.tripleShot:
                StartCoroutine(TripleShotCooldown());
                break;

            case PowerupType.speedBoost:
                StartCoroutine(SpeedBoostCooldown());
                break;

            case PowerupType.shield:
                EnableShield();
                break;
        }
    }

    IEnumerator TripleShotCooldown()
    {
        _tripleShotEnabled = true;
        yield return new WaitForSeconds(_tripleShotTime);
        _tripleShotEnabled = false;

    }

    IEnumerator SpeedBoostCooldown()
    {
        _speedBoostEnabled = true;
        yield return new WaitForSeconds(_speedBoostTime);
        _speedBoostEnabled = false;
    }

    private void EnableShield()
    {
         _shieldGO.SetActive(true);

        _shieldEnabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyProjectile")
        {
            DamagePlayer();

            Destroy(other.gameObject);
        }

       
    }

}

       
