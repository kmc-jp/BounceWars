using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientLobbyScene : IntersceneBehaviour
{

    private GameObject startGameButton;
    private Text ClientSceneInfo;
    private int isClientReady;
    private int isReadyRequest;
    private List<int> HostUnitTypes;

    [SerializeField]
    private UnitChooserManager UCManager = default;

    private void OnEnable()
    {
        startGameButton = GameObject.Find("StartGameBtn");
        ClientSceneInfo = GameObject.Find("ClientSceneInfo").GetComponent<Text>();
        //startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);

        isClientReady = 0;
        isReadyRequest = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        //get audio manager instance
        audioMgr = audioMgr ?? AudioManager.AudioManager.m_instance;
        audioMgr.PlayMusic("menuTheme");


        //set UI username and ip address.
        ClientSceneInfo.text =
            "Client:  " + G_username
            + " \n Host IP Address " + TargetURL;
    }
    private void Update()
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
                if (isClientReady > 0)
                {
                    startGameButton.GetComponentInChildren<Text>().color = new Color(1, 0, 0);
                    ClientSceneInfo.text =
                        "Client:  " + G_username + " VS " + G_opponentName
                        + " \n Host IP Address " + TargetURL;
                }
                else
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0, 0, 0);

                HostUnitTypes = c.unitTypes;
            }
            else if (cTemp is LobbyStartgameCmd)
            {
                SceneManager.LoadScene("Client_c");
            }
            // The unprocessed commands are saved for other child of IntersceneBehaviour
            else
                continue;
            receivedCmds.RemoveAt(i);
        }
    }
    public void onStartGameBtnClick()
    {
        audioMgr.PlaySFX("buttonLow");
        //Dont set isReady until host reply
        isReadyRequest = isClientReady == 0 ? 1 : 0;
    }
    public void onReturnBtnClick()
    {
        audioMgr.PlaySFX("buttonHigh");
        SceneManager.LoadScene("MainMenu");
        // For debug
        var units = GetClientUnits();
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
    public override List<Command> GetCmd()
    {
        List<Command> outCmdTmp = new List<Command>(tosendCmds);
        tosendCmds = new List<Command>();
        if (isClientReady != isReadyRequest)
        {
            var cunits = GetClientUnits();
            if (cunits != null)
            {
                outCmdTmp.Add((Command)(new LobbyReadyCmd(isReadyRequest, G_username, cunits)));
            }
        }
        return outCmdTmp;
    }

    private List<int> GetClientUnits()
    {
        return UCManager.GetSelectedUnitTypes();
    }
}
