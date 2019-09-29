using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScene : IntersceneBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TODO judge g_isHost, isHostWin
        //Change text
    }
    public void onReturnToMainBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
