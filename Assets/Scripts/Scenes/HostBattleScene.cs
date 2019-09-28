using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostBattleScene : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Command> commands;
    private List<Command> cmdFromHost;

    void Start()
    {
        commands = new List<Command>();
        cmdFromHost = new List<Command>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public List<Command> GetCommandsFromHost()
    {
        List<Command> outCmdTmp = cmdFromHost.ConvertAll(cmd => new Command());
        cmdFromHost = new List<Command>();
        return outCmdTmp;
    }
}
