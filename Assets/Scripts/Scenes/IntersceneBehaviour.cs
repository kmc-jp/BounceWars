using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class IntersceneBehaviour : MonoBehaviour
{
    // Inter-Scene data
    protected static bool g_isHost;
    protected static string g_username;
    protected static List<Unit> units;
    protected static bool isHostWin;

    //For network
    static HttpListener _httpListener;
    protected List<Command> receivedCmds;
    protected List<Command> tosendCmds;
    
    public IntersceneBehaviour()
    {
        //This is called every time a new scene is loaded. 
        //Careful when child classes implement their own _init function.
        if (tosendCmds == null) tosendCmds = new List<Command>();
        if (receivedCmds == null) receivedCmds = new List<Command>();
    }
    public bool G_isHost { get => g_isHost; set => g_isHost = value; }
    public string G_username { get => g_username; set => g_username = value; }
    public List<Unit> Units { get => units; set => units = value; }

    //For network System
    public HttpListener GetHttpListener()
    {
        return _httpListener;
    }
    public HttpListener StartHttpListener(string ipAddr)
    {
        if (_httpListener == null)
        {
            Debug.Log("Starting server...");
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://localhost:5000/");
            _httpListener.Start();
        }
        return _httpListener;
    }
    public void CloseHttpListener()
    {
        if (_httpListener != null)
            _httpListener.Close();
        _httpListener = null;
    }
    //Add the network received commands
    public virtual void AddCmd(Command cmd)
    {
        receivedCmds.Add(cmd);
    }
    //Give the commands ready to send
    public virtual List<Command> GetCmd()
    {
        List<Command> outCmdTmp = new List<Command>(tosendCmds);
        tosendCmds = new List<Command>();
        return outCmdTmp;
    }
    //Get current scene, can only be used in MainThread
    public int CurrScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static void SetWinner(bool p)
    {
        isHostWin = p;
    }

}
