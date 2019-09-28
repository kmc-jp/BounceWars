using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientBattleScene : IntersceneBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        tosendCmds.Add((Command)new ClientJoinedCmd());
        //if units are null ask host battle scene
    }

    // Update is called once per frame
    void Update()
    {
        //Check cmmands
        for (int i = receivedCmds.Count - 1; i >= 0; i--)
        {
            Command cTemp = receivedCmds[i];
            if (cTemp is LobbyStartgameCmd)
            {
                // the client is already in game
            }
            // The unprocessed commands are saved for other child of IntersceneBehaviour
            else
                continue;
            receivedCmds.RemoveAt(i);
        }
    }
    public void onQuitBattleBtnClick()
    {
        CloseHttpListener();
        SceneManager.LoadScene("MainMenu");
        //TODO clear user info
    }
}
