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
    static bool simulateLag = false;
    // Start is called before the first frame update
    void Awake()
    {
        //UnitUpdateCmd testA = new UnitUpdateCmd();
        //testA.hp = -10;
        //Command testB = (Command)testA;
        //Debug.Log(JsonUtility.ToJson(testB));
        //Command testC = JsonUtility.FromJson<Command>(JsonUtility.ToJson(testB));
        //Debug.Log(testB.GetType());
        //UnitUpdateCmd testC = JsonUtility.FromJson<UnitUpdateCmd>(JsonUtility.ToJson(testB));
        //Debug.Log(testC.hp);
        //UnitUpdateCmd testD = (UnitUpdateCmd)testC;
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
            CommandJsonList fromHost = new CommandJsonList();

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
            if (simulateLag)
                Thread.Sleep(500);
            //            Debug.Log(resposeFromClient);
            try
            {
                CommandJsonList fromClient = JsonUtility.FromJson<CommandJsonList>(resposeFromClient);
                List<Command> commands = new List<Command>();
                if (fromClient.commandsJson.Count != 0)
                {
                    //Debug.Log(resposeFromClient);
                }
                /*
                for (int i = 0; i < fromClient.commandsJson.Count; i++)
                {
                    Command c = null;
                    switch (fromClient.type[i])
                    {
                        case 0:
                            c = (JsonUtility.FromJson<UnitMovedCmd>(fromClient.commandsJson[i]));
                            break;
                        case -2:
                            //returnList.Add()
                            break;
                    }
                    if (c == null) continue;
                    if (!(c.sent))
                    {
                        c.processed = false;
                        commands.Add(c);
                        Debug.Log(((UnitMovedCmd)c).vx);
                    }

                }*/
                //Debug.Log((UnitUpdateCmd)commands[0]);
                //simulator.commands.AddRange(commands);
                simulator.commands.AddRange(fromClient.GetCommands());//use getcommands later
            }
            catch
            {
                Debug.Log("data error");
            }
            fromHost.AddRange(simulator.GetCommandsFromHost());
            //lag test
            //byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            /*
            PayloadFromHost payload = new PayloadFromHost();
            payload.units = simulator.units;
            payload.time = simulator.time;*/
            //byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload));
            byte[] _responseArray = Encoding.UTF8.GetBytes(JsonUtility.ToJson(fromHost));

            //context.Response.ContentLength64 = _responseArray.LongLength;
            if (simulateLag)
                Thread.Sleep(500);
            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
            context.Response.Close(); // close the connection
            Debug.Log("Respone given to a request.");
        }
    }
}

/*[System.Serializable]
public class PayloadFromHost
{
    public List<Unit> units;
    public float time;
}*/


[System.Serializable] 
public class CommandJsonList
{
    public List<string> commandsJson;
    public List<int> type;

    public CommandJsonList()
    {
        commandsJson = new List<string>();
        type = new List<int>();
    }
    public CommandJsonList(List<Command> commands)
    {
        commandsJson = new List<string>();
        type = new List<int>();
        for (int i = 0; i < commands.Count; i++)
        {
            commandsJson.Add(JsonUtility.ToJson(commands[i]));
            type.Add(commands[i].type);
        }
    }
    public void Add(Command c)
    {
        commandsJson.Add(JsonUtility.ToJson(c));
        type.Add(c.type);
    }
    public void AddRange(List<Command> cs)
    {
        for (int i = 0; i < cs.Count; i++)
        {
            Command c = cs[i];
            commandsJson.Add(JsonUtility.ToJson(c));
            type.Add(c.type);
        }
    }
    public List<Command> GetCommands()
    {
        List<Command> commands = new List<Command>();
        for (int i = 0; i < commandsJson.Count; i++)
        {
            Command c = null;
            switch (type[i])
            {
                case 0:
                    c = (JsonUtility.FromJson<UnitMovedCmd>(commandsJson[i]));
                    break;
                case -1:
                    c = (JsonUtility.FromJson<UnitUpdateCmd>(commandsJson[i]));
                    break;
            }
            if (c == null) continue;
            if (!(c.sent))
            {
                c.processed = false;
                commands.Add(c);
                //Debug.Log(((UnitMovedCmd)c).vx);
            }

        }
        return commands;
    }
}
