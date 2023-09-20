using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Leap.Unity;
using Leap;

public class GestureRecognizer : MonoBehaviour {
    
    Controller controller;
    public float radius = 0.02f;
    public GameObject representation;
    public Material selectedMat;
    public Material unselectedMat;
    
    private OrderedDictionary leftHandMap = new OrderedDictionary();
    private OrderedDictionary rightHandMap = new OrderedDictionary();

    public static MeshWrapper3 meshWrapper;
    public VertexVisualizer visualizer;

    void Start () {
        controller = new Controller();

        Mesh m = new Mesh();
        m.subMeshCount = 2;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) {
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = m;
        meshWrapper = new MeshWrapper3(m);
        visualizer = new VertexVisualizer(meshWrapper, representation, unselectedMat, selectedMat);

        string obj = ObjLoader.getLoadedObj();
        if(!string.IsNullOrEmpty(obj)) {
            meshWrapper.loadFromObj(obj);
        }

        PointGesture point = this.gameObject.AddComponent<PointGesture>();
        leftHandMap.Add(point, new SelectVertexAction(meshWrapper, radius));
        rightHandMap.Add(point, new SelectVertexAction(meshWrapper, radius));

        GrabGesture grab = this.gameObject.AddComponent<GrabGesture>();
        leftHandMap.Add(grab, new MoveCameraAction());
        rightHandMap.Add(grab, new TranslateSelectionAction(meshWrapper));

        PinchGesture pinch = this.gameObject.AddComponent<PinchGesture>();
        pinch.activationDist = 20.0f;
        pinch.maintainDist = 50.0f;
        leftHandMap.Add(pinch, new AddVertexAction(meshWrapper));
        rightHandMap.Add(pinch, new AddVertexAction(meshWrapper));

        FlatHandGesture flatHand = this.gameObject.AddComponent<FlatHandGesture>();
        flatHand.startAngle = 10.0f;
        flatHand.maintainAngle = 30.0f;
        leftHandMap.Add(flatHand, new CreateTrianglesAction(meshWrapper));
        rightHandMap.Add(flatHand, new CreateTrianglesAction(meshWrapper));

        ThumbUpGesture thumbsUp = this.gameObject.AddComponent<ThumbUpGesture>();
        thumbsUp.startAngle = 15.0f;
        thumbsUp.maintainAngle = 30.0f;
        leftHandMap.Add(thumbsUp, new InvertNormalsAction(meshWrapper));
        rightHandMap.Add(thumbsUp, new InvertNormalsAction(meshWrapper));

        ThumbDownGesture thumbsDown = this.gameObject.AddComponent<ThumbDownGesture>();
        thumbsDown.startAngle = 35.0f;
        thumbsDown.maintainAngle = 60.0f;
        leftHandMap.Add(thumbsDown, new DeleteSelectionAction(meshWrapper));
        rightHandMap.Add(thumbsDown, new DeleteSelectedTrianglesAction(meshWrapper));
    }

    void Update () {
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;

        foreach(Hand hand in hands) {
            bool detected = false;
            if(hand.IsLeft) {
                foreach(DictionaryEntry pair in leftHandMap) {
                    GestureState state = ((MyGesture)pair.Key).getState(hand);
                    if(detected) {
                        ((MyGesture)pair.Key).resetDetection(hand);
                        continue;
                    }

                    if(state != GestureState.STOP && state != GestureState.INVALID) { 
                        ((GestureAction)pair.Value).perform(hand, state);
                        detected = true;
                    }
                }
            } else {
                foreach (DictionaryEntry pair in rightHandMap) {
                    GestureState state = ((MyGesture)pair.Key).getState(hand);
                    if (detected) {
                        ((MyGesture)pair.Key).resetDetection(hand);
                        continue;
                    }

                    if (state != GestureState.STOP && state != GestureState.INVALID) {
                        ((GestureAction)pair.Value).perform(hand, state);
                        detected = true;
                    }
                }
            }
        }
	}
}
