using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class Host : MonoBehaviour
{
    //Remember to bind the Simulator script here, if you've moved the Host object.
    public Simulator simulator;

    HttpListener _httpListener = new HttpListener();
    Thread _responseThread;

    static bool simulateLag = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Start http server and establish a response thread
        Debug.Log("Starting server...");
        // Set network address here
        _httpListener.Prefixes.Add("http://localhost:5000/");
        _httpListener.Start();
        Debug.Log("Server started.");
        _responseThread = new Thread(ResponseThread);
        _responseThread.Start();
    }
    private void OnApplicationQuit()
    {
        _responseThread.Abort();
        _httpListener.Close();
    }
    void ResponseThread()
    {
        while (true)
        {
            try
            {
                ////    Receiving HTTP Request    ////
                //                                  //
                //Receive HTTP request from client as a stream
                HttpListenerContext context = _httpListener.GetContext();
                StreamReader reader = new StreamReader(context.Request.InputStream);
                string resposeFromClient = reader.ReadToEnd();
                if (simulateLag)
                    Thread.Sleep(500);
                //depack the Json into CommandJsonList object
                CommandJsonList fromClient = JsonUtility.FromJson<CommandJsonList>(resposeFromClient);
                //check the type of each command, inside CommandJsonList
                for (int i = 0; i < fromClient.commandsJson.Count; i++)
                {
                    Command c = null;
                    //For Command subclass types, refer to Command.cs comments
                    switch (fromClient.type[i])
                    {
                        //UnitUpdateCmd
                        case -1:
                            c = (JsonUtility.FromJson<UnitUpdateCmd>(fromClient.commandsJson[i]));
                            simulator.commands.Add(c);
                            break;
                        //UnitMovedCmd
                        case 1:
                            c = (JsonUtility.FromJson<UnitMovedCmd>(fromClient.commandsJson[i]));
                            simulator.commands.Add(c);
                            break;
                        //UnitTimerCmd
                        case 2:
                            break; // Ignore UnitTimerCmd from clients
                        //case other:
                        //Dump Unknown type Command
                        default:
                            Debug.LogWarning("Unknown Command");
                            Debug.Log((JsonUtility.FromJson<Command>(fromClient.commandsJson[i])));
                            break;
                    }
                }

                ////    Responding HTTP Request    ////
                //                                   //
                //create a response JsonList
                CommandJsonList fromHost = new CommandJsonList();

                //collect requests here//
                //collect requests from simulator
                fromHost.AddRange(simulator.GetCommandsFromHost());
                //collect TimerResetCmd Tinaxd
                fromHost.AddRange(simulator.unitTimerRequests);
                simulator.unitTimerRequests.Clear();

                //fromHost.AddRange(List<Command>);


                //after collecting all the responses, convert them into binary stream
                byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(fromHost));

                //write the response 
                if (simulateLag)
                    Thread.Sleep(500);
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
                context.Response.Close();


                //Debug.Log("Respone given to a request.");
            }
            catch(System.Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }
}

/*[System.Serializable]
public class PayloadFromHost
{
    public List<Unit> units;
    public float time;
}*/