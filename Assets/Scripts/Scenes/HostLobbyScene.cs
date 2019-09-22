using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostLobbyScene : IntersceneBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void onStartGameBtnClick()
    {
        //this.GetComponent<HostLobby>().stopPort();
        SceneManager.LoadScene("Host_c");
    }
    public void onReturnBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void setOppoUsername(string name)
    {

    }
}
