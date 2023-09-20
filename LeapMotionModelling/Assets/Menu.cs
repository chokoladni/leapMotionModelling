using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    public GameObject main;
    public Canvas canvas;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {
            bool mainActive = main.activeSelf;
            main.SetActive(!mainActive);
            main.GetComponent<GestureRecognizer>().visualizer.show(!mainActive);
            canvas.gameObject.SetActive(mainActive);

            if (canvas.isActiveAndEnabled) {
                canvas.transform.position = LeapUtil.LeapTransform.position + new Vector3(0, 0, 0.45f);
                canvas.transform.rotation = LeapUtil.LeapTransform.rotation;
            }
        }
	}
}
