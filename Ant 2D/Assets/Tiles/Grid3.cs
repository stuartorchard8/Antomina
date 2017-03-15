using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3 : MonoBehaviour {

    public Hexagon hexagonPrefab;

    public short size;

	// Use this for initialization
	void Start () {
        Hexagon t;
        bool up;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x != size; x ++)
            {
                t = Instantiate(hexagonPrefab);
                t.transform.Translate(new Vector3(x + (y%2)/2f, y * Triangle.root3/2));
                t.transform.SetParent(transform);
            }
        }
    }
}
