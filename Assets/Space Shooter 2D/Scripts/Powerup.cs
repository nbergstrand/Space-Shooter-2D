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
    
    void Update()
    {
        Movement();
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
            if(collision.GetComponent<Player>() != null)
            {
                collision.GetComponent<Player>().EnablePowerup(_powerupType);
            }
               

            Destroy(gameObject);
        }
    }
}

public enum PowerupType
{
    tripleShot,
    speedBoost,
    shield
    
}