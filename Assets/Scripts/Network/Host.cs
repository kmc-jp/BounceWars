using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class Host : MonoBehaviour
{
    HttpListener _httpListener = new HttpListener();
    Thread _responseThread;
    public Simulator simulator;
    // Start is called before the first frame update
    void Awake()
    {
        //participants = new List<int>();
        //participants.Add(manager.myid);

        Debug.Log("Starting server...");
        _httpListener.Prefixes.Add("http://localhost:5000/"); // add prefix "http://localhost:5000/"
        _httpListener.Start(); // start server (Run application as Administrator!)
        Debug.Log("Server started.");
        _responseThread = new Thread(ResponseThread);
        _responseThread.Start(); // start the response thread
    }
    private void OnApplicationQuit()
    {
        _responseThread.Abort();
    }
    void ResponseThread()
    {
        while (true)
        {
            HttpListenerContext context = _httpListener.GetContext(); // get a context
                                                                      /*                                                         // Now, you'll find the request URL in context.Request.Url
                                                                     byte[] _responseArray = Encoding.UTF8.GetBytes("<html><head><title>Localhost server -- port 5000</title></head>" +
                                                                     "<body>Welcome to the <strong>Localhost server</strong> -- <em>port 5000!</em></body></html>"); // get the bytes to response
                                                                     //XmlSerializer ser = new XmlSerializer(typeof(List<Unit>));
                                                                     */
                                                                      //BinaryFormatter binaryFormatter = new BinaryFormatter();
                                                                      // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(context.Request.InputStream);
            //BattleData data = (BattleData)binaryFormatter.Deserialize(context.Request.InputStream);
            string resposeFromClient = reader.ReadToEnd();
            //            Debug.Log(resposeFromClient);
            try
            {
                PayloadFromClient fromClient = JsonUtility.FromJson<PayloadFromClient>(resposeFromClient);
                List<Command> commands = new List<Command>();
                Debug.Log(resposeFromClient);
                for(int i = 0; i < fromClient.commands.Count; i++)
                {
                    if (!fromClient.commands[i].sent)
                    {
                        commands.Add(fromClient.commands[i]);
                    }
                    else
                    {
                        //Debug.Log("wrong command");
                    }
                }
                simulator.commands.AddRange(commands);
            }
            catch
            {
                Debug.Log("data error");
            }
            //byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            PayloadFromHost payload = new PayloadFromHost();
            payload.units = simulator.units;
            byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload));
            //context.Response.ContentLength64 = _responseArray.LongLength;
            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
            context.Response.Close(); // close the connection
            Debug.Log("Respone given to a request.");
        }
    }
}

[System.Serializable]
public class PayloadFromHost{
    public List<Unit> units;
}