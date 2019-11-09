using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostLobbyScene : IntersceneBehaviour
{

    private GameObject startGameButton;
    private Text HostSceneInfo;
    private int isClientReady;
    private List<int> ClientUnitTypes;

    void OnEnable()
    {
        startGameButton = GameObject.Find("StartGameBtn");
        HostSceneInfo = GameObject.Find("HostSceneInfo").GetComponent<Text>();
        startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);

        isClientReady = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        //get audio manager instance
        audioMgr = audioMgr ?? AudioManager.AudioManager.m_instance;
        audioMgr.PlayMusic("menuTheme");


        //set UI username and ip address.
        HostSceneInfo.text =
            "Host:  " + G_username
            + " \n Host IP Address " + SelfURL;

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
                G_opponentName = c.opponentName;
                ClientUnitTypes = c.unitTypes;    // Client's units
                if (isClientReady > 0)
                {
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0, 0, 0);
                    HostSceneInfo.text =
                        "Host:  " + G_username + " VS " + G_opponentName
                        + " \n Host IP Address " + SelfURL;
                }
                else
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);
                // return this cmd back to Client for checking
                // Tinaxd added unit type field
                tosendCmds.Add((Command)(new LobbyReadyCmd(isClientReady, G_username, GetHostUnits())));
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
            audioMgr.PlaySFX("buttonLow");
            //send LobbyStartgameCmd after loading new scene.
            SceneManager.LoadScene("Host_c");
        }
    }
    public void onReturnBtnClick()
    {
        audioMgr.PlaySFX("buttonHigh");
        SceneManager.LoadScene("MainMenu");
        // For debug
        var units = GetHostUnits();
        if (units == null)
        {
            Debug.Log("Units null");
        }
        else
        {
            foreach (var unit in units)
            {
                Debug.Log(unit);
            }
        }
    }

    private List<int> GetHostUnits()
    {
        return GameObject.Find("UnitSelectPanel").GetComponent<UnitChooserManager>().GetSelectedUnitTypes();
    }
}
