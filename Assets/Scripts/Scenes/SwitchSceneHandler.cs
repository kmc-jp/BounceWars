using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersceneBehaviour : MonoBehaviour
{
    // Inter-Scene data
    private static bool g_isHost;
    private static string g_username;

    public static bool G_isHost { get => g_isHost; set => g_isHost = value; }
    public static string G_username { get => g_username; set => g_username = value; }
}
