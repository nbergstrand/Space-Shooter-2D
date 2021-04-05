using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedUI : MonoBehaviour
{

    [SerializeField]
    Text scoreText;

    float score;

    [SerializeField]
    Image _currentLivesImage;

    [SerializeField]
    Sprite[] _livesSprites;


    void Start()
    {
        StartCoroutine(animateUI());
    }

    private void Update()
    {
        score += Time.deltaTime * 40;

        int showScore = Mathf.RoundToInt(score);
        scoreText.text = "Score: " + showScore;
    }


    IEnumerator animateUI()
    {
        while(true)
        {
            _currentLivesImage.sprite = _livesSprites[3];
            yield return new WaitForSeconds(0.25f);
            _currentLivesImage.sprite = _livesSprites[2];
            yield return new WaitForSeconds(0.25f);
            _currentLivesImage.sprite = _livesSprites[1];
            yield return new WaitForSeconds(0.25f);
            _currentLivesImage.sprite = _livesSprites[0];
            yield return new WaitForSeconds(0.25f);
        }
    }

}
