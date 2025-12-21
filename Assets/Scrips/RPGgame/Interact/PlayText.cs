using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
public class PlayText : InteractableObject
{
    
    public List<string> linesOfText = new List<string>();

    public TMP_Text textComponent;
    public override void interact()
    {
        playText();
    }  
    public void playText()
    {
        StartCoroutine(textCoroutine());
    }

    IEnumerator textCoroutine()
    {
        foreach(string line in linesOfText)
        {
            textComponent.text = line;
            yield return new WaitForSeconds(3f);
        }
        textComponent.text = "";
    }
}
