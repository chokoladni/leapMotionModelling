using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public interface Vector3Operation {
    Vector3 perform(Vector3 subject);
}

public delegate Vector3 Vector3Operation2(Vector3 subject);

public abstract class MeshObservable {

    private List<MeshObserver> observers = new List<MeshObserver>();

    public void addMeshObserver(MeshObserver o) {
        if (!observers.Contains(o)) {
            observers.Add(o);
        }
    }

    public void removeMeshObserver(MeshObserver o) {
        observers.Remove(o);
    }

    protected void notify() {
        foreach (MeshObserver o in new List<MeshObserver>(observers)) {
            o.meshChanged();
        }
    }
}

public class MeshWrapper3 : MeshObservable {
    private Mesh mesh;
    int cnt = 0;
    private List<int> selectedVerts = new List<int>();
    private List<Vector3> vertPositions = new List<Vector3>();
    private List<Vector3> selectedPositions = new List<Vector3>();

    public string meshToObj() {
        StringBuilder sb = new StringBuilder();
        
        foreach(Vector3 vertex in mesh.vertices) {
            sb.Append("v " + vertex.x + " " + vertex.y + " " + vertex.z + "\n");
        }

        int[] frontTriangles = mesh.GetTriangles(0);
        for (int i = 0; i < frontTriangles.Length; i+= 3) {
            sb.Append("f " + (frontTriangles[i] + 1) + " " + (frontTriangles[i + 1] + 1) + " " + (frontTriangles[i + 2] + 1) + "\n");
        }

        return sb.ToString();
    }

    public string meshToObj2() {
        StringBuilder sb = new StringBuilder();

        foreach (Vector3 vertex in vertPositions) {
            sb.Append("v " + vertex.x + " " + vertex.y + " " + vertex.z + "\n");
        }

        Vector3[] vertices = mesh.vertices;
        int[] frontTriangles = mesh.GetTriangles(0);
        for (int i = 0; i < frontTriangles.Length; i += 3) {
            int first = vertPositions.IndexOf(vertices[frontTriangles[i]]) + 1;
            int second = vertPositions.IndexOf(vertices[frontTriangles[i + 1]]) + 1;
            int third = vertPositions.IndexOf(vertices[frontTriangles[i + 2]]) + 1;
            sb.Append("f " + first + " " + second + " " + third + "\n");
        }

        return sb.ToString();
    }

    public void loadFromObj(string obj) {
        string[] lines = obj.Split('\n');

        List<Vector3> verts = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<int> backTriangles = new List<int>();

        this.clearData();
        mesh.subMeshCount = 2;
        foreach (string line in lines) {
            if (line.StartsWith("v ")) {
                string[] values = line.Substring(2).Split(' ');
                this.addVertex(new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));
            }
            if (line.StartsWith("f ")) {
                string[] values = line.Substring(2).Split(' ');
                for (int i = 0; i < 3; i++) {
                    selectedVerts.Add(int.Parse(values[i]) - 1);
                }
                createTrianglesFromSelected();
                selectedVerts.Clear();
            }
        }
    }

    public void clearData() {
        mesh.Clear();
        selectedVerts.Clear();
        selectedPositions.Clear();
        vertPositions.Clear();
    }

    public MeshWrapper3(Mesh mesh) {
        this.mesh = mesh;
    }

    public bool select(Vector3 vertex, float radius) {
        return selection(vertex, radius, true);
    }

    public bool deselect(Vector3 vertex, float radius) {
        return selection(vertex, radius, false);
    }

    private bool selection(Vector3 vertex, float radius, bool select) {
        if (vertPositions.Count == 0) {
            return false;
        }

        float min = Vector3.Distance(vertPositions[0], vertex);
        int index = 0;
        for (int i = 1; i < vertPositions.Count; i++) {
            float dist = Vector3.Distance(vertPositions[i], vertex);
            if (dist < min) {
                index = i;
                min = dist;
            }
        }
        if (min <= radius) {
            if(select && !selectedVerts.Contains(index)) { 
                selectedVerts.Add(index);
                selectedPositions.Add(vertPositions[index]);
            } else if(!select && selectedVerts.Contains(index)) {
                selectedVerts.Remove(index);
                selectedPositions.Remove(vertPositions[index]);
            }
            notify();
            return true;
        }
        return false;
    }

    public int vertexCount() {
        return vertPositions.Count;
    }

    public List<Vector3> vertices() {
        return vertPositions;
    }

    public int[] selected() {
        return selectedVerts.ToArray();
    }

    public int addVertex(Vector3 vertex) {
        vertPositions.Add(vertex);
        notify();
        return vertPositions.Count - 1;
    }

    public void deleteSelectedVertices() {
        List<int> frontTriangles = new List<int>(mesh.GetTriangles(0));
        List<int> backTriangles = new List<int>(mesh.GetTriangles(1));
        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        
        for(int i = 0; i < frontTriangles.Count; i += 3) {
            if(selectedPositions.Contains(vertices[frontTriangles[i]]) ||
               selectedPositions.Contains(vertices[frontTriangles[i+1]]) ||
               selectedPositions.Contains(vertices[frontTriangles[i+2]])) {
                frontTriangles.RemoveAt(i+2);
                frontTriangles.RemoveAt(i+1);
                frontTriangles.RemoveAt(i);

                i -= 3;
            }
        }

        for (int i = 0; i < backTriangles.Count; i += 3) {
            if (selectedPositions.Contains(vertices[backTriangles[i]]) ||
               selectedPositions.Contains(vertices[backTriangles[i + 1]]) ||
               selectedPositions.Contains(vertices[backTriangles[i + 2]])) {
                backTriangles.RemoveAt(i + 2);
                backTriangles.RemoveAt(i + 1);
                backTriangles.RemoveAt(i);

                i -= 3;
            }
        }
        
        //for(int i = 0; i < vertices.Count; i++) {
        //    if(selectedPositions.Contains(vertices[i])) {
        //        vertices.RemoveAt(i);
        //        i--;
        //    }
        //}

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(frontTriangles.ToArray(), 0);
        mesh.SetTriangles(backTriangles.ToArray(), 1);
        vertPositions.RemoveAll(x => selectedPositions.Contains(x));
        selectedPositions.Clear();
        selectedVerts.Clear();
        notify();
        mesh.RecalculateNormals();
    }

    public void deleteSelectedTriangles() {
        List<int> frontTriangles = new List<int>(mesh.GetTriangles(0));
        List<int> backTriangles = new List<int>(mesh.GetTriangles(1));
        List<Vector3> vertices = new List<Vector3>(mesh.vertices);

        for (int i = 0; i < frontTriangles.Count; i += 3) {
            if (selectedPositions.Contains(vertices[frontTriangles[i]]) &&
               selectedPositions.Contains(vertices[frontTriangles[i + 1]]) &&
               selectedPositions.Contains(vertices[frontTriangles[i + 2]])) {
                frontTriangles.RemoveAt(i + 2);
                frontTriangles.RemoveAt(i + 1);
                frontTriangles.RemoveAt(i);

                i -= 3;
            }
        }

        for (int i = 0; i < backTriangles.Count; i += 3) {
            if (selectedPositions.Contains(vertices[backTriangles[i]]) &&
               selectedPositions.Contains(vertices[backTriangles[i + 1]]) &&
               selectedPositions.Contains(vertices[backTriangles[i + 2]])) {
                backTriangles.RemoveAt(i + 2);
                backTriangles.RemoveAt(i + 1);
                backTriangles.RemoveAt(i);

                i -= 3;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(frontTriangles.ToArray(), 0);
        mesh.SetTriangles(backTriangles.ToArray(), 1);
        selectedPositions.Clear();
        selectedVerts.Clear();
        notify();
        mesh.RecalculateNormals();
    }

    public void moveVertex(int index, Vector3 newPosition) {
        Vector3 pos = vertPositions[index];

        List<Vector3> meshVerts = new List<Vector3>();
        Vector3[] vertices = mesh.vertices;

        for(int i = 0; i < vertices.Length; i++) {
            if(pos.Equals(vertices[i])) {
                vertices[i] = newPosition;
            }
        }

        vertPositions[index] = newPosition;

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        notify();
    }

    public void moveSelected1(Vector3 offset) {
        Vector3[] vertices = mesh.vertices;
        for(int i = 0; i < selectedPositions.Count; i++) {
            Vector3 moved = new Vector3(selectedPositions[i].x + offset.x, selectedPositions[i].y + offset.y, selectedPositions[i].z + offset.z);
            for(int j = 0; j < vertices.Length; j++) {
                if(vertices[j].Equals(selectedPositions[i])) {
                    vertices[j] = moved;
                }
            }
            for(int j = 0; j < vertPositions.Count; j++) {
                if(vertPositions[j].Equals(selectedPositions[i])) {
                    vertPositions[j] = moved;
                }
            }

            selectedPositions[i] = moved;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        notify();
    }

    public void moveSelected(Vector3 offset) {
        performOperationOnSelected(v => new Vector3(v.x + offset.x, v.y + offset.y, v.z + offset.z));
    }

    public void rotateSelected(Vector3 pivot, Vector3 angles) {
        performOperationOnSelected(x => Quaternion.Euler(angles) * (x - pivot) + pivot);
    }

    private void performOperationOnSelected(Vector3Operation2 operation) {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < selectedPositions.Count; i++) {
            Vector3 transformed = operation(selectedPositions[i]);
            for (int j = 0; j < vertices.Length; j++) {
                if (vertices[j].Equals(selectedPositions[i])) {
                    vertices[j] = transformed;
                }
            }
            for (int j = 0; j < vertPositions.Count; j++) {
                if (vertPositions[j].Equals(selectedPositions[i])) {
                    vertPositions[j] = transformed;
                }
            }

            selectedPositions[i] = transformed;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        notify();
    }

    public void switchSelectedNormals() {
        int[] front = mesh.GetTriangles(0);
        int[] back = mesh.GetTriangles(1);
        Vector3[] vertices = mesh.vertices;

        for(int i = 0; i < front.Length; i+=3) {
            if (!selectedPositions.Contains(vertices[front[i]]) ||
                !selectedPositions.Contains(vertices[front[i + 1]]) ||
                !selectedPositions.Contains(vertices[front[i + 2]])) {
                continue;
            }

            int temp = front[i + 1];
            front[i + 1] = front[i + 2];
            front[i + 2] = temp;
        }

        for(int i = 0; i < back.Length; i+=3) {
            if (!selectedPositions.Contains(vertices[back[i]]) ||
                !selectedPositions.Contains(vertices[back[i + 1]]) ||
                !selectedPositions.Contains(vertices[back[i + 2]])) {
                continue;
            }

            int temp = back[i+1];
            back[i + 1] = back[i + 2];
            back[i + 2] = temp;
        }

        mesh.SetTriangles(front, 0);
        mesh.SetTriangles(back, 1);
        mesh.RecalculateNormals();
    }

    public void createTrianglesFromSelected() {
        if (selectedVerts.Count < 3) {
            return;
        }

        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        List<int> frontTriangles = new List<int>(mesh.GetTriangles(0));
        List<int> backTriangles = new List<int>(mesh.GetTriangles(1));
        
        for (int i = 0; i < selectedVerts.Count - 2; i++) {
            int offset = vertices.Count;

            vertices.Add(vertPositions[selectedVerts[i]]);
            vertices.Add(vertPositions[selectedVerts[i + 1]]);
            vertices.Add(vertPositions[selectedVerts[i + 2]]);

            frontTriangles.Add(offset);
            if(i % 2 == 0) { 
                frontTriangles.Add(offset + 1);
                frontTriangles.Add(offset + 2);
            } else {
                frontTriangles.Add(offset + 2);
                frontTriangles.Add(offset + 1);
            }
            vertices.Add(vertPositions[selectedVerts[i]]);
            vertices.Add(vertPositions[selectedVerts[i + 2]]);
            vertices.Add(vertPositions[selectedVerts[i + 1]]);

            backTriangles.Add(offset + 3);

            if(i % 2 == 0) {
                backTriangles.Add(offset + 4);
                backTriangles.Add(offset + 5);
            } else {
                backTriangles.Add(offset + 5);
                backTriangles.Add(offset + 4);
            }

            
        }

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(frontTriangles.ToArray(), 0);
        mesh.SetTriangles(backTriangles.ToArray(), 1);


        /*TODO: obrisi*/ selectedVerts.Clear();
        /*TODO: obrisi*/ selectedPositions.Clear();
        notify();

        mesh.RecalculateNormals();
    }


}
