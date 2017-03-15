using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2 : MonoBehaviour {

    public Triangle2 trianglePrefab;

    public short size;

	// Use this for initialization
	void Start () {
        Triangle2 t;
        bool up;

        for (int y = 0; y < size; y++)
        {
            up = (y % 2 == 1);
            for (int x = 0; x != size; x ++)
            {
                up = !up;
                t = Instantiate(trianglePrefab);
                t.transform.Translate(new Vector3(x / 2f, y * Triangle.root3 / 2f));
                if (!up)
                {
                    t.transform.Translate(new Vector3(0, Triangle.root3 / 6f, 0));
                    t.transform.Rotate(new Vector3(0, 0, 1), 180);
                }
                t.transform.SetParent(transform);
            }
        }
    }
}
