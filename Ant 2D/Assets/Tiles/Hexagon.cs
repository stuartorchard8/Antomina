using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    public float stickyness = 100f;
    new Rigidbody2D rigidbody;
    new PolygonCollider2D collider;
    PolygonCollider2D recent_collision = null;

	// Use this for initialization
	void Awake () {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(recent_collision != collision.collider)
        {
            if (collision.rigidbody.CompareTag("Hexagon"))
            {
                Hexagon other = collision.rigidbody.GetComponent<Hexagon>();
                other.recent_collision = collider;
                Vector2 displacement = collision.rigidbody.transform.position - transform.position;
                float distance = displacement.magnitude;

                if (distance < 1f)
                {
                    float relative_angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg - transform.eulerAngles.z + 90f;
                    // Corrective Action
                    while (relative_angle < -180f)
                    {
                        relative_angle += 360f;
                    }
                    while (relative_angle > 180f)
                    {
                        relative_angle -= 360f;
                    }

                    FixedJoint2D j = gameObject.AddComponent<FixedJoint2D>();
                    //RelativeJoint2D j = gameObject.AddComponent<RelativeJoint2D>();
                    j.connectedBody = other.rigidbody;
                    //j.correctionScale = 1f;
                    j.breakForce = stickyness;
                    j.breakTorque = stickyness;
                    // Check which side to attach on this triangle
                    //if (Mathf.Abs(relative_angle) < 60f)
                    //{
                    //    // Attach to bottom of this triangle
                    //    j.linearOffset = new Vector2(0, -0.57735027f);
                    //    j.angularOffset = 60f;
                    //}
                    //else if (relative_angle > 0f)
                    //{
                    //    // Attach to right side of this triangle
                    //    j.linearOffset = new Vector2(0.5f, 0.2886752f);
                    //    j.angularOffset = -180f;
                    //}
                    //else
                    //{
                    //    // Attach to left side of this triangle
                    //    j.linearOffset = new Vector2(-0.5f, 0.2886752f);
                    //    j.angularOffset = -60f;
                    //}

                    //float angle_displacement2 = angle + other.transform.eulerAngles.z + 90f;
                    //// Corrective Action
                    //while (angle_displacement2 < -180f)
                    //{
                    //    angle_displacement2 += 360f;
                    //}
                    //while (angle_displacement2 > 180f)
                    //{
                    //    angle_displacement2 -= 360f;
                    //}
                    // Check which side to attach on the other triangle
                    //if (Mathf.Abs(angle_displacement2) < 60f)
                    //{
                    //    // Attach to bottom of the other triangle
                    //    //j.connectedAnchor = bottom_anchor;
                    //}
                    //else if (angle_displacement2 > 0f)
                    //{
                    //    // Attach to right of the other triangle
                    //    //j.angularOffset -= 120f;
                    //}
                    //else
                    //{
                    //    // Attach to left of the other triangle
                    //    //j.angularOffset += 120f;
                    //}
                }
            }
        }
        else
        {
            recent_collision = null;
        }
    }

}
