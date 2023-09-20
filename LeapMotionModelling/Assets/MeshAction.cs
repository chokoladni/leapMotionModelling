using System;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public interface GestureAction {
    void perform(Hand hand, GestureState state);
}

public abstract class MeshAction : GestureAction {
    protected MeshWrapper3 mesh;
    
    public MeshAction(MeshWrapper3 mesh) {
        this.mesh = mesh;
    }

    public abstract void perform(Hand hand, GestureState state);
}

public class AddVertexAction : MeshAction {

    int vertexIndex = -1;

    public AddVertexAction(MeshWrapper3 mesh) : base(mesh) {}

    public override void perform(Hand hand, GestureState state) {
        if(state == GestureState.STOP || state == GestureState.INVALID) {
            vertexIndex = -1;
            return;
        }
        
        Vector3 position = LeapUtil.LeapVectorToScaledVector3(hand.Fingers[0].StabilizedTipPosition);

        if (state == GestureState.START) {
            vertexIndex = mesh.addVertex(position);
        } else if (state == GestureState.ONGOING) {
            mesh.moveVertex(vertexIndex, position);
        }
    }
}

public class SelectVertexAction : MeshAction {

    private float radius;
    int count = 0;

    public SelectVertexAction(MeshWrapper3 mesh, float radius) : base(mesh) {
        this.radius = radius;
    }

    public override void perform(Hand hand, GestureState state) {
        if (state == GestureState.STOP || state == GestureState.INVALID) {
            return;
        }

        Vector3 pos = LeapUtil.LeapVectorToScaledVector3(hand.Fingers[1].StabilizedTipPosition);
        if (hand.IsRight) {
            mesh.select(pos, radius);
        } else {
            mesh.deselect(pos, radius);
        }
    }
}

public class TranslateSelectionAction : MeshAction {

    private Vector3 lastPosition;

    public TranslateSelectionAction(MeshWrapper3 mesh) : base(mesh) {
    }

    public override void perform(Hand hand, GestureState state) {
        if (state == GestureState.STOP || state == GestureState.INVALID) {
            return;
        }

        if (state == GestureState.START) {
            lastPosition = LeapUtil.LeapVectorToScaledVector3(hand.PalmPosition);
        } else {
            Vector3 currPosition = LeapUtil.LeapVectorToScaledVector3(hand.PalmPosition);
            mesh.moveSelected(currPosition - lastPosition);
            lastPosition = currPosition;
        }
   }
}

public class InvertNormalsAction : MeshAction {

    public InvertNormalsAction(MeshWrapper3 mesh) : base(mesh) {
    }

    public override void perform(Hand hand, GestureState state) {
        if(state == GestureState.START) {
            mesh.switchSelectedNormals();
        }
    }
}

public class CreateTrianglesAction : MeshAction {

    public CreateTrianglesAction(MeshWrapper3 mesh) : base(mesh) {
    }

    public override void perform(Hand hand, GestureState state) {
        if (state == GestureState.START) {
            mesh.createTrianglesFromSelected();
        }
    }
}

public class DeleteSelectionAction : MeshAction {

    public DeleteSelectionAction(MeshWrapper3 mesh) : base(mesh) {
    }

    public override void perform(Hand hand, GestureState state) {
        if (state == GestureState.START) {
            mesh.deleteSelectedVertices();
        }
    }
}

public class DeleteSelectedTrianglesAction : MeshAction {

    public DeleteSelectedTrianglesAction(MeshWrapper3 mesh) : base(mesh) {
    }

    public override void perform(Hand hand, GestureState state) {
        if (state == GestureState.START) {
            mesh.deleteSelectedTriangles();
        }
    }
}

public class MoveCameraAction : GestureAction {

    private Vector3 lastPosition;

    public void perform(Hand hand, GestureState state) {
        if (state == GestureState.STOP || state == GestureState.INVALID) {
            return;
        }

        if (state == GestureState.START) {
            lastPosition = LeapUtil.LeapVectorToLocalScaledVector3(hand.PalmPosition);
            lastPosition = LeapUtil.LeapTransform.rotation * lastPosition;

        } else {
            Vector3 currPosition = LeapUtil.LeapVectorToLocalScaledVector3(hand.PalmPosition);
            currPosition = LeapUtil.LeapTransform.rotation * currPosition;
            CameraController.moveCameraGlobally(lastPosition - currPosition);
            lastPosition = currPosition;
        }
    }
}