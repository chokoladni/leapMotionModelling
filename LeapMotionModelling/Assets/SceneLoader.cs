using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public void loadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void quitGame() {
        Application.Quit();
    }
}
