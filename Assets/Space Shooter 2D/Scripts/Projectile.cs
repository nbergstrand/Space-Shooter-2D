using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Private variable for the speed of the projectile
    [SerializeField]
    private float _speed;
    
       
   

    void Update()
    {
        MoveProjectile();
        DestroyProjectile();
    }

    void DestroyProjectile()
    {
        //If projectile is outside of the screen destroy the object
        if (transform.position.y > 10f)
        {
            if(transform.parent == null || transform.parent.name == "Projectiles_parent")
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
        }
        else if (transform.position.y < -10f)
        {
            if (transform.parent == null || transform.parent.name == "Projectiles_parent")
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }

    private void MoveProjectile()
    {
          
        transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);

    }


  

}
