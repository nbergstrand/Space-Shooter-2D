using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    float _speed;

    

    // Update is called once per frame
    void Update()
    {
        Movement();
    }


    void Movement()
    {
        //While active move the enemy downwards
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //If the enemy is outside of the screen move it back to the top
        if (transform.position.y < -6f)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(collision.GetComponent<Player>() != null)
                collision.GetComponent<Player>().EnablePowerUp();

            Destroy(gameObject);
        }
    }
}
