using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GracesGames.SimpleFileBrowser.Scripts;
using System.IO;
using Boo.Lang.Runtime;
using UnityEngine.SceneManagement;

public class ObjLoader : MonoBehaviour {

    public static string filePath;
    private static string loadedObj;

    public GameObject FileBrowserPrefab;

    public string[] FileExtensions;

    public bool PortraitMode;

    public void createNew(string path) {
        if(!path.EndsWith(".obj")) {
            path += ".obj";
        }
        filePath = path;

        if(File.Exists(path)) {
            Debug.Log("File already exists!");
            return;
        }

        File.Create(path);
        loadedObj = "";
        Debug.Log("kreiram! " + path);
        SceneManager.LoadScene("Starting");
    }

	public void loadObj(string path) {
        if (!path.EndsWith(".obj")) {
            path += ".obj";
        }
        filePath = path;
        
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadToEnd();
        reader.Close();

        Debug.Log("loadam... " + path);
        Debug.Log(text);
        loadedObj = text;
        SceneManager.LoadScene("Starting");
    }

    public void saveObj(string path) {

        while(File.Exists(path)) {
            path.Replace(".obj", "1.obj");
        }
        
        string obj = GestureRecognizer.meshWrapper.meshToObj2();
        System.IO.File.WriteAllText(@path, obj);
    }

    public static string getLoadedObj() {
        return loadedObj;
    }

    public void openFileBrowser(bool save) {
        // Create the file browser and name it
        GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
        fileBrowserObject.name = "FileBrowser";
        // Set the mode to save or load
        FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
        fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
        if (save) {
            fileBrowserScript.SaveFilePanel("DemoText", FileExtensions);
            // Subscribe to OnFileSelect event (call SaveFileUsingPath using path) 
            fileBrowserScript.OnFileSelect += saveObj;
        } else {
            fileBrowserScript.OpenFilePanel(FileExtensions);
            // Subscribe to OnFileSelect event (call LoadFileUsingPath using path) 
            fileBrowserScript.OnFileSelect += loadObj;
        }
    }

}
