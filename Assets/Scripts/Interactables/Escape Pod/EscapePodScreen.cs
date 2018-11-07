using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePodScreen : MonoBehaviour {

    [Header("Canvas")]
    [SerializeField] private Image _background;
    [SerializeField] private Text _screenText;
    [TextArea]
    [SerializeField] private string _fullyRepairedText;
    [SerializeField] private Color _brokenBackgroundColor;
    [SerializeField] private Color _repairedBackgroundColor;
    private List<EscapePodModule> _modules = new List<EscapePodModule>();

    [Header("Escape Pod Engine")]
    [SerializeField] private EscapePodEngine _engine;

    [Header("Escape Pod Doors")]
    [SerializeField] private DoorButton _doors;
    [SerializeField] private GameObject _invisibleWall;

    [Header("Save")]
    private string _savedText;
    private Color _savedBackgroundColor;

    public void Start()
    {
        _invisibleWall.SetActive(false);
        _background.color = _brokenBackgroundColor;
        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);
    }

    public void UpdateScreen()
    {
        foreach (EscapePodModule module in _modules) //Return from the method if any module hasn't been fully repaired
        { 
            if (!module.Repaired())
            {
                foreach (EscapePodModule module2 in _modules)
                    module2.ShowText();

                return;
            }
        }

        foreach (EscapePodModule module in _modules) //Will only run if all modules have been fully repaired, tells each module to hide its text on the canvas
            module.HideText();

        _background.color = _repairedBackgroundColor;
        _screenText.text = _fullyRepairedText;
        _doors.CloseLockDisable();
        _invisibleWall.SetActive(true);
        _engine.Enable();
    }

    public void AddModule(EscapePodModule module)
    {
        _modules.Add(module);
    }

    public void Save()
    {
        _savedText = _screenText.text;
        _savedBackgroundColor = _background.color;
    }

    public void Reload()
    {
        _screenText.text = _savedText;
        _background.color = _savedBackgroundColor;
        UpdateScreen();
    }
}
