using UnityEngine;
using Leap.Unity;
using Leap;
using System.Collections.Generic;

public class LeapUtil : MonoBehaviour {

    public static Transform LeapTransform;
    private static Controller controller;

    void Awake() {
        LeapTransform = GetComponent<Transform>();
        controller = new Controller();
    }

    public static List<Hand> getHands() {
        return controller.Frame().Hands;
    }

    public static Vector3 LeapVectorToVector3(Vector vector) {
        Vector3 position = LeapTransform.position;
        
        Vector3 local = new Vector3(-vector.x, -vector.z, vector.y);
        local = LeapTransform.rotation * local;

        return local;
    }

    public static Vector3 LeapVectorToLocalScaledVector3(Vector vector) {
        float factor = UnityMatrixExtension.MM_TO_M;
        return new Vector3(-vector.x * factor, -vector.z * factor, (vector.y + 130) * factor);
    }
    
    public static Vector3 LeapVectorToScaledVector3(Vector vector) {
        Vector3 position = LeapTransform.position;
        float factor = UnityMatrixExtension.MM_TO_M;

        Vector3 local = new Vector3(-vector.x * factor, -vector.z * factor, (vector.y + 130) * factor);
        local = LeapTransform.rotation * local;
        local = local + position;

        return local;

        //return new Vector3(-vector.x * factor + position.x, -vector.z * factor + position.y, vector.y * factor + position.z);
    }
}
