using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientJoinedCmd : Command
{
    public bool isClientJoined;
    // the new scene client is in.
    public int sceneID;
    public ClientJoinedCmd(int sceneID)
    {
        isClientJoined = true;
        this.sceneID = sceneID;
        type = 106;
    }
}
