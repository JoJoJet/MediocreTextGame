using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChoiceDriver : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] Canvas canvas;
    [SerializeField] Button backButton;
    [SerializeField] Image image;
    [SerializeField] VerticalLayoutGroup layout;
    [SerializeField] Text textBox;
    [SerializeField] Text questionBox;

    [Header("Prefabs")]
    [SerializeField]
    Button buttonPrefab;


    [Header("State")]
    [SerializeField]
    Vignette currentVignette;

    [SerializeField]
    GameObject[] buttonInstances;

    private bool isDead = false;

    void Start()
    {
        ResetDisplay();
    }

    public void MoveBackwards()
    {
        ///
        // Set the new current vignette as the parent of the old current vignette.
        currentVignette = currentVignette.parent;
        if(currentVignette.parent == null)
            isDead = false;
        //
        // Recalculate the UI objects for the current vignette.
        ResetDisplay();
    }

    void ResetDisplay()
    {
        //
        // If the vignette defines a background color,
        // set the camera background as that.
        if(currentVignette.background.a > 0)
            Camera.main.backgroundColor = currentVignette.background;
        //
        // If the vignette defines a music clip, and it's different from the current music,
        // start playing it.
        if(currentVignette.music != null && musicPlayer.clip != currentVignette.music) {
            musicPlayer.clip = currentVignette.music;
            if(currentVignette.isQuiet)
                musicPlayer.volume = 0.75f;
            else
                musicPlayer.volume = 1f;
            musicPlayer.Play();
        }
        //
        // If the vignette has a cute image, show it onscreen.
        image.sprite = currentVignette.image;
        if(image.sprite == null)
            image.color = Color.clear;
        else
            image.color = Color.white;
        //
        // Display the text from the vignette.
        textBox.text = currentVignette.text;
        //
        // Display the question from the vignette.
        questionBox.text = currentVignette.question;
        
        //
        // Destroy all of the old buttons.
        for(int i = 0; i < buttonInstances.Length; i++) {
            Destroy(buttonInstances[i]);
        }
        //
        // If the vignette is multiple choice, make a choice for each button.
        if(currentVignette.mode == VignetteResult.Choices) {
            buttonInstances = new GameObject[currentVignette.optionsText.Length];
            for(int i = 0; i < currentVignette.optionsText.Length; i++) {
                var currentOption = currentVignette.optionRefs[i];
                var but = buttonInstances[i] = Instantiate(buttonPrefab.gameObject, canvas.transform);
                but.transform.SetParent(layout.transform);
                but.GetComponentInChildren<Text>().text = currentVignette.optionsText[i];
                but.GetComponent<Button>().onClick.AddListener(
                    () => {
                        isDead = false;
                        currentVignette = currentOption;
                        ResetDisplay();
                    }
                );
            }
        }
        //
        // If the vignette is not multiple choice,
        // delete any past references to old buttons.
        else {
            buttonInstances = new GameObject[0];
        }

        //
        // If we are dead, mark it as such.
        // This means a back button will be displayed.
        if(currentVignette.mode == VignetteResult.Death || currentVignette.mode == VignetteResult.RealDeath) {
            isDead = true;
        }

        //
        // If the vignette results in a death in the second half of a storyine,
        // give the user an option to restart.
        if(currentVignette.mode == VignetteResult.RealDeath) {
            var but = Instantiate(buttonPrefab.gameObject, canvas.transform);
            but.transform.SetParent(layout.transform);
            but.GetComponentInChildren<Text>().text = "Restart the game";
            but.GetComponent<Button>().onClick.AddListener(
                () => SceneManager.LoadScene(SceneManager.GetActiveScene().name)
            );
            buttonInstances = new GameObject[] { but };
        }

        //
        // Turn the back button on or off depending on whether we are dead.
        backButton.gameObject.SetActive(isDead);

        //
        // Force the canvases to recalculate their transforms.
        Canvas.ForceUpdateCanvases();
        layout.SetLayoutVertical();
    }

}
