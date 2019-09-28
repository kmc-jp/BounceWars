using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        AddRange(commands);
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
    //Schin Merge another CommandJsonList
    public void MergeJsonList(CommandJsonList newCJL)
    {
        List<string> newCmds = newCJL.commandsJson;
        List<int> newTypes = newCJL.type;
        for (int i = 0; i < (newCmds).Count; i++)
        {
            commandsJson.Add(newCmds[i]);
            type.Add(newTypes[i]);
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
    public int Count()
    {
        if (commandsJson.Count != type.Count)
            Debug.LogWarning("CommandJsonList Json and type number don't match");
        return type.Count;
    }
}
