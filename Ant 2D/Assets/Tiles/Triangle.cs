using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public static float root3 = 1.73205f;

    public TriangleGroup triangleGroupPrefab;
    TriangleGroup group;            // The group which this triangle belongs to.
    bool up = true;                 // Triangle is pointing upward.

    public bool IsPointingUpward() { return up; }

    public TriangleGroup GetGroup() { return group; }
    public void JoinGroup(TriangleGroup _group, int x, int y)
    {
        if(group)
        {
            group.Remove(this);
        }
        group = _group;
        group.Add(this, x, y);
        transform.SetParent(group.transform);
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

    /* 
     * Splits this Triangle from its previous group.
     */
    public Rigidbody2D Split(Ant2 splitter)
    {
        TriangleGroup new_group = Instantiate(triangleGroupPrefab);
        new_group.transform.position = transform.position;
        new_group.transform.rotation = transform.rotation;
        JoinGroup(new_group, 0, 0);
        return new_group.GetRigidbody();
    }

    public void Lift(float strength)
    {
        group.Lift(strength);
    }

    public void Drop(float strength)
    {
        group.Drop(strength);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Ant"))
        {
            other.GetComponent<Ant2>().OnTriangleEnter2D(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Ant"))
        {
            other.GetComponent<Ant2>().OnTriangleExit2D(this);
        }
    }

    public Rigidbody2D GetRigidbody() { return group.GetRigidbody(); }

    public bool IsInGroup(TriangleGroup g)
    {
        return g == group;
    }
}
