﻿using System.Collections;
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
    float _fireRate;
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
      
    //Private Vector3 for the spawn position of the tripples shot 
    [SerializeField]
    Vector3 _tripleShotSpawnPos;

    //Private float for how long the tripleshot will be active
    [SerializeField]
    float _tripleShotTime;

    //Private bool for enabling speed boost
    [SerializeField]
    bool _speedBoostEnabled = false;
         

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

   

    //***PHASE 1: FRAMEWORK***///

    /*Thrusters*/
    [SerializeField]
    GameObject _thrusters;
    
    [SerializeField]
    float _speedBoostAmount;

    /*Shield strength*/
     int _shieldStrength;
    
    /*Ammo count*/
    int _ammoAmount = 15;

    /*Secondary Fire Powerup*/ 
    private FireMode firemode;

    [SerializeField]
    GameObject _homingPrefab;

    /*Thruster: Scaling Bar HUD*/
    float thrustersMaxCharge = 20f;
    float thrustersCurrentCharge;


    //*******************************************************************//

    //************************PHASE 2************************///

    bool reverseControl = false;


    //*******************************************************************//

    void Start()
    {
        //Reset the player position when game starts
        transform.position = new Vector3(0f, -3f, 0f);

        audioSource = GetComponent<AudioSource>();

        firemode = FireMode.laser;

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
      
        ActivateThrusters();
        PlayerMovement();
        ToggleWrapping();
        Shoot();
         
    }

    //**********************************THRUSTERS****************************//
    private void ActivateThrusters()
    {
        SpeedBoostCooldown();
                     
        if(thrustersCurrentCharge <= 0 && _thrusters.activeSelf == true)
        {
            _speedBoostEnabled = false;
            _thrusters.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && thrustersCurrentCharge > 0)
        {
            _speedBoostEnabled = true;
            _thrusters.SetActive(true);
        }
        
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speedBoostEnabled = false;
            _thrusters.SetActive(false);
        }
    }
    //********************************************************************//


    private void Shoot()
    {
        
        //If space is pushed down and cooldown timer is lower than the time since game start
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _timeToNextShot)
        {
            //Reset the cooldown timer to the time since game started plus fire rate
            _timeToNextShot = Time.time + _fireRate;

            
            if (_ammoAmount <= 0)
            {
                uiManager.NoAmmo();
                return;
            }
               
            _ammoAmount--;
            
            uiManager.UpdateAmmoUI(_ammoAmount);

            /***********Secondary Fire Powerup**********/
            switch (firemode)
            {
                case FireMode.laser:
                    GameObject laser = Instantiate(_laserPrefab, transform.position + _tripleShotSpawnPos, Quaternion.identity);
                    laser.transform.parent = _projectileParent;
                    break;

                case FireMode.tripleShot:
                    GameObject tripleShot = Instantiate(_trippleShotPrefab, transform.position + _laserSpawnPos, Quaternion.identity);
                    tripleShot.transform.parent = _projectileParent;
                    break;

                case FireMode.homing:
                    GameObject homing = Instantiate(_homingPrefab, transform.position + _laserSpawnPos, Quaternion.identity);
                    homing.transform.parent = _projectileParent;
                    break;

            }
            //********************************************//

            audioManager.PlayLaserSound();
            
        }
    }

    private void PlayerMovement()
    {
        Vector3 newPosition;

        if (_speedBoostEnabled)
        {
            if(!reverseControl)
            {
                newPosition = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speedBoostAmount * Time.deltaTime;

            }
            else
            {
                newPosition = transform.position + new Vector3(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"), 0f) * _speedBoostAmount * Time.deltaTime;

            }

        }
        else
        {
            if (!reverseControl)
            {
                newPosition = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speed * Time.deltaTime;
            }
            else
            {
                newPosition = transform.position + new Vector3(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"), 0f) * _speed * Time.deltaTime;

            }

        }



        //Clamp y beween -3.8 - 0
        newPosition.y = Mathf.Clamp(newPosition.y, -4.8f, 0f);
                        
        //If wrapping is enabled move player to the opposite side of the screen
        if (_horizontalWrapEnabled)
        {
            if (newPosition.x < -9.6)
            {
                newPosition.x = 11.3f;
            }
            else if (newPosition.x > 11.3f)
            {
                newPosition.x = -9.6f;
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
            _shieldStrength--;

            switch(_shieldStrength)
            {
                case 2:
                    _shieldGO.GetComponent<Renderer>().material.color = Color.blue;
                    break;

                case 1:
                    _shieldGO.GetComponent<Renderer>().material.color = Color.red;
                    break;

                case 0:
                    _shieldGO.SetActive(false);
                    _shieldEnabled = false;
                    break;
            }
                    
            return;
        }

        _lives--;
        StartCoroutine(CameraShake());
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

            case PowerupType.ammo:
                _ammoAmount = 15;
                uiManager.UpdateAmmoUI(_ammoAmount);
                break;

            case PowerupType.shield:
                EnableShield();
                break;

            case PowerupType.health:
                if (_lives >= 3)
                    return;
                _lives++;
                uiManager.UpdateLivesUI(_lives);
                ShowDamage(_lives);
                break;

            case PowerupType.homing:
                StartCoroutine(HomingCooldown());
                break;

            case PowerupType.speedBoost:
                thrustersCurrentCharge = thrustersMaxCharge;
                break;

            case PowerupType.reverse:
                StartCoroutine(ReverseCoolDown());
                break;

        }
    }

    IEnumerator TripleShotCooldown()
    {
        firemode = FireMode.tripleShot;
        yield return new WaitForSeconds(_tripleShotTime);
        firemode = FireMode.laser;

    }

    /***********Secondary Fire Powerup**********/
    IEnumerator HomingCooldown()
    {
        firemode = FireMode.homing;
        yield return new WaitForSeconds(5f);
        firemode = FireMode.laser;

    }
    /******************************************/

    void SpeedBoostCooldown()
    {
        thrustersCurrentCharge -= Time.deltaTime;
        float thrustersPercent = thrustersCurrentCharge / thrustersMaxCharge;
        uiManager.UpdateThrusterChargeUI(thrustersPercent);
    }

    IEnumerator ReverseCoolDown()
    {
        reverseControl = true;
        yield return new WaitForSeconds(10f);
        reverseControl = false;

    }

    private void EnableShield()
    {
        _shieldStrength = 3;
         _shieldGO.GetComponent<Renderer>().material.color = Color.green;


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


    IEnumerator CameraShake()
    {
        Vector3 camCurrentPos = Camera.main.transform.position;

        for (int i = 0; i < 30; i++)
        {
            float randomX = Random.Range(-.5f, .5f);
            float randomY = Random.Range(-.5f, .5f);

            Camera.main.transform.position = new Vector3(randomX, randomY, camCurrentPos.z);
            yield return null;

        }

        Camera.main.transform.position = camCurrentPos;
    }


}

public enum FireMode
{
    laser,
    tripleShot,
    homing
}
       
