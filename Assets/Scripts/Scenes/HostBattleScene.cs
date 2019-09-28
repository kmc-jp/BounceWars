using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostBattleScene : IntersceneBehaviour
{
    private bool isClientInGame;
    // Start is called before the first frame update
    void Start()
    {
        isClientInGame = false;
    }
    void Update()
    {
        //Check cmmands
        for (int i = receivedCmds.Count - 1; i >= 0; i--)
        {
            Command cTemp = receivedCmds[i];
            if (cTemp is ClientJoinedCmd)
            {
                Debug.Log("ClientJoined");
                isClientInGame = true;
            }
            // The unprocessed commands are saved for other child of IntersceneBehaviour
            else
                continue;
            receivedCmds.RemoveAt(i);
        }
    }

    // Update is called once per frame
    public override List<Command> GetCmd()
    {
        if (isClientInGame == false)
        {
            //Tell client to start match
            tosendCmds.Add((Command)new LobbyStartgameCmd());
        }
        return base.GetCmd();
    }
    public void onQuitBattleBtnClick()
    {
        CloseHttpListener();
        SceneManager.LoadScene("MainMenu");

        //TODO clear user info
    }
}
