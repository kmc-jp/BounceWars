using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    //Remember to bind the Simulator script here, if you've moved the Host object.
    public Simulator simulator;

    Thread _responseThread;
    // Start is called before the first frame update
    void Awake()
    {
        // start a response thread 
        _responseThread = new Thread(ResponseThread);
        _responseThread.Start();
    }
    private void OnApplicationQuit()
    {
        _responseThread.Abort();
    }
    void ResponseThread()
    {
        while (true)
        {
            ////        Sending HTTP Request        ////
            //                                        //
            // Establish an http request
            WebRequest request = WebRequest.Create("http://localhost:5000");
            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "text/json";
            request.Method = "POST";

            //create a request JsonList
            CommandJsonList fromClient = new CommandJsonList();
            //collect requests here//
            //collect requests from simulator
            fromClient.AddRange(simulator.commands);
            simulator.SetCommandsSent();

            //fromClient.AddRange(List<Command>);


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
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server. 
            // The using block ensures the stream is automatically closed. 
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
                        //For Command subclass types, refer to Command.cs comments
                        switch (fromHost.type[i])
                        {
                            //UnitUpdateCmd
                            case -1:
                                c = (JsonUtility.FromJson<UnitUpdateCmd>(fromHost.commandsJson[i]));
                                simulator.commands.Add(c);
                                break;
                            //case other:
                            //Dump Unknown type Command
                            default:
                                Debug.LogWarning("Unknown Command");
                                Debug.Log((JsonUtility.FromJson<Command>(fromClient.commandsJson[i])));
                                break;
                        }
                        //Debug
                        //timer.Stop();
                        //simulator.commands.AddRange()
                        //simulator.units = data.units;
                        //simulator.time = data.time + timer.ElapsedMilliseconds * 0.001f * 0.5f;
                    }
            }

            // Close the response.  
            response.Close();
        }
    }
}


