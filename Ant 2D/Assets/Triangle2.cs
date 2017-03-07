using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle2 : MonoBehaviour {

    public FixedJoint2D left, right, down;
    new Rigidbody2D rigidbody;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void SetLeft(Triangle2 t, bool active)
    {
        left.enabled = active;
        left.connectedBody = t.rigidbody;
    }

    void SetRight(Triangle2 t, bool active)
    {
        left.enabled = active;
        left.connectedBody = t.rigidbody;
    }

    void SetBottom(Triangle2 t, bool active)
    {
        left.enabled = active;
        left.connectedBody = t.rigidbody;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.rigidbody.CompareTag("Triangle"))
        {
            Vector2 displacement = collision.rigidbody.transform.position - transform.position;
            float distance = displacement.magnitude;

            if(distance < 0.64f)
            {
                float angle = Mathf.Atan2(displacement.x, displacement.y);
            }
        }
    }
}
