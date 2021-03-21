using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //Private variable for the speed of the laser
    [SerializeField]
    private float _speed;
    
    void Update()
    {
        MoveLaser();
        DestroyLaser();
    }

    void MoveLaser()
    {
        //While gameobject is active move it upwards
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    void DestroyLaser()
    {
        //Checked if laser is outside of the screen and if yes destory
        if (transform.position.y > 8f)
        {
            Destroy(gameObject);
        }
    }
}
