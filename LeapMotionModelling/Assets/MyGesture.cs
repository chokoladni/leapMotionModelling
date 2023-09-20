using System;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public abstract class MyGesture : MonoBehaviour {

    private static Controller c;
    
    public abstract GestureState isDetected(Hand hand);

    private Dictionary<int, GestureState> handsToStates;

    public GestureState getState(Hand hand) {
        if(handsToStates == null) {
            return GestureState.INVALID;
        }
        if (!handsToStates.ContainsKey(hand.Id)) {
            return GestureState.INVALID;
        }

        return handsToStates[hand.Id];
    }

    void Awake() {
        c = new Controller();
    }

    void Start() {
        handsToStates = new Dictionary<int, GestureState>();
    }

    void Update() {
        List<Hand> hands = c.Frame().Hands;
        foreach(Hand hand in hands) {
            if(handsToStates == null) {
                break;
            }
            if(!handsToStates.ContainsKey(hand.Id)) {
                handsToStates.Add(hand.Id, GestureState.STOP);
            }
            handsToStates[hand.Id] = isDetected(hand);
        }
    }

    public void resetDetection(Hand hand) {
        handsToStates[hand.Id] = GestureState.STOP;
    }
}

public enum GestureState {
    START, ONGOING, STOP, INVALID
}

public class PointGesture : MyGesture {

    public override GestureState isDetected(Hand hand) {
        List<Finger> fingers = hand.Fingers;

        for (int i = 1; i < fingers.Count; i++) { //skips thumb
            if (i == 1) {
                if (!fingers[1].IsExtended) { //index finger must be extended
                    return GestureState.STOP;
                }
                continue;
            }
            if(fingers[i].IsExtended) { //all other fingers must not be extended
                return GestureState.STOP;
            }
        }
        if(getState(hand) == GestureState.STOP) {
            return GestureState.START;
        } else {
            return GestureState.ONGOING;
        }
    }
}

public class PinchGesture : MyGesture {

    public float activationDist;
    public float maintainDist;

    public PinchGesture(float activationDist, float maintainDist) {
        this.activationDist = activationDist;
        this.maintainDist = maintainDist;
    }
    public override GestureState isDetected(Hand hand) {
        List<Finger> fingers = hand.Fingers;

        Finger thumb = fingers[0];
        Finger index = fingers[1];

        float distance = thumb.TipPosition.DistanceTo(index.TipPosition);

        if (getState(hand) == GestureState.STOP && distance <= activationDist) {
            return GestureState.START;
        }

        if (getState(hand) != GestureState.STOP && distance <= maintainDist) {
            return GestureState.ONGOING;
        }

        return GestureState.STOP;
    }
}

public class GrabGesture : MyGesture {

    public override GestureState isDetected(Hand hand) {
        foreach (Finger finger in hand.Fingers) {
            if (finger.IsExtended) {
                return GestureState.STOP;
            }
        }
        if (getState(hand) == GestureState.STOP) {
            return GestureState.START;
        } else {
            return GestureState.ONGOING;
        }
    }
}

public class FlatHandGesture : MyGesture {

    public float startAngle;
    public float maintainAngle;
    private int count = 0;

    public FlatHandGesture(float startAngle, float maintainAngle) {
        this.startAngle = startAngle;
        this.maintainAngle = maintainAngle;
    }

    public override GestureState isDetected(Hand hand) {
        foreach (Finger finger in hand.Fingers) {
            if (!finger.IsExtended) {
                return GestureState.STOP;
            }
        }

        count++;
        //        Vector3 palmNormal = new Vector3(hand.PalmNormal.x, hand.PalmNormal.y, hand.PalmNormal.z);
        Vector3 palmNormal = LeapUtil.LeapVectorToVector3(hand.PalmNormal);

        float angle = Vector3.Angle(LeapUtil.LeapTransform.up, palmNormal);

        if (getState(hand) == GestureState.STOP && angle <= startAngle) {
            return GestureState.START;
        }

        if (getState(hand) != GestureState.STOP && angle <= maintainAngle) {
            return GestureState.ONGOING;
        }

        return GestureState.STOP;
    }
}

public class ThumbUpGesture : MyGesture {

    public float startAngle;
    public float maintainAngle;

    public ThumbUpGesture(float startAngle, float maintainAngle) {
        this.startAngle = startAngle;
        this.maintainAngle = maintainAngle;
    }

    public override GestureState isDetected(Hand hand) {
        List<Finger> fingers = hand.Fingers;

        if (!fingers[0].IsExtended) {
            return GestureState.STOP;
        }
        for (int i = 1; i < fingers.Count; i++) {
            if (fingers[i].IsExtended) {
                return GestureState.STOP;
            }
        }

        Vector3 thumbDirection = LeapUtil.LeapVectorToVector3(fingers[0].Direction);
        double angle = Vector3.Angle(thumbDirection, LeapUtil.LeapTransform.up);

        if (getState(hand) == GestureState.STOP && angle <= startAngle) {
            return GestureState.START;
        }

        if (getState(hand) != GestureState.STOP && angle <= maintainAngle) {
            return GestureState.ONGOING;
        }

        return GestureState.STOP;
    }
}

public class ThumbDownGesture : MyGesture {

    public float startAngle;
    public float maintainAngle;

    public ThumbDownGesture(float startAngle, float maintainAngle) {
        this.startAngle = startAngle;
        this.maintainAngle = maintainAngle;
    }

    public override GestureState isDetected(Hand hand) {
        List<Finger> fingers = hand.Fingers;

        if (!fingers[0].IsExtended) {
            return GestureState.STOP;
        }
        for (int i = 1; i < fingers.Count; i++) {
            if (fingers[i].IsExtended) {
                return GestureState.STOP;
            }
        }

        Vector3 thumbDirection = LeapUtil.LeapVectorToVector3(fingers[0].Direction);
        double angle = Vector3.Angle(thumbDirection, -LeapUtil.LeapTransform.up);

        if (getState(hand) == GestureState.STOP && angle <= startAngle) {
            return GestureState.START;
        }

        if (getState(hand) != GestureState.STOP && angle <= maintainAngle) {
            return GestureState.ONGOING;
        }

        return GestureState.STOP;
    }
}