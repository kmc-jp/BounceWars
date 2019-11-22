using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientBattleScene : IntersceneBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //4 is the ID of clientBattleScene
        tosendCmds.Add((Command)new ClientJoinedCmd(4));
        //if units are null ask host battle scene

        //get audio manager instance
        audioMgr = audioMgr ?? AudioManager.AudioManager.m_instance;
        audioMgr.PlayMusic("battleConflict");
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
                // send again in case of Package lose
                tosendCmds.Add((Command)new ClientJoinedCmd(4));
            }
            // The unprocessed commands are saved for other child of IntersceneBehaviour
            else
                continue;
            receivedCmds.RemoveAt(i);
        }
    }
    public void onQuitBattleBtnClick()
    {
        audioMgr.PlaySFX("buttonLow");
        //CloseHttpListener();
        //In ResultScene, it tells HostBattleScene that the client quitted game.
        SetWinner(!G_isHost);
        SceneManager.LoadScene("ResultClient");
        //TODO clear user info
    }

}
