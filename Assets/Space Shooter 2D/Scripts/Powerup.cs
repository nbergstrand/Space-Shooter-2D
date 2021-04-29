using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Powerup : MonoBehaviour
{

    [SerializeField]
    float _speed;

    [SerializeField]
    PowerupType _powerupType;

    AudioManager audioManager;

    /**********************Phase 2*******************************/

    Transform _player;

    [SerializeField]
    float _absorptionSpeed;

    /*************************************************************/


    private void Start()
    {
        audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if(audioManager == null)
        {
            Debug.Log("No AudioManager found");
        }
              

    }

    void Update()
    {
        if(Input.GetKey(KeyCode.C))
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.position, _absorptionSpeed * Time.deltaTime);
        }
        else
        {
            Movement();
        }
        
    }


    void Movement()
    {
        
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6f)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            audioManager.PlayPowerUpSound();

            if (collision.GetComponent<Player>() != null)
            {
                collision.GetComponent<Player>().EnablePowerup(_powerupType);
            }

            
            Destroy(gameObject);
        }

        if (collision.tag == "EnemyProjectile")
        {
                  
            Destroy(gameObject);
        }


    }
}

public enum PowerupType
{
    tripleShot,
    speedBoost,
    shield,
    ammo,
    health,
    homing,
    reverse
    
}