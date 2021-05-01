using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    [SerializeField]
    private float _speed;
    
    [SerializeField]
    Transform target;
    

    [SerializeField]
    bool _homing;

    bool _gotTarget;
    
    void Start()
    {
        //To prevent checking for enemy every frame restrict it by invoking 0.25 second
        if(_homing == true)
            InvokeRepeating("UpdateTarget", 0f, 0.25f);
    }
    
    void Update()
    {
        MoveProjectile();
        DestroyProjectile();
    }

    void DestroyProjectile()
    {
        //If projectile is outside of the screen destroy the object
        if (transform.position.y > 20f)
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
        else if (transform.position.y < -20f)
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
                 

        if(target == null)
        {

            transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);

        }
        else
        {
            

            //Ensure that the game object is rotated towards the target
            transform.up = target.position - transform.position;

            //if enemy is dead stop following target
            if (target.GetComponent<Enemy>().IsDead)
                target = null;

            //Move towards target. Relative space needs ot be set to self or the object will just move upwards even if turned towards target
            transform.Translate(Vector3.up * _speed * Time.deltaTime, Space.Self);
            

            
        }

    }

    private void UpdateTarget()
    {
        //If target has already been found do not check again
        if (_gotTarget)
            return;

        //Look for all enemy objects in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //Initialize  with Mathf.inifinity to ensure the initial distance check is not too short
        float shortestDistance = Mathf.Infinity;

        //Local GameObject to store the nearst enemy
        GameObject nearestEnemy = null;

        //Iterate through each enemy in the scene and find the closest one
        foreach (GameObject enemy in enemies)
        {
            //Skip to next enemy it is dead already i.e currently running the explosion animation 
            if (enemy.GetComponent<Enemy>().IsDead)
                continue;

            //Store the distance to the enemy in a local variable
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            //if new distanceToEnemy is shorter than the shortestDistance update shortestDistance and set the nearestEnemy to this enemy
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;

                nearestEnemy = enemy;
            }
        }

        //If an enemy has been found set target
        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
            _gotTarget = true;
        }
        else
        {
            target = null;
        }
    }

   

}
