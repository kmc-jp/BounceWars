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
    // Start is called before the first frame update
    void Start()
    {
        GameObject a = GameObject.Find("ResultTitle");
        Text text = a.GetComponent<Text>();
        if (IntersceneBehaviour.g_isHost)
        {
            if (isHostWin) text.text = "You Win!";
            else text.text = "You Lose...";
        }
        else
        {
            if (isHostWin) text.text = "You Lose...";
            else text.text = "You Win!";
        }

            //if (IntersceneBehaviour.g_isHost) networkSetUp();
            //TODO judge g_isHost, isHostWin
            //Change text
        }
    public void onReturnToMainBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
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
