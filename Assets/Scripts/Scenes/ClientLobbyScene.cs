using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientLobbyScene : IntersceneBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public void onStartGameBtnClick()
    {
        SceneManager.LoadScene("Client");
    }
    public void onReturnBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
