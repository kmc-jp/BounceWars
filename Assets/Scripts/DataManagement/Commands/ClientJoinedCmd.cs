using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientJoinedCmd : Command
{
    public bool isClientJoined;
    public ClientJoinedCmd()
    {
        isClientJoined = true;
        type = 106;
    }
}
