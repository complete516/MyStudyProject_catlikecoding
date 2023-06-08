using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedCube : MonoBehaviour
{

    [SerializeField]
    private int xSize, ySize, zSize;

    // Start is called before the first frame update

    Mesh mesh;
    Vector3[] vertices;

    void Start()
    {
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Cube";
        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = 2 * (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1));

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        int v = 0;
        for (int x = 0; x <= xSize; x++)
        {
            vertices[v++] = new Vector3(x, 0, 0);
            yield return wait;
        }

		// for (int z = 1; z <= zSize; z++) {
		// 	vertices[v++] = new Vector3(xSize, 0, z);
		// 	yield return wait;
		// }
		// for (int x = xSize - 1; x >= 0; x--) {
		// 	vertices[v++] = new Vector3(x, 0, zSize);
		// 	yield return wait;
		// }
		// for (int z = zSize - 1; z > 0; z--) {
		// 	vertices[v++] = new Vector3(0, 0, z);
		// 	yield return wait;
		// }

        yield return wait;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

}
