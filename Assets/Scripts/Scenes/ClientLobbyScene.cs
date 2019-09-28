using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientLobbyScene : IntersceneBehaviour
{
    private List<Command> cmdToHost;
    public List<Command> commands;

    private GameObject startGameButton;
    private int isClientReady;
    private int isReadyRequest;

    private void OnEnable()
    {
        startGameButton = GameObject.Find("StartGameBtn");
        //startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);

        cmdToHost = new List<Command>();
        commands = new List<Command>();

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
        for (int i = commands.Count - 1; i >= 0; i--)
        {
            Command cTemp = commands[i];
            if (cTemp is LobbyReadyCmd)
            {
                LobbyReadyCmd c = (LobbyReadyCmd)cTemp;
                isClientReady = c.isReady;
                if (isClientReady > 0)
                    startGameButton.GetComponentInChildren<Text>().color = new Color(1, 0, 0);
                else
                    startGameButton.GetComponentInChildren<Text>().color = new Color(1,1,1);
            }
            else if (cTemp is LobbyStartgameCmd)
            {

                SceneManager.LoadScene("Client_c");
            }
            commands.RemoveAt(i);
        }
    }
    public List<Command> getCmd()
    {
        List<Command> outCmdTmp = cmdToHost.ConvertAll(cmd => new Command());
        if (isClientReady != isReadyRequest)
        {
            outCmdTmp.Add((Command)(new LobbyReadyCmd(isClientReady)));
        }
        return outCmdTmp;
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
}
