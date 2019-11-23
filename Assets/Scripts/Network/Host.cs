using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Host : MonoBehaviour
{
    //Remember to bind the Simulator script here, if you've moved the Host object.
    public Simulator simulator;
    public IntersceneBehaviour interScene;

    //public static Host host { get => host; }

    static HttpListener _httpListener;
    static Thread _responseThread;

    static bool simulateLag = false;

    private static CommandJsonList inList;
    private static CommandJsonList outList;

    public readonly bool debugflag = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (AutoPlay.isOffline) return;
        // Bind a childclass of IntersceneBehaviour in current scene
        interScene = GetComponent<IntersceneBehaviour>();
        // Start http server and establish a response thread
        // get http server
        _httpListener = interScene.GetHttpListener();
        // Set network address here
        if(_httpListener == null)
        {
            _httpListener = interScene.StartHttpListener(null);
        }
        // A scene change detector
        SceneManager.sceneUnloaded += RefreshThread;
        // start a response thread 
        Debug.Log("Starting thread...");
        closeResponseThread();
        _responseThread = new Thread(ResponseThread);
        _responseThread.Start();
    }
    private void RefreshThread(Scene s)
    {
        closeResponseThread();
    }

    public void closeResponseThread()
    {
        if (_responseThread != null && _responseThread.IsAlive)
            _responseThread.Abort();
    }
    private void OnApplicationQuit()
    {
        closeResponseThread();
        interScene.CloseHttpListener();
    }
    void ResponseThread()
    {
        var unprocessedCmds = new Queue<Command>();
        while (true)
        {
            //Simulate Lag
            if (simulateLag)
                Thread.Sleep(5);
            ////    Receiving HTTP Request    ////
            //                                  //
            if (_httpListener == null || !_httpListener.IsListening)
                break;

            //Receive HTTP request from client as a stream
            HttpListenerContext context = _httpListener.GetContext();
            StreamReader reader = new StreamReader(context.Request.InputStream);
            string resposeFromClient = reader.ReadToEnd();
            //depack the Json into CommandJsonList object
            CommandJsonList fromClient = JsonUtility.FromJson<CommandJsonList>(resposeFromClient);
            //get the commands inside CommandJsonList
            List<Command> cmds = fromClient.GetCommands();
            //check the type of each command, inside CommandJsonList
            for (int i = 0; i < cmds.Count; i++)
            {
                Command c = cmds[i];
                try
                {
                    //Schin check the classes of Commands
                    switch (c.GetType().ToString())
                    {
                        case "UnitUpdateCmd":
                        case "UnitMovedCmd":
                        case "NewUnitCmd":
                        case "HealingBuffRequestCmd":
                            if (simulator == null) // simulator is not prepared
                            {
                                unprocessedCmds.Enqueue(c);
                            }
                            else
                            {
                                foreach (var upc in unprocessedCmds)
                                {
                                    simulator.commands.Add(upc);
                                }
                                simulator.commands.Add(c);
                            }
                            break;
                        case "UnitTimerCmd":
                            if (((UnitTimerCmd)c).timerType == 2) // Accept CLIENT LOCKDOWN only
                            {
                                simulator.commands.Add(c);
                            }
                            break;
                        case "LobbyReadyCmd":
                        case "ClientJoinedCmd":
                            interScene.AddCmd(c);
                            break;
                        //case other:
                        //Dump Unknown type Command
                        default:
                            Debug.LogWarning("Unknown Command" + c.GetType());
                            Debug.Log(c);
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            ////    Responding HTTP Request    ////
            //                                   //
            //create a response JsonList
            CommandJsonList fromHost = new CommandJsonList();

            //collect requests here//
            //collect requests from simulator
            if (simulator != null)
            {
                fromHost.AddRange(simulator.GetCommandsFromHost());
                //collect TimerResetCmd Tinaxd
                fromHost.AddRange(simulator.unitTimerRequests);
                simulator.unitTimerRequests.Clear();
            }
            if (interScene != null)
            {
                List<Command> tst = interScene.GetCmd();
                fromHost.AddRange(tst);
            }

            //after collecting all the responses, convert them into binary stream
            byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(fromHost));
            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
            context.Response.Close();


            //Debug.Log("Respone given to a request.");
        }
    }
}

/*[System.Serializable]
public class PayloadFromHost
{
    public List<Unit> units;
    public float time;
}*/