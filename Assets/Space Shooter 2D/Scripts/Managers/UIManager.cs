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
    
    [SerializeField]
    GameObject _pauseMenu;

    //***PHASE 1: FRAMEWORK***///

    [SerializeField]
    Text _ammoText;

    [SerializeField]
    GameObject _noAmmoWarningText;

    [SerializeField]
    Image _chargeFillImage;

    //**********************************//

    /******************PHASE 2************/
    [SerializeField]
    Text _waveText;


    //float _fadeSpeed = 0.1f;

    //**********************************//

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
        if (lives > 3)
            return;

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


    public void TogglePauseMenu()
    {
        if (_pauseMenu.activeSelf != true)
        {
            _pauseMenu.SetActive(true);

        }
        else
        {
            _pauseMenu.SetActive(false);
        }
    }
    
    public void UpdateAmmoUI(int ammoAmount)
    {
        _ammoText.text = ammoAmount + " /  15";

    }

    public void NoAmmo()
    {
        StartCoroutine(ShowAmmoNoWarning());
    }

    IEnumerator ShowAmmoNoWarning()
    {
        int count = 4;
        while(count > 1 )
        {
            _noAmmoWarningText.SetActive(true);
            yield return new WaitForSeconds(.5f);
            _noAmmoWarningText.SetActive(false);
            yield return new WaitForSeconds(.5f);
            count--;
        }
    }
    

    public void UpdateThrusterChargeUI(float percent)
    {
        _chargeFillImage.fillAmount = percent;
    }

    
    public void ShowWaveText(int wave)
    {
        if(wave < 10)
        {
            StartCoroutine("TextFade");
            _waveText.text = "WAVE " + wave;
        }
        else if (wave == 10)
        {
            StartCoroutine("TextFade");
            _waveText.color = new Color(_waveText.color.r, _waveText.color.g, _waveText.color.b, 1);
            _waveText.text = "BOSS BATTLE!";
        }
        else if(wave == 100)
        {
            StopCoroutine("TextFade");
            _waveText.color = new Color(_waveText.color.r, _waveText.color.g, _waveText.color.b, 1);
            _waveText.text = "YOU WON!";
        }
       
    }

    IEnumerator TextFade()
    {

        float progress = 0;
        _waveText.color = new Color(_waveText.color.r, _waveText.color.g, _waveText.color.b, 1);
        yield return new WaitForSeconds(1f);

        while (progress <= 1)
        {
            float alphaValue = Mathf.Lerp(1, 0, progress);
            _waveText.color = new Color(_waveText.color.r, _waveText.color.g, _waveText.color.b, alphaValue);
            progress += Time.deltaTime;
            yield return null;
        }

        _waveText.color = new Color (_waveText.color.r, _waveText.color.g, _waveText.color.b, 0);

    }
    //******************************************************//
}
