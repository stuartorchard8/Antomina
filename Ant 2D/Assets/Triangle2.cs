using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle2 : MonoBehaviour {

    static Vector2 left_anchor = new Vector2(-0.2165064f, 0.125f);
    static Vector2 right_anchor = new Vector2(0.2165064f, 0.125f);
    static Vector2 bottom_anchor = new Vector2(0, -0.25f);
    new Rigidbody2D rigidbody;
    new PolygonCollider2D collider;
    PolygonCollider2D recent_collision = null;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(recent_collision != collision.collider)
        {
            if (collision.rigidbody.CompareTag("Triangle"))
            {
                Triangle2 other = collision.rigidbody.GetComponent<Triangle2>();
                Vector2 displacement = collision.rigidbody.transform.position - transform.position;
                float distance = displacement.magnitude;

                if (distance < 0.64f)
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

                    FixedJoint2D j = gameObject.AddComponent<FixedJoint2D>();
                    j.connectedBody = other.rigidbody;
                    // Check which side to attach on this triangle
                    if (Mathf.Abs(angle_displacement) > 120f)
                    {
                        // Attach to bottom of this triangle
                        j.anchor = bottom_anchor;
                    }
                    else if (angle_displacement > 0f)
                    {
                        // Attach to right of this triangle
                        j.anchor = right_anchor;
                    }
                    else
                    {
                        // Attach to left of this triangle
                        j.anchor = left_anchor;
                    }

                    float angle_displacement2 = angle - other.transform.eulerAngles.z;
                    // Corrective Action
                    while (angle_displacement2 < -180f)
                    {
                        angle_displacement2 += 360f;
                    }
                    while (angle_displacement2 > 180f)
                    {
                        angle_displacement2 -= 360f;
                    }
                    // Check which side to attach on the other triangle
                    if (Mathf.Abs(angle_displacement2) < 60f)
                    {
                        // Attach to bottom of the other triangle
                        j.connectedAnchor = bottom_anchor;
                    }
                    else if (angle_displacement2 < 0f)
                    {
                        // Attach to right of the other triangle
                        j.connectedAnchor = right_anchor;
                    }
                    else
                    {
                        // Attach to left of the other triangle
                        j.connectedAnchor = left_anchor;
                    }
                }
            }
        }
        else
        {
            recent_collision = null;
        }
    }

}
