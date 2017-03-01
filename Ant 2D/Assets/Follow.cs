using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public GameObject target;
    public float tracking_speed = 1f;

    float z;
	
    void Start()
    {
        z = transform.position.z;
    }

	void Update () {
        Vector3 newPos = (transform.position + target.transform.position * tracking_speed) / (tracking_speed + 1f);
        newPos.z = z;
        transform.position = newPos;
	}
}
