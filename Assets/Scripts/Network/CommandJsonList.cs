using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CommandJsonList
{
    //Dont opearte on the params directly
    //may cause problems
    public List<string> commandsJson;
    public List<string> types;


    public CommandJsonList()
    {
        commandsJson = new List<string>();
        types = new List<string>();
    }
    public CommandJsonList(List<Command> commands)
    {
        commandsJson = new List<string>();
        types = new List<string>();
        AddRange(commands);
    }
    public void Add(Command c)
    {
        commandsJson.Add(JsonUtility.ToJson(c));
        types.Add(c.GetType().ToString());
    }
    public void AddRange(List<Command> cs)
    {
        if (cs != null)
            for (int i = 0; i < cs.Count; i++)
            {
                Add(cs[i]);
            }
    }
    //Schin Merge another CommandJsonList
    public void MergeJsonList(CommandJsonList newCJL)
    {
        List<string> newCmds = newCJL.commandsJson;
        List<string> newTs = newCJL.types;
        for (int i = 0; i < (newCmds).Count; i++)
        {
            commandsJson.Add(newCmds[i]);
            types.Add(newTs[i]);
        }
    }
    public List<Command> GetCommands()
    {
        //Schin Get commands using reflection
        List<Command> commands = new List<Command>();
        for (int i = 0; i < commandsJson.Count; i++)
        {
            Type tp = Type.GetType(types[i]);
            Command c;
            //Type a = typeof(JsonUtility);
            //System.Reflection.MethodInfo b = a.GetMethod("FromJson", new[] { typeof(string) });
            //System.Reflection.MethodInfo cc = b.MakeGenericMethod(tp);
            //object d = cc.Invoke(this, new object[] { commandsJson[i]});
            //c = (Command)d;
            //JsonUtility.FromJson<SubclassOfCommand>(CommandsJsonString);
            c = (Command)typeof(JsonUtility).
                    GetMethod("FromJson", new[] { typeof(string) }).
                    MakeGenericMethod(tp).
                    Invoke(this, new object[] { commandsJson[i] });
            if (c == null) continue;
            commands.Add(c);
            /*if (!(c.sent))
            {
                c.processed = false;
                commands.Add(c);
                //Debug.Log(((UnitMovedCmd)c).vx);
            }*/
        }
        return commands;
    }
    public int Count()
    {
        if (commandsJson.Count != types.Count)
            Debug.LogWarning("CommandJsonList Json and type number don't match");
        return commandsJson.Count;
    }
}
