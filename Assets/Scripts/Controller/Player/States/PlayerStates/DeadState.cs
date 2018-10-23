using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player/States/Dead")]
public class DeadState : PlayerState {

    [Header("Images")]
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private Sprite _image;
    private Canvas _canvas;
    private RectTransform _deadScreenTransform;
    private Image _deadScreen;

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);

        _canvas = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
        _deadScreenTransform = new GameObject().AddComponent<RectTransform>();
        _deadScreenTransform.transform.SetParent(_canvas.transform);
        _deadScreenTransform.sizeDelta = new Vector2(1300, 800);
        _deadScreenTransform.localPosition = Vector3.zero;
        _deadScreen = _deadScreenTransform.gameObject.AddComponent<Image>();
        _deadScreen.sprite = _image;
        _deadScreen.color = new Color(0, 0, 0, 0);
    }

    public override void Enter()
    {
        controller.StartCoroutine(FadeOut());
    }

    public void Respawn()
    {
        controller.GetComponent<GameManager>().Respawn();
        controller.sanity.ResetSanity();
        controller.StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        while(_deadScreen.color.a < 1f)
        {
            _deadScreen.color = new Color(0f, 0f, 0f, _deadScreen.color.a + _fadeSpeed * Time.deltaTime);
            Debug.Log(_deadScreen.color);
            yield return null;
        }

        Respawn();

        yield break;
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1f);
        while(_deadScreen.color.a > 0f)
        {
            _deadScreen.color = new Color(_deadScreen.color.r + _fadeSpeed * Time.deltaTime, _deadScreen.color.g + _fadeSpeed * Time.deltaTime, _deadScreen.color.b + _fadeSpeed * Time.deltaTime, _deadScreen.color.a - _fadeSpeed * Time.deltaTime);
            yield return null;
        }

        controller.TransitionTo<AirState>();

        yield break;
    }
}
