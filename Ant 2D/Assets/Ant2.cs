using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant2 : MonoBehaviour {

    public const short none = 0, forward = 1, backward = -1,
                       clockwise = -1, counterclockwise = 1;
    short motion, direction;

    new Rigidbody2D rigidbody;
    new Collider2D collider;
    FixedJoint2D jaws;
    bool grabbing = false;
    
    public Animator bender;
    public Animator walker;
    public Animator antennae;
    public float walk_speed = 1f;
    public float rot_speed = 1f;
    public float target_distance;
    public float strength = 1f;

    public enum ReverseType { invert_rotation, invert_bend }
    public ReverseType reverse_type = ReverseType.invert_bend;

    enum Action { none, walk_straight, walk_left, walk_right }

    // AI variables
    bool controlled = false;
    float next_time = 0;
    float wait_time = 1f;
    ArrayList sensedObjects = new ArrayList();


    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        jaws = GetComponent<FixedJoint2D>();
        antennae.SetFloat("Offset", Random.value);
        next_time = Time.time + Random.value * wait_time;
    }

    void Update()
    {
        if(!controlled)
        {
            Think();
        }
        Act();
    }

    void Think()
    {
        Collider2D target = null;
        if(sensedObjects.Count > 0) target = (Collider2D)sensedObjects[0];
        if(target != null)
        {
            Vector2 displacement = target.transform.position - transform.position - transform.right;

            float distance = displacement.magnitude;

            if(distance > target_distance)
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

                float angle_tolerance = 5f;
                if (angle_displacement < -angle_tolerance)
                {
                    TurnClockwise();
                }
                else if (angle_displacement > angle_tolerance)
                {
                    TurnCounterClockwise();
                }
                else
                {
                    Straighten();
                }

                if (Mathf.Abs(angle_displacement) < 60f)
                {
                    WalkForward();
                }
                else
                {
                    WalkBackward();
                }
            }
            else
            {
                StopWalking();
            }
        }
        else
        {
            Straighten();
            StopWalking();
        }
    }

    void Act()
    {
        if (motion != none)
        {
            if (direction != none)
            {
                float invert = 1f;
                if(reverse_type == ReverseType.invert_rotation)
                {
                    invert = motion;
                }
                rigidbody.AddTorque(direction * rot_speed * walk_speed * invert * 40f);
            }
            rigidbody.AddForce(transform.right * motion * 3f * walk_speed * 50f);
            walker.SetFloat("Speed", rigidbody.velocity.magnitude / 3f);
        }
    }
    
    public void SubmitControl() { controlled = true; }
    public int GetMotion() { return motion; }
    public int GetDirection() { return direction; }
    public void ToggleReverseType()
    {
        if (reverse_type == ReverseType.invert_bend)
        {
            reverse_type = ReverseType.invert_rotation;
        }
        else
        {
            reverse_type = ReverseType.invert_bend;
        }
    }
    public Collider2D GetCollider() { return collider; }

    public void StopWalking()
    {
        motion = none;
        walker.SetBool("Walk", false);
    }

    public void WalkForward()
    {
        motion = forward;
        walker.SetBool("Walk", true);
        walker.SetFloat("Speed", walk_speed);
    }

    public void WalkBackward()
    {
        motion = backward;
        walker.SetBool("Walk", true);
        walker.SetFloat("Speed", -walk_speed);
    }

    public void InvertBend()
    {
        if (reverse_type == ReverseType.invert_bend && direction != none)
        {
            bender.SetInteger("Direction", -bender.GetInteger("Direction"));
        }
    }

    public void Straighten()
    {
        direction = none;
        bender.SetInteger("Direction", none);
    }

    public void TurnCounterClockwise()
    {
        bender.SetInteger("Direction", counterclockwise);
        direction = counterclockwise;
    }

    public void TurnClockwise()
    {
        bender.SetInteger("Direction", clockwise);
        direction = clockwise;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.isTrigger)
        {
            if (other.CompareTag("Ant"))
            {
                sensedObjects.Add(other);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        while(sensedObjects.Contains(other))
        {
            sensedObjects.Remove(other);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!grabbing)
        {
            return;
        }
        else if( collision.rigidbody.GetComponent<TriangleGroup>() )
        {
            float angle;
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                ContactPoint2D c = collision.contacts[i];
                angle = Mathf.Atan2(c.normal.y, c.normal.x) * Mathf.Rad2Deg - transform.eulerAngles.z + 180f;

                // Corrective Action
                while (angle < -180f)
                {
                    angle += 360f;
                }
                while (angle > 180f)
                {
                    angle -= 360f;
                }

                float angle_tolerance = 90f;
                if (Mathf.Abs(angle) < angle_tolerance / 2f)
                {
                    Grab(collision.rigidbody);
                    return;
                }
            }
        }
    }

    public void Grab(Rigidbody2D other)
    {
        if(other != null)
        {
            jaws.enabled = true;
            jaws.connectedBody = other;
            jaws.connectedBody.GetComponent<TriangleGroup>().Lift(strength);
        }
    }

    public void StartGrabbing()
    {
        grabbing = true;
    }

    public void Release()
    {
        if(jaws.connectedBody)
        {
            jaws.connectedBody.GetComponent<TriangleGroup>().Drop(strength);
            jaws.connectedBody = null;
        }
        jaws.enabled = false;
        grabbing = false;
    }

    public void Split()
    {
        if(jaws.enabled)
        {
            TriangleGroup group = jaws.connectedBody.GetComponent<TriangleGroup>();
            if(group)
            {
                jaws.connectedBody = group.Split(this);
            }
        }
    }
}
