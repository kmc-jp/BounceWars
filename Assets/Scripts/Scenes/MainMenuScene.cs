using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScene : IntersceneBehaviour
{
    private GameObject nameInputPanel;
    private GameObject errorText;

    private bool ishost;
    private bool firstTimeNameInput;

    void OnEnable()
    {
        nameInputPanel = GameObject.Find("NameInputPanel");
        errorText = GameObject.Find("IllegalNameWarning");
    }

    void Start()
    {
        nameInputPanel.SetActive(false);
        errorText.SetActive(false);

        nameInputPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }

    public void onHostBtnClick()
    {
        ishost = true;
        nameInputPanel.SetActive(true);
    }
    public void onClientBtnClick()
    {
        ishost = false;
        nameInputPanel.SetActive(true);
    }
    public void onQuitGameBtnClick()
    {
        // exit game
        Application.Quit();
    }
    public void onNameConfirmClick()
    {
        string inputName = nameInputPanel.GetComponentInChildren<InputField>().text;
        //Debug.Log("inputName " + inputName);
        if (!isNameLegal(inputName))
        {
            return;
        }

        G_username = inputName;
        G_isHost = ishost;

        if (ishost)
        {
            nameInputPanel.SetActive(false);
            // goto Host Lobby scene, which shall be 1 within project setting.
            SceneManager.LoadScene("HostLobby");
            
        }
        else
        {
            nameInputPanel.SetActive(false);
            // goto Client Lobby scene, which shall be 2 within project setting.
            SceneManager.LoadScene("ClientLobby");
        }
    }

    public void onNameCancelClick()
    {
        nameInputPanel.SetActive(false);
    }

    private bool isNameLegal(string nameStr)
    {
        string errMsg = "";
        if (nameStr.Length < 4)
            errMsg = "Username must have at least 4 characters.";

        if (!errMsg.Equals(""))
        {
            errorText.GetComponent<Text>().text = errMsg;
            errorText.SetActive(true);

            return false;
        }
        else
            errorText.SetActive(false);

        return true;
    }
}
