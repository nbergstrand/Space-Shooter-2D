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
    bool _trippleShotEnabled = false;

    //Private Vector3 for the spawn position of the tripples shot 
    [SerializeField]
    Vector3 _trippleShotSpawnPos;

    //Private Vector3 for the spawn position of the tripples shot 
    [SerializeField]
    float _trippleShotTime;

    SpawnManager spawnManager;
        
    void Start()
    {
        //Reset the player position when game starts
        transform.position = new Vector3(0f, -2.78f, 0f);

        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (spawnManager == null)
            Debug.LogError("No SpawnManager found");

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

            if(_trippleShotEnabled)
            {
                GameObject laser = Instantiate(_trippleShotPrefab, transform.position + _trippleShotSpawnPos, Quaternion.identity);

                laser.transform.parent = _projectileParent;
            }
            else
            {
                //Spawn the laser at the player position plus the offset
                GameObject laser = Instantiate(_laserPrefab, transform.position + _laserSpawnPos, Quaternion.identity);

                laser.transform.parent = _projectileParent;
            }
            
        }
    }

    private void PlayerMovement()
    {
      
        //Move player based on horizonal and vertical axis input
        Vector3 newPosition = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speed * Time.deltaTime;

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
        _lives--;

        if(_lives <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        spawnManager.OnPlayerDeath();
        Destroy(gameObject);
    }


    public void EnablePowerUp()
    {
        StartCoroutine(TrippleShotCooldown());
    }

    IEnumerator TrippleShotCooldown()
    {
        _trippleShotEnabled = true;
        yield return new WaitForSeconds(_trippleShotTime);
        _trippleShotEnabled = false;

    }

}
