using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle2 : MonoBehaviour {
    public float x,y,z,w;
    public float stickyness = 100f;
    static Vector2 left_anchor = new Vector2(-0.2165064f, 0.125f);
    static Vector2 right_anchor = new Vector2(0.2165064f, 0.125f);
    static Vector2 bottom_anchor = new Vector2(0, -0.25f);
    static Vector2 bottom_offset = new Vector2(0, -0.57735027f);
    static Vector2 right_offset = new Vector2(0.5f, 0.2886752f);
    static Vector2 left_offset = new Vector2(-0.5f, 0.2886752f);
    new Rigidbody2D rigidbody;
    new PolygonCollider2D collider;
    PolygonCollider2D recent_collision = null;

	// Use this for initialization
	void Awake () {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        Quaternion r = transform.rotation;
        x = r.x;
        y = r.y;
        z = r.z;
        w = r.w;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(recent_collision != collision.collider)
        {
            if (collision.rigidbody.CompareTag("Triangle"))
            {
                Triangle2 other = collision.rigidbody.GetComponent<Triangle2>();
                other.recent_collision = collider;
                Vector2 displacement = collision.rigidbody.transform.position - transform.position;
                float distance = displacement.magnitude;

                if (distance < 0.7f)
                {
                    float r_angle_a = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg + 90f;
                    float r_angle_b = r_angle_a - other.transform.rotation.z;
                    r_angle_a -= transform.rotation.z;

                    // Keep both angles within the bounds of -180 to +180
                    while (r_angle_a < -180f) {
                        r_angle_a += 360f;
                    }
                    while (r_angle_a > 180f) {
                        r_angle_a -= 360f;
                    }
                    while (r_angle_b < -180f) {
                        r_angle_b += 360f;
                    }
                    while (r_angle_b > 180f) {
                        r_angle_b -= 360f;
                    }
                    
                    RelativeJoint2D j = gameObject.AddComponent<RelativeJoint2D>();
                    j.correctionScale = 1f;
                    j.breakForce = stickyness;
                    j.breakTorque = stickyness;
                    j.connectedBody = other.rigidbody;
                    j.autoConfigureOffset = false;

                    // Check which side to attach on this triangle
                    if (Mathf.Abs(r_angle_a) <= 60f)
                    {
                        // Attach to bottom of this triangle
                        j.linearOffset = bottom_offset;
                        j.angularOffset = 0f;
                    }
                    else if (r_angle_a > 0f)
                    {
                        // Attach to right side of this triangle
                        j.linearOffset = right_offset;
                        j.angularOffset = 120f;
                    }
                    else
                    {
                        // Attach to left side of this triangle
                        j.linearOffset = left_offset;
                        j.angularOffset = -120f;
                    }

                    // Check which side to attach on the other triangle
                    if (Mathf.Abs(r_angle_b) >= 120f)
                    {
                        // Attach to bottom of the other triangle
                        j.angularOffset -= 180f;
                    }
                    else if (r_angle_b > 0f)
                    {
                        // Attach to right of the other triangle
                        j.angularOffset -= 60f;
                    }
                    else
                    {
                        // Attach to left of the other triangle
                        j.angularOffset += 60;
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
