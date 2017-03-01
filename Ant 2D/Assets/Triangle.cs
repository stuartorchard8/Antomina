using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public static float root3 = 1.73205f;

    TriangleGroup group;            // The group which this triangle belongs to.
    bool up = true;                 // Triangle is pointing upward.

    ArrayList closeAnts = new ArrayList();  // Ants which are touching this triangle.

    void OnTriggerEnter2D( Collider2D collider )
    {
        if(collider.CompareTag("Ant"))
        {
            // Store the collider somewhere.
            closeAnts.Add(collider);
            // If an ant requests a split from the group,
            // and it has this collider,
            // then the group knows to split from this triangle.
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Ant"))
        {
            while(closeAnts.Contains(collider))
            {
                // The ant is no longer touching this triangle
                closeAnts.Remove(collider);
            }
        }
    }

    // Used to determine what part of a TileGroup is touching a particular ant.
    public bool Touching(Ant2 ant)
    {
        for (int i = 0; i < closeAnts.Count; i++)
        {
            if ((Collider2D)closeAnts[i] == ant.GetCollider())
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPointingUpward() { return up; }

    public void JoinGroup(TriangleGroup _group, int x, int y)
    {
        if(group)
        {
            group.Remove(this);
        }
        group = _group;
        group.Add(this, x, y);
    }

    public void SetUp(bool _up)
    {
        if(up != _up)
        {
            transform.Translate(new Vector3(0, Triangle.root3 / 6f, 0));
            transform.Rotate(new Vector3(0, 0, 1), 180);
            up = _up;
        }
    }
}
