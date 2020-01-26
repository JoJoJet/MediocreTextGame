using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextScaler : MonoBehaviour
{
    private Text text;
    private int defaultSize;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        defaultSize = text.fontSize;
    }

    public void SetText(string newText)
    {
        text.fontSize = defaultSize;
        text.text = newText;

        const int DecrUnit = 5;
        while(text.fontSize > DecrUnit && text.preferredHeight > text.rectTransform.sizeDelta.x) {
            text.fontSize -= DecrUnit;
        }
    }
}
