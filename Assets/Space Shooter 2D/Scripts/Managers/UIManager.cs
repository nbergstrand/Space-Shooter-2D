using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    Text _scoreText;

    [SerializeField]
    Image _currentLivesImage;

    [SerializeField]
    Sprite[] _livesSprites;

    [SerializeField]
    Text _gameOverText;

    [SerializeField]
    Text _restartText;

    bool _playerIsDead;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
    }

    public void UpdateScoreUI(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLivesUI(int lives)
    {
        _currentLivesImage.sprite = _livesSprites[lives];
    }

    public void ShowGameOver()
    {
        _playerIsDead = true;
        _restartText.gameObject.SetActive(true);
        gameManager.GameOver();
        StartCoroutine(TextFlicker());


    }

    IEnumerator TextFlicker()
    {
        while(_playerIsDead)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
    }

    
}
