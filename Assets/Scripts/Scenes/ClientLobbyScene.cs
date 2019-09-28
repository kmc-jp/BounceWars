using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientLobbyScene : IntersceneBehaviour
{

    private GameObject startGameButton;
    private int isClientReady;
    private int isReadyRequest;

    private void OnEnable()
    {
        startGameButton = GameObject.Find("StartGameBtn");
        //startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);

        isClientReady = 0;
        isReadyRequest = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

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
                if (isClientReady > 0)
                    startGameButton.GetComponentInChildren<Text>().color = new Color(1, 0, 0);
                else
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0, 0, 0);
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
        //Dont set isReady until host reply
        isReadyRequest = isClientReady == 0 ? 1 : 0;
    }
    public void onReturnBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public override List<Command> GetCmd()
    {
        List<Command> outCmdTmp = new List<Command>(tosendCmds);
        tosendCmds = new List<Command>();
        if (isClientReady != isReadyRequest)
        {
            outCmdTmp.Add((Command)(new LobbyReadyCmd(isReadyRequest)));
        }
        return outCmdTmp;
    }
}
