using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    //Private variable for player speed 
    [SerializeField]
    float _speed = 3.5f;

    //Private bool varial to togle horizo
    [SerializeField]
    bool _horizonalWrapEnabled = false;
        
    void Start()
    {
        //Reset the player position when game starts
        transform.position = new Vector3(0f, 0f, 0f);
               
    }

    void Update()
    {
        ToggleWrapping();


        PlayerMovement();
                
    }

    private void ToggleWrapping()
    {
        //Toggle wrapping when Q is pushed down
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _horizonalWrapEnabled = !_horizonalWrapEnabled;
        }
    }


    private void PlayerMovement()
    {
      
        //Move player based on horizonal and vertical axis input
        Vector3 newPosition = transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speed * Time.deltaTime;

        //Clamp y beween -3.8 - 0
        newPosition.y = Mathf.Clamp(newPosition.y, -3.8f, 0f);
                        
        //If wrapping is enabled move player to the opposite side of the screen
        if (_horizonalWrapEnabled)
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
}
