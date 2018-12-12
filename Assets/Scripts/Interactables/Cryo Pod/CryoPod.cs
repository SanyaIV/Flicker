using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CryoPod : Interactable {

    const float WAIT_AT_BLACK_IN_SECONDS = 1.5f;

    [Header("Images")]
    [SerializeField] private float _fadeSpeed;
    private static Image _saveScreen;
    private static Text _saveText;
    private static Color _saveScreenColor;
    private static Color _saveTextColor;
    private bool _running;

    public override void Start()
    {
        base.Start();

        if (!_saveScreen)
        {
            _saveScreen = GameObject.FindWithTag("SaveScreen").GetComponent<Image>();
            _saveText = _saveScreen.GetComponentInChildren<Text>();
            _saveScreen.gameObject.SetActive(false);
            _saveScreenColor = _saveScreen.color;
            _saveTextColor = _saveText.color;
        }
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (!_running)
        {
            player.sanity.ResetSanity();
            GameManager.Save();
            StartCoroutine(Save());
        }
    }

    public override string ActionType()
    {
        return "Rest in";
    }

    public override string GetName()
    {
        return "Cryo Pod";
    }

    private IEnumerator Save()
    {
        if (_running)
            yield break;

        _running = true;
        GameManager.pausePlayerMovement = true;

        float alpha = 0f;
        _saveScreenColor.a = alpha;
        _saveTextColor.a = alpha;
        _saveScreen.color = _saveScreenColor;
        _saveText.color = _saveTextColor;
        _saveScreen.gameObject.SetActive(true);

        while(alpha < 1f)
        {
            alpha += _fadeSpeed * Time.deltaTime;
            _saveScreenColor.a = alpha;
            _saveTextColor.a = alpha;
            _saveScreen.color = _saveScreenColor;
            _saveText.color = _saveTextColor;
            yield return null;
        }

        yield return new WaitForSeconds(WAIT_AT_BLACK_IN_SECONDS);

        while (alpha > 0f)
        {
            alpha -= _fadeSpeed * Time.deltaTime;
            _saveScreenColor.a = alpha;
            _saveTextColor.a = alpha;
            _saveScreen.color = _saveScreenColor;
            _saveText.color = _saveTextColor;
            yield return null;
        }

        _saveScreen.gameObject.SetActive(false);
        _running = false;
        GameManager.pausePlayerMovement = false;
        yield break;
    }
}
