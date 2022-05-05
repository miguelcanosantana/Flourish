using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private Color disabledTextColor;


    [Header("References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private SaveDataScriptable dataSave;
    [SerializeField] private SaveDataScriptable emptyDataSave;
    [SerializeField] private GameObject loadScreen;


    //Variables
    private int areThereSaves = 0; //0 is false, 1 is true


    //Blur the continue button if there is no a started game
    private void Start()
    {
        //If the key exists, get if there is a previous game
        if (PlayerPrefs.HasKey("anySavesPresent")) areThereSaves = PlayerPrefs.GetInt("anySavesPresent");

        //Create the key and save it with the default value, 0 (false)
        else
        {
            PlayerPrefs.SetInt("anySavesPresent", 0);
            PlayerPrefs.Save();
            areThereSaves = 0;

            Debug.Log("Created player prefs");
        }

        //Blur the button if there are no saves
        if (areThereSaves == 0)
        {
            continueButton.interactable = false;
            continueText.color = disabledTextColor;
        }


    }


    //Activate load screen and load the game scene
    private void LoadScene()
    {
        loadScreen.SetActive(true);
        SceneManager.LoadScene(1);
    }


    //Event => OnNewClick
    public void OnNewClick()
    {
        //Reset the data, copy the new save to the used save
        dataSave = emptyDataSave;
        dataSave.Save();

        //Set as there are no saves
        PlayerPrefs.SetInt("anySavesPresent", 0);

        //Load the scene
        LoadScene();
    }


    //Event => OnContinueClick
    public void OnContinueClick()
    {
        LoadScene();
    }


    //Event => OnConfigClick
    public void OnConfigClick()
    {

    }


    //Event => OnExitQuit, exit the application
    public void OnExitQuit()
    {
        Application.Quit();
    }
}
