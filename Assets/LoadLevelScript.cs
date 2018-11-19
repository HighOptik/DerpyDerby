using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelScript : MonoBehaviour {

    public int levelToLoad;
    public void LoadRace()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
