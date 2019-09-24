using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void onReturnToMainBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
