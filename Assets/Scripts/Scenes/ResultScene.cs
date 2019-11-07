using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScene : IntersceneBehaviour
{
    public static HttpListener _httpListener;

    private bool isClientInResultScene;

    // Start is called before the first frame update
    void Start()
    {
        // Client tell host its entering result scene, 6is ID for client result scene.
        if(!G_isHost)
            tosendCmds.Add((Command)new ClientJoinedCmd(6));

        isClientInResultScene = false;

        Text resultTitleSelf = GameObject.Find("ResultTitleSelf").GetComponent<Text>();
        Text ResultTitleOpponent = GameObject.Find("ResultTitleOpponent").GetComponent<Text>();

        Text ButtonText = GameObject.Find("ReturnBtn").GetComponentInChildren<Text>();

        // set players' names
        GameObject.Find("selfName").GetComponent<Text>().text = G_username;
        GameObject.Find("opponentName").GetComponent<Text>().text = G_opponentName;

        if (G_isHost == IsHostWin)// Current Player wins.
        {
            resultTitleSelf.text = "You Win!";
            ResultTitleOpponent.text = "Loser";
            ButtonText.text = "Yeah!";
        }
        else// Current Player loses.
        {
            resultTitleSelf.text = "You Loses!";
            ResultTitleOpponent.text = "Winner";
            ButtonText.text = "Alright..";
        }
    }
    void Update()
    {
        //Check cmmands
        for (int i = receivedCmds.Count - 1; i >= 0; i--)
        {
            Command cTemp = receivedCmds[i];
            // the client is already in result scene.
            // CLIENT send again in case of Package lose
            if (cTemp is GameSetCmd)
            {
                tosendCmds.Add((Command)new ClientJoinedCmd(6)); // 6 for ResultClient Scene.
            }
            // HOST knows client is already in result scene.
            if (cTemp is ClientJoinedCmd & ((ClientJoinedCmd)cTemp).sceneID == 6)
            {
                Debug.Log("ClientInResultScene");
                    isClientInResultScene = true;
            }
            // The unprocessed commands are saved for other child of IntersceneBehaviour
            else
            {
                //remove unnecessary leftover commands.
                receivedCmds.RemoveAt(i);
                continue;
            }
        }
    }
    public void onReturnToMainBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public override List<Command> GetCmd()
    {
        if (G_isHost && isClientInResultScene == false)
        {
            //Tell client to go to Result Scene
            tosendCmds.Add((Command)new GameSetCmd(IsHostWin));
        }
        return base.GetCmd();
    }

    /*private void networkSetUp()
    {
        // Bind a childclass of IntersceneBehaviour in current scene
        IntersceneBehaviour interScene = GetComponent<IntersceneBehaviour>();
        // Start http server and establish a response thread
        // get http server
        _httpListener = interScene.GetHttpListener();
        // Set network address here
        if (_httpListener == null)
            _httpListener = interScene.StartHttpListener("http://localhost:5000/");
        // A scene change detector
        //SceneManager.sceneUnloaded += RefreshThread;
        // start a response thread 
        Debug.Log("Starting thread...");
        //closeResponseThread();
        Thread _responseThread = new Thread(sendGameSetCommandToClient);
        _responseThread.Start();
    }

    public void sendGameSetCommandToClient()
    {
        if (_httpListener == null || !_httpListener.IsListening) return;
        Debug.Log("CameSendComand");
        HttpListenerContext context = _httpListener.GetContext();
        byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Command[] { new GameSetCmd(isHostWin) }));
        context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
        context.Response.Close();
    }*/
}
