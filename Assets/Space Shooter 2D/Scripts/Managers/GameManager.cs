using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    int _score;
    UIManager uiManager;

    bool _playerIsDead;

    private void Start()
    {
        uiManager = GameObject.Find("Game_Manager").GetComponent<UIManager>();

    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && _playerIsDead)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void GameOver()
    {
        _playerIsDead = true;
    }

    public void IncreaseScore(int score)
    {
        _score += score;

        uiManager.UpdateScoreUI(_score);
    }
}
