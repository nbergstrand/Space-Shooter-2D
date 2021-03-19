using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    [SerializeField]
    float speed = 3.5f;
        
    void Start()
    {
        //Reset the player position when game starts
        transform.position = new Vector3(0f, 1f, 0f);



    }

    void Update()
    {
        transform.Translate( Vector3.right * speed *  Time.deltaTime);
    }
}
