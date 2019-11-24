using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    //Remember to bind the Simulator script here, if you've moved the Host object.
    public Simulator simulator;
    public IntersceneBehaviour interScene;

    static Thread _responseThread;

    static bool simulateLag = false;

    private bool threadAborted = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Bind a childclass of IntersceneBehaviour in current scene
        interScene = GetComponent<IntersceneBehaviour>();
        // A scene change detector
        SceneManager.sceneUnloaded += RefreshThread;
        // start a response thread 
        Debug.Log("Starting thread...");
        closeResponseThread();
        _responseThread = new Thread(ResponseThread);
        threadAborted = false;
        _responseThread.Start();
    }
    private void RefreshThread(Scene s)
    {
        closeResponseThread();
    }
    public void closeResponseThread()
    {
        threadAborted = true;
        _responseThread = null;
    }
    private void OnApplicationQuit()
    {
        closeResponseThread();
    }
    private void hostTimedOut()
    {
        Debug.LogError("Host Timed Out Several Times");
        closeResponseThread();
        if (simulator != null) // simulator is null in ResultScene
            simulator.openMainMenuScene();
    }
    void ResponseThread()
    {
        int timedoutNum = 0;
        var unprocessedCmds = new Queue<Command>();
        while (!threadAborted)
        {
            //Simulate Lag
            if (simulateLag)
                Thread.Sleep(5);
            ////        Sending HTTP Request        ////
            //                                        //
            // Establish an http request
            string targetURL = "http://" + interScene.TargetURL + ":5000/";
            WebRequest request = WebRequest.Create(targetURL);
            request.Timeout = 1000;
            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "text/json";
            request.Method = "POST";
            //create a request JsonList
            CommandJsonList fromClient = new CommandJsonList();
            //collect requests here//
            //collect requests from simulator
            if (simulator != null)
            {
                fromClient.AddRange(simulator.commands);
                simulator.SetCommandsSent();
            }
            if (interScene != null)
                fromClient.AddRange(interScene.GetCmd());
            //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            //timer.Start();
            //after collecting all the requests, convert them into binary stream
            byte[] binary = Encoding.UTF8.GetBytes(JsonUtility.ToJson(fromClient));

            try
            {
                    // Send the requests
                    using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(binary, 0, binary.Length);
                }

            ////        receiving HTTP Request        ////
            //                                          //
                // Get the stream containing content returned by the server. 
                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                {

                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    //depack the Json into CommandJsonList object
                    CommandJsonList fromHost = JsonUtility.FromJson<CommandJsonList>(responseFromServer);
                    // If the response Json is empty, there's a big chance the host is down.
                    if (fromHost != null)
                    {
                        //get the commands inside CommandJsonList
                        List<Command> cmds = fromHost.GetCommands();
                        for (int i = 0; i < cmds.Count; i++)
                        {
                            Command c = cmds[i];
                            try
                            {
                                //Schin check the classes of Commands
                                switch (c.GetType().ToString())
                                {
                                    case "UnitUpdateCmd":
                                    case "UnitTimerCmd":
                                    case "GameSetCmd":
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
                                    case "LobbyReadyCmd":
                                    case "LobbyStartgameCmd":
                                        interScene.AddCmd(c);
                                        break;
                                    //case other:
                                    //Dump Unknown type Command
                                    default:
                                        Debug.LogWarning("Unknown Command" + c.GetType());
                                        Debug.Log(c);
                                        break;
                                }
                                //Debug
                                //timer.Stop();
                                //simulator.commands.AddRange()
                                //simulator.units = data.units;
                                //simulator.time = data.time + timer.ElapsedMilliseconds * 0.001f * 0.5f;
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogWarning(e);
                            }
                        }
                    }
                }
            }
            catch(WebException we)
            {
                Debug.LogWarning(we.StackTrace);
                Debug.LogWarning(we.Message);
                if (we.Status == WebExceptionStatus.Timeout && timedoutNum++ >= 5)
                {
                    hostTimedOut();
                    break;
                }
                // Possibly Host has quit game
                // Assume Host loses and client wins.
                //if (simulator!= null)
                //    simulator.commands.Add(new GameSetCmd(false)); 
            }
                
        }
        Debug.Log("Client thread ended");
    }
}


