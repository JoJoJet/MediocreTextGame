using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum VignetteResult { Choices, Death, RealDeath };

[CreateAssetMenu(fileName = "Vignette", menuName = "Vignette", order = 1)]
public class Vignette : ScriptableObject
{
    [Header("Completion")]
    public bool parentFinished;
    public bool imageFinished, textFinished, questionFinished, branchFinished;

    [Header("Data")]
    public Vignette parent;

    public AudioClip music = null;
    public bool isQuiet = false;
    public Color background = new Color(1f, 1f, 1f, 0f);

    public Sprite image;

    [Multiline]
    public string text;

    public string question;

    public VignetteResult mode;

    [SerializeField]
    public string[] optionsText;
    [SerializeField]
    public Vignette[] optionRefs;

    void OnValidate()
    {
        if(optionsText.Length > optionRefs.Length) {
            var nu = new Vignette[optionsText.Length];
            for(int i = 0; i < optionRefs.Length; i++)
                nu[i] = optionRefs[i];
            optionRefs = nu;
        }
        if(optionsText.Length < optionRefs.Length) {
            var nu = new Vignette[optionsText.Length];
            for(int i = 0; i < optionsText.Length; i++)
                nu[i] = optionRefs[i];
            optionRefs = nu;
        }
    }
}