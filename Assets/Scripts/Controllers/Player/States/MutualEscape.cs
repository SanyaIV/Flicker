using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player/States/MutualEscape")]
public class MutualEscape : PlayerState
{

    [Header("Images")]
    [SerializeField] private float _fadeSpeed;
    private Image _gameOverImage;
    private Text _gameOverText;

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);

        GameObject go = GameObject.Find("MutualEscape");
        _gameOverImage = go.GetComponent<Image>();
        _gameOverText = go.GetComponentInChildren<Text>();
    }

    public override void Enter()
    {
        base.Enter();
        controller.StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        controller.StartCoroutine(HurtSanity());
        yield return new WaitForSeconds(3f);
        controller.StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Color textColor = _gameOverText.color;
        Color imageColor = _gameOverImage.color;

        while (_gameOverImage.color.a < 1f)
        {
            imageColor.a += _fadeSpeed * Time.deltaTime;
            _gameOverImage.color = imageColor;

            yield return null;
        }

        while (_gameOverText.color.a < 1f)
        {
            textColor.a += _fadeSpeed * Time.deltaTime;
            _gameOverText.color = textColor;

            yield return null;
        }

        yield break;
    }

    private IEnumerator HurtSanity()
    {
        while (true)
        {
            controller.sanity.DepleteSanity();
            yield return null;
        }
    }
}
