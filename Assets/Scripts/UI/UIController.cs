using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public List<Image> images;
    private Canvas _canvas;
    private Color _c;

    void Start()
    {
        _canvas = GetComponent<Canvas>();

        foreach(Image i in images)
        {
            _c = i.color;
            _c.a = 1.0f;
            i.color = _c;
        }
    }

    
    void Update()
    {

        if (Input.GetKey(KeyCode.T))
        {
            foreach (Image i in images)
            {
                i.CrossFadeAlpha(1, 2.0f, false);
                i.CrossFadeAlpha(0, 2.0f, false);

            }
        }
    }
}
