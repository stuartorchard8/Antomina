using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Triangle trianglePrefab;
    TriangleGroup group;

    public short size;

	// Use this for initialization
	void Start () {
        group = GetComponent<TriangleGroup>();

        Triangle[,] triangles = new Triangle[size*2,size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size * 2; x++)
            {
                triangles[x,y] = Instantiate(trianglePrefab);
            }
        }
        for (int y = 0; y < size; y++)
        {
            int start = 0, end = size * 2, increment = 1;
            if(y % 2 == 1)
            {
                start = size * 2 - 1;
                end = -1;
                increment = -1;
            }
            for (int x = start; x != end; x += increment)
            {
                bool up = (x % 2) == 0;     // Whether the Triangle will point up or down.
                if (y % 2 == 1)
                {
                    up = !up;
                }

                triangles[x, y].transform.SetParent(this.transform);
                triangles[x, y].transform.Translate(new Vector3(x / 2f, y * Triangle.root3 / 2));

                if (!up)
                {
                    triangles[x, y].transform.Translate(new Vector3(0, Triangle.root3 / 6f, 0));
                    triangles[x, y].transform.Rotate(new Vector3(0, 0, 1), 180);
                }
                triangles[x, y].JoinGroup(group, up);
                group.Add(triangles[x, y], x, y);
            }
        }
    }
}
