using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private Canvas canvas;
    public List<Image> images;
    private Color c;

    void Start()
    {
        canvas = GetComponent<Canvas>();

        foreach(Image i in images)
        {
            c = i.color;
            c.a = 1.0f;
            i.color = c;
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
