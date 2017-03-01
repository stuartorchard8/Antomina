using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour {

    const short stationary = 0,
                clockwise = -1, counterclockwise = 1,
                forward = 1, backward = -1;
    short motion, direction;
    new Rigidbody2D rigidbody2D;

    public bool player = false;
    public Animator bender;
    public Animator walker;
    public Animator antennae;
    public float walk_speed = 1f;
    public float rot_speed = 1f;

    bool w_down = false, w_up = false, s_down = false, s_up = false,
         a_down = false, a_up = false, d_down = false, d_up = false;

    // AI variables
    float next_time = 0;
    float wait_time = 1f;

    
	void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        antennae.SetFloat("Offset", Random.value);
        next_time = Time.time + Random.value * wait_time;
    }
	
	void Update () {
        GetInput();
        Bend();
        Walk();
    }

    void GetInput()
    {
        if(player)
        {
            a_up = Input.GetKeyUp(KeyCode.A);
            a_down = Input.GetKeyDown(KeyCode.A);

            d_up = Input.GetKeyUp(KeyCode.D);
            d_down = Input.GetKeyDown(KeyCode.D);

            w_up = Input.GetKeyUp(KeyCode.W);
            w_down = Input.GetKeyDown(KeyCode.W);

            s_up = Input.GetKeyUp(KeyCode.S);
            s_down = Input.GetKeyDown(KeyCode.S);
        }
        else if( next_time <= Time.time )
        {
            next_time = Time.time + wait_time;

            a_up = Random.value > 0.5f;
            if (!a_up)
            {
                a_down = Random.value > 0.5f;
            }

            d_up = Random.value > 0.5f;
            if (!d_up)
            {
                d_down = Random.value > 0.5f;
            }

            w_up = Random.value > 0.5f;
            if (!w_up)
            {
                w_down = Random.value > 0.5f;
            }

            //s_up = Random.value > 0.5f;
            //if (!s_up)
            //{
            //    s_down = Random.value > 0.5f;
            //}
        }
    }

    void Bend()
    {
        if (a_down)
        {
            a_down = false;
            if (motion == backward)
            {
                bender.SetBool("Right", true);
                bender.SetBool("Left", false);
            }
            else
            {
                bender.SetBool("Left", true);
                bender.SetBool("Right", false);
            }
            direction = counterclockwise;
        }
        if (a_up)
        {
            a_up = false;
            if (direction == counterclockwise)
            {
                if (motion == backward)
                {
                    direction = stationary;
                    bender.SetBool("Right", false);
                }
                else
                {
                    direction = stationary;
                    bender.SetBool("Left", false);
                }
            }
        }

        if (d_down)
        {
            d_down = false;
            if (motion == backward)
            {
                bender.SetBool("Left", true);
                bender.SetBool("Right", false);
            }
            else
            {
                bender.SetBool("Right", true);
                bender.SetBool("Left", false);
            }
            direction = clockwise;
        }
        if (d_up)
        {
            d_up = false;
            if (direction == clockwise)
            {
                if (motion == backward)
                {
                    direction = stationary;
                    if (motion == -1)
                    {
                        bender.SetBool("Right", false);
                    }
                    else
                    {
                        bender.SetBool("Left", false);
                    }
                }
                else
                {
                    direction = stationary;
                    bender.SetBool("Right", false);
                }
            }
        }

        if (direction != stationary && motion != stationary)
        {
            transform.Rotate(0, 0, direction*rot_speed*walk_speed);
        }
    }

    void Walk()
    {
        if (w_down)
        {
            w_down = false;
            if (motion == backward)
            {
                if(direction == counterclockwise)
                {
                    bender.SetBool("Left", true);
                    bender.SetBool("Right", false);
                }
                else if(direction == clockwise)
                {
                    bender.SetBool("Right", true);
                    bender.SetBool("Left", false);
                }
            }
            motion = forward;
            walker.SetBool("Walk", true);
            walker.SetFloat("Speed", walk_speed);
        }
        if (w_up)
        {
            w_up = false;
            if (motion == forward)
            {
                motion = stationary;
                walker.SetBool("Walk", false);
            }
        }

        if (s_down)
        {
            s_down = false;
            if (direction == counterclockwise)
            {
                bender.SetBool("Right", true);
                bender.SetBool("Left", false);
            }
            else if (direction == clockwise)
            {
                bender.SetBool("Left", true);
                bender.SetBool("Right", false);
            }
            motion = backward;
            walker.SetBool("Walk", true);
            walker.SetFloat("Speed", -walk_speed);
        }
        if (s_up)
        {
            s_up = false;
            if (motion == backward)
            {
                motion = stationary;
                walker.SetBool("Walk", false);
                if (direction == clockwise)
                {
                    bender.SetBool("Right", true);
                    bender.SetBool("Left", false);
                }
                else if (direction == counterclockwise)
                {
                    bender.SetBool("Left", true);
                    bender.SetBool("Right", false);
                }
            }
        }

        if (motion != stationary)
        {
            transform.Translate(Vector2.right * motion * 3 * walk_speed / 60);
        }
    }
}
