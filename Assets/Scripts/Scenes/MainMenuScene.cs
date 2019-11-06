using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioManager;

public class MainMenuScene : IntersceneBehaviour
{
    private GameObject nameInputPanel;
    private GameObject ipAddrInputPanel;
    private GameObject nameErrText;
    private GameObject ipErrText;

    private bool ishost;
    private bool firstTimeNameInput;

    void OnEnable()
    {
        nameInputPanel = GameObject.Find("NameInputPanel");
        ipAddrInputPanel = GameObject.Find("ipAddrInputPanel");

        nameErrText = GameObject.Find("IllegalNameWarning");
        ipErrText = GameObject.Find("IllegalIpAddrWarning");
        //If Http port is on, close it
        CloseHttpListener();
    }

    void Start()
    {
        nameInputPanel.SetActive(false);
        nameErrText.SetActive(false);
        ipAddrInputPanel.SetActive(false);
        ipErrText.SetActive(false);

        //get audio manager instance
        audioMgr = audioMgr ?? AudioManager.AudioManager.m_instance;
        audioMgr.PlayMusic("menuTheme");
        nameInputPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        ipAddrInputPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        nameInputPanel.GetComponentInChildren<InputField>().text = null;
        // set IP if already defined
        if (TargetURL != null)
            ipAddrInputPanel.GetComponentInChildren<InputField>().text = TargetURL;
        else
            ipAddrInputPanel.GetComponentInChildren<InputField>().text = null;
    }

    public void onHostBtnClick()
    {
        ishost = true;
        audioMgr.PlaySFX("buttonLow");
        nameInputPanel.SetActive(true);
    }
    public void onClientBtnClick()
    {
        ishost = false;
        audioMgr.PlaySFX("buttonLow");
        nameInputPanel.SetActive(true);
    }
    public void onQuitGameBtnClick()
    {
        // exit game
        audioMgr.PlaySFX("buttonHigh");
        Application.Quit();
    }
    public void onNameConfirmClick()
    {
        audioMgr.PlaySFX("buttonLow");
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
            ipAddrInputPanel.SetActive(true);
        }
    }
    public void onIpConfirmClick()
    {
        audioMgr.PlaySFX("buttonLow");
        string ipAddr = ipAddrInputPanel.GetComponentInChildren<InputField>().text;
        if (!isIpAddrLegal(ipAddr))
        {
            return;
        }
        else
        {
            TargetURL = ipAddr;
            ipAddrInputPanel.SetActive(false);
            // goto Client Lobby scene, which shall be 2 within project setting.
            SceneManager.LoadScene("ClientLobby");
        }
    }

    public void onNameCancelClick()
    {
        audioMgr.PlaySFX("buttonHigh");
        nameInputPanel.SetActive(false);
    }

    public void onIpCancelClick()
    {
        audioMgr.PlaySFX("buttonHigh");
        ipAddrInputPanel.SetActive(false);
        nameInputPanel.SetActive(true);
    }

    public void onMuteClick()
    {
        audioMgr.PlaySFX("buttonHigh");
        if (AudioListener.volume == 1.0f)
        {
            AudioListener.volume = 0.0f;
            GameObject.Find("MuteBtn/Text").GetComponent<Text>().text = "Unmute";
        }
        else
        {
            AudioListener.volume = 1.0f;
            GameObject.Find("MuteBtn/Text").GetComponent<Text>().text = "Mute";
        }
    }

    private bool isNameLegal(string nameStr)
    {
        string errMsg = "";
        if (nameStr.Length < 4)
            errMsg = "Username must have at least 4 characters.";

        if (!errMsg.Equals(""))
        {
            nameErrText.GetComponent<Text>().text = errMsg;
            nameErrText.SetActive(true);

            return false;
        }
        else
            nameErrText.SetActive(false);

        return true;
    }
    private bool isIpAddrLegal(string ipStr)
    {
        string errMsg = "";
        if ((!ValidateIPv4(ipStr))&&ipStr!="localhost")
        {
            errMsg = "Invalid IP address.";
        }
        if (!errMsg.Equals(""))
        {
            ipErrText.GetComponent<Text>().text = errMsg;
            ipErrText.SetActive(true);

            return false;
        }
        else
        {
            ipErrText.SetActive(false);
        }
        return true;
    }
    private bool ValidateIPv4(string ipString)
    {
        if (System.String.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }
}
