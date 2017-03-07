using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Triangle trianglePrefab;
    public TriangleGroup triangleGroupPrefab;
    TriangleGroup group;

    public short size;

	// Use this for initialization
	void Start () {
        group = Instantiate(triangleGroupPrefab);
        Triangle t;
        
        for (int y = 0; y < size; y++)
        {
            int start = 0, end = size + 1, increment = 1;
            if (y % 2 == 1)
            {
                start = size;
                end = -1;
                increment = -1;
            }
            for (int x = start; x != end; x += increment)
            {
                t = Instantiate(trianglePrefab);
                t.transform.SetParent(group.transform);
                t.transform.Translate(new Vector3(x / 2f, y * Triangle.root3 / 2f));

                t.JoinGroup(group, x, y);
            }
        }
        Destroy(this.gameObject);
    }
}
