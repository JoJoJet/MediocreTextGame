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

    // Start is called before the first frame update
    void Start()
    {
        ResetDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveBackwards()
    {
        currentVignette = currentVignette.parent;
        if(currentVignette.parent == null)
            isDead = false;
        ResetDisplay();
    }

    void ResetDisplay()
    {
        if(currentVignette.background.a > 0)
            Camera.main.backgroundColor = currentVignette.background;

        if(currentVignette.music != null && musicPlayer.clip != currentVignette.music) {
            musicPlayer.clip = currentVignette.music;
            if(currentVignette.isQuiet)
                musicPlayer.volume = 0.75f;
            else
                musicPlayer.volume = 1f;
            musicPlayer.Play();
        }

        image.sprite = currentVignette.image;
        if(image.sprite == null)
            image.color = Color.clear;
        else
            image.color = Color.white;
        textBox.text = currentVignette.text;
        questionBox.text = currentVignette.question;

        for(int i = 0; i < buttonInstances.Length; i++) {
            Destroy(buttonInstances[i]);
        }

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
        else {
            buttonInstances = new GameObject[0];
        }

        if(currentVignette.mode == VignetteResult.Death || currentVignette.mode == VignetteResult.RealDeath) {
            isDead = true;
        }

        if(currentVignette.mode == VignetteResult.RealDeath) {
            var but = Instantiate(buttonPrefab.gameObject, canvas.transform);
            but.transform.SetParent(layout.transform);
            but.GetComponentInChildren<Text>().text = "Restart the game";
            but.GetComponent<Button>().onClick.AddListener(
                () => SceneManager.LoadScene(SceneManager.GetActiveScene().name)
            );
        }

        backButton.gameObject.SetActive(isDead);

        Canvas.ForceUpdateCanvases();
        layout.SetLayoutVertical();
    }

}
