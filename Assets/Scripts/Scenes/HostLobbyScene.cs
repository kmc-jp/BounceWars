using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostLobbyScene : IntersceneBehaviour
{
    public List<Command> commands;
    private List<Command> cmdFromHost;

    private GameObject startGameButton;
    private int isClientReady;

    private void OnEnable()
    {
        startGameButton = GameObject.Find("StartGameBtn");
        startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);
        commands = new List<Command>();
        cmdFromHost = new List<Command>();

        isClientReady = 0;
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    void Update()
    {
        //Check cmmands
        for (int i = commands.Count - 1; i >= 0; i--)
        {
            Command cTemp = commands[i];
            if (cTemp is LobbyReadyCmd)
            {
                LobbyReadyCmd c = (LobbyReadyCmd)cTemp;
                isClientReady = c.isReady;
                if(isClientReady>0)
                    startGameButton.GetComponentInChildren<Text>().color = new Color(1,1,1);
                else
                    startGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f);
                // return this cmd back to Client for checking
                cmdFromHost.Add((Command)c);
            }
            commands.RemoveAt(i);
        }
    }
    public void onStartGameBtnClick()
    {
        if(isClientReady > 0)
        {
            //send LobbyStartgameCmd after loading new scene.
            SceneManager.LoadScene("Host_c");
        }
    }
    public void onReturnBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public List<Command> GetCommandsFromHost()
    {
        List<Command> outCmdTmp = cmdFromHost.ConvertAll(cmd => new Command());
        cmdFromHost = new List<Command>();
        return outCmdTmp;
    }
}
