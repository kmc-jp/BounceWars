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

    static bool simulateLag = true;

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
    }
    private void hostTimedOut()
    {
        Debug.LogWarning("Host Timed Out Several Times");
        closeResponseThread();
        SceneManager.LoadScene("MainMenu");
    }
    void ResponseThread()
    {
        int timedoutNum = 0;
        while (true)
        {
            //Simulate Lag
            if (simulateLag)
                Thread.Sleep(5);
            ////        Sending HTTP Request        ////
            //                                        //
            // Establish an http request
            WebRequest request = WebRequest.Create("http://localhost:5000");
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
            if (interScene != null) fromClient.AddRange(interScene.GetCmd());
            //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            //timer.Start();
            //after collecting all the requests, convert them into binary stream
            byte[] binary = Encoding.UTF8.GetBytes(JsonUtility.ToJson(fromClient));

            // Send the requests
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(binary, 0, binary.Length);
                postStream.Close();
            }

            ////        receiving HTTP Request        ////
            //                                          //
            try
            {
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
                        for (int i = 0; i < fromHost.commandsJson.Count; i++)
                        {
                            Command c = null;
                            try
                            {
                                //For Command subclass types, refer to Command.cs comments
                                Debug.Log(fromHost.type[i]);
                                switch (fromHost.type[i])
                                {
                                    //UnitUpdateCmd
                                    case -1:
                                        c = (JsonUtility.FromJson<UnitUpdateCmd>(fromHost.commandsJson[i]));
                                        simulator.commands.Add(c);
                                        break;
                                    //UnitTimerCmd
                                    case 2:
                                        c = (JsonUtility.FromJson<UnitTimerCmd>(fromHost.commandsJson[i]));
                                        simulator.commands.Add(c);
                                        break;
                                    //LobbyReadyCmd
                                    case 101:
                                        c = (JsonUtility.FromJson<LobbyReadyCmd>(fromHost.commandsJson[i]));
                                        interScene.AddCmd(c);
                                        break;
                                    //LobbyStartgameCmd
                                    case 105:
                                        c = (JsonUtility.FromJson<LobbyStartgameCmd>(fromHost.commandsJson[i]));
                                        interScene.AddCmd(c);
                                        break;
                                    //case other:
                                    //Dump Unknown type Command
                                    default:
                                        Debug.LogWarning("Unknown Command" + fromHost.type[i]);
                                        Debug.Log((JsonUtility.FromJson<Command>(fromClient.commandsJson[i])));
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
            catch(TimeoutException we)
            {
                if(timedoutNum++ >= 5)
                {
                    hostTimedOut();
                    break;
                }
            }
                
        }
    }
}


