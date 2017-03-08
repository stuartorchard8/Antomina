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

    void ConnectWith(Triangle2 t)
    {
        Vector2 displacement;
        Vector2 anchor = left.anchor;
        anchor.Scale(transform.localScale);
        displacement = (Vector2)transform.position - anchor;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.rigidbody.CompareTag("Triangle"))
        {
            Vector2 displacement = collision.rigidbody.transform.position - transform.position;
            float distance = displacement.magnitude;

            if(distance < 0.64f)
            {
                float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
                float angle_displacement = angle - transform.eulerAngles.z;
                // Corrective Action
                while (angle_displacement < -180f)
                {
                    angle_displacement += 360f;
                }
                while (angle_displacement > 180f)
                {
                    angle_displacement -= 360f;
                }

                if (Mathf.Abs(angle_displacement)>120f)
                {
                    // Attach to bottom of this triangle
                }
                else if(angle_displacement > 0f)
                {
                    // Attach to right of this triangle
                }
                else
                {
                    // Attach to left of this triangle
                }
            }
        }
    }

}
