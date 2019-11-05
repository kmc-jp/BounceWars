using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostLobbyScene : IntersceneBehaviour
{

    private GameObject startGameButton;
    private int isClientReady;

    void OnEnable()
    {
        startGameButton = GameObject.Find("StartGameBtn");
        startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);

        isClientReady = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        //get audio manager instance
        audioMgr = audioMgr ?? AudioManager.AudioManager.m_instance;
        audioMgr.PlayMusic("menuTheme");

    }
    void Update()
    {
        //Check cmmands
        for (int i = receivedCmds.Count - 1; i >= 0; i--)
        {
            Command cTemp = receivedCmds[i];
            if (cTemp is LobbyReadyCmd)
            {
                LobbyReadyCmd c = (LobbyReadyCmd)cTemp;
                isClientReady = c.isReady;
                if (isClientReady > 0)
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0, 0, 0);
                else
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);
                // return this cmd back to Client for checking
                tosendCmds.Add((Command)(new LobbyReadyCmd(isClientReady)));
            }
            else
                continue;
            receivedCmds.RemoveAt(i);
        }
    }
    public void onStartGameBtnClick()
    {
        if(isClientReady > 0)
        {
            audioMgr.PlaySFX("buttonHigh");
            //send LobbyStartgameCmd after loading new scene.
            SceneManager.LoadScene("Host_c");
        }
    }
    public void onReturnBtnClick()
    {
        audioMgr.PlaySFX("buttonLow");
        SceneManager.LoadScene("MainMenu");
    }
}
