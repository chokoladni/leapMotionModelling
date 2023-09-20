using UnityEngine;
using System.Collections.Generic;

public interface MeshObserver {

    void meshChanged();
}

public class VertexVisualizer : MeshObserver {

    private List<GameObject> objects = new List<GameObject>();
    private GameObject representation;
    private Material unselectedMat;
    private Material selectedMat;
    private MeshWrapper3 mesh;

    public VertexVisualizer(MeshWrapper3 subject, GameObject representation, Material unselectedMat, Material selectedMat) {
        this.representation = representation;
        this.unselectedMat = unselectedMat;
        this.selectedMat = selectedMat;
        subject.addMeshObserver(this);
        this.mesh = subject;
    }
    
    public void show(bool show) {
        foreach (GameObject obj in objects) {
            obj.SetActive(show);
        }
    }

    public void meshChanged() {
        List<Vector3> vertices = mesh.vertices();
        List<int> selected = new List<int>(mesh.selected());

        while(objects.Count < vertices.Count) {
            objects.Add(GameObject.Instantiate(representation, Vector3.zero, Quaternion.identity));
        }
        
        while(objects.Count > vertices.Count) {
            objects[objects.Count - 1].SetActive(false);
            objects.RemoveAt(objects.Count - 1);
        }

        for (int i = 0; i < vertices.Count; i++) {
            objects[i].transform.position = vertices[i];
            if(selected.Contains(i)) {
                objects[i].GetComponent<MeshRenderer>().material = selectedMat;
            } else {
                objects[i].GetComponent<MeshRenderer>().material = unselectedMat;
            }
        }
    }
}