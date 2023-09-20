using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static Transform cameraTransform;

    void Awake() {
        cameraTransform = GetComponent<Transform>();
    }


    public static void moveCameraGlobally(Vector3 delta) {
        cameraTransform.position = cameraTransform.position + delta;
    }

    public static void moveCameraLocally(Vector3 delta) {
        cameraTransform.localPosition = cameraTransform.localPosition + delta;
    }
}
