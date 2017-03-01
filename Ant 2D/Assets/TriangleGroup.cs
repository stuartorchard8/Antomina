using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGroup : MonoBehaviour {
    
    Triangle[,] triangles = new Triangle[1,1];
    int x_length = 0, y_length = 0;
    int x_origin = 0, y_origin = 0;
    new Rigidbody2D rigidbody;
    new PolygonCollider2D collider;
    public static TriangleGroup triangleGroupPrefab;

    float default_drag, default_angular_drag;
    int n_triangles = 0;
    float amount_lifted = 0f;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        collider.points = new Vector2[0];
        triangles[0, 0] = null;
        default_drag = rigidbody.drag;
        rigidbody.drag = default_drag * n_triangles;
        default_angular_drag = rigidbody.angularDrag;
    }
	
    public bool Add(Triangle t, int x, int y)
    {
        bool up = (x % 2) == 0;     // Whether the Triangle will point up or down.
        if (y % 2 == 1)
        {
            up = !up;
        }
        t.SetUp(up);
        if (IsTouchingGroup(x, y, up))
        {
            n_triangles++;
            UpdateDrag();
            ResizeArray(x, y);
            triangles[x + x_origin, y + y_origin] = t;

            if (triangles.Length == 1)
            {
                AddFirstTriangleToCollider();
            }
            else
            {
                AddTriangleToCollider(x + x_origin, y + y_origin);
            }
            return true;
        }
        
        return false;
    }

    public void AddFirstTriangleToCollider()
    {
        Vector2[] triangle_points = new Vector2[3];
        float height = Triangle.root3 / 2f;
        float y_direction = -1;
        if (triangles[0, 0].IsPointingUpward())
        {
            y_direction = 1;
        }
        triangle_points[0] = new Vector2(0, height / 2f * y_direction + height / 6f);
        triangle_points[1] = new Vector2(0.5f, -height / 2f * y_direction + height / 6f);
        triangle_points[2] = new Vector2(-0.5f, -height / 2f * y_direction + height / 6f);

        collider.points = triangle_points;
    }

    public void AddTriangleToCollider(int x, int y)
    {
        Vector2[] new_points;

        short target_triangle_point = -1;
        int add_before = -1;
        int i,j;

        // Set up the points of the triangle which was added
        Vector2[] triangle_points = new Vector2[3];
        float height = Triangle.root3/2;
        triangle_points[0] = new Vector2(0, height / 2f);
        triangle_points[1] = new Vector2(0.5f, -height / 2f);
        triangle_points[2] = new Vector2(-0.5f, -height / 2f);
        for (i = 0; i < triangle_points.Length; i++)
        {
            if (!triangles[x, y].IsPointingUpward())
            {
                triangle_points[i].Scale(new Vector2(1f, -1f));
            }
            triangle_points[i] += new Vector2(x / 2f, y * height + height / 6f);
        }

        // Store where each of those points can be found on the collider
        int[] indecies = { -1, -1, -1 };
        for (i = 0; i < collider.points.Length; i++)
        {
            Vector2 a = collider.points[i];
            for (j = 0; j < triangle_points.Length; j++)
            {
                Vector2 b = triangle_points[j];

                if(MyVectorMath.Equals(a, b, 1))
                {
                    indecies[j] = i;
                }
            }
        }

        // Check what kind of connection is being made - adding or removing a point, and which point
        if( indecies[0] != -1)
        {
            if (indecies[1] != -1)
            {
                if (indecies[2] != -1)
                {
                    // Double-sided connection
                    // This means that one point will be removed from the collider
                    new_points = new Vector2[collider.points.Length - 1];
                    // Default to be a mid connection
                    target_triangle_point = 0;

                    // Check for right connection (x+0, x+1, x+2) or (x+2, x+1, x+0)
                    if (indecies[1] == indecies[0] - 1 || (indecies[1] == collider.points.Length - 1 && indecies[0] == 0)    // Pointing Downward
                     || indecies[1] == indecies[2] - 1 || (indecies[1] == collider.points.Length - 1 && indecies[2] == 0))   // Pointing Upward
                    {
                        // Remove right triangle point from the collider
                        target_triangle_point = 1;
                    }
                    // Check for left connection (x+1, x+2, x+0) or (x+0, x+2, x+1)
                    else if (indecies[2] == indecies[1] - 1 || (indecies[2] == collider.points.Length - 1 && indecies[1] == 0)    // Pointing Downward
                          || indecies[2] == indecies[0] - 1 || (indecies[2] == collider.points.Length - 1 && indecies[0] == 0))   // Pointing Upward
                    {
                        // Remove left triangle point from the collider
                        target_triangle_point = 2;
                    }

                    // Replace array with an array without the removed point
                    j = 0;
                    for (i = 0; i < collider.points.Length; i++)
                    {
                        if (i != indecies[target_triangle_point])
                        {
                            new_points[j] = collider.points[i];
                            j++;
                        }
                    }
                    collider.points = new_points;
                    // All done.
                    return;
                }
                else
                {
                    // Triangle points 0 and 1 are connected to the group
                    // Therefore we are adding Triangle point 2 to the group
                    target_triangle_point = 2;
                    add_before = Mathf.Max(indecies[0], indecies[1]);
                }
            }
            else // if (indecies[2] != -1) // This should definitely be true this case.
            {
                // Triangle points 0 and 2 are connected to the group
                // Therefore we are adding Triangle point 1 to the group
                target_triangle_point = 1;
                add_before = Mathf.Max(indecies[0], indecies[2]);

            }
        }
        else //if (indecies[1] != -1 && indecies[2] != -1) // This should definitely be true in this case.
        {
            // Triangle points 1 and 2 are connected to the group
            // Therefore we are adding Triangle point 0 to the group
            target_triangle_point = 0;
            add_before = Mathf.Max(indecies[1], indecies[2]);
        }

        // Make a normal connection
        new_points = new Vector2[collider.points.Length + 1];
        j = 0;
        for (i = 0; i < new_points.Length; i++)
        {
            if (i == add_before)
            {
                new_points[i] = triangle_points[target_triangle_point];
            }
            else
            {
                new_points[i] = collider.points[j];
                j++;
            }
        }
        collider.points = new_points;
    }

    public void RemoveTriangleFromCollider(int x, int y)
    {
        Vector2[] new_points;

        short target_triangle_point = -1;
        int add_before = -1;
        int i, j;

        // Set up the points of the triangle which was added
        Vector2[] triangle_points = new Vector2[3];
        float height = Triangle.root3 / 2;
        triangle_points[0] = new Vector2(0, height / 2f);
        triangle_points[1] = new Vector2(0.5f, -height / 2f);
        triangle_points[2] = new Vector2(-0.5f, -height / 2f);
        for (i = 0; i < triangle_points.Length; i++)
        {
            if (!triangles[x, y].IsPointingUpward())
            {
                triangle_points[i].Scale(new Vector2(1f, -1f));
            }
            triangle_points[i] += new Vector2(x / 2f, y * height + height / 6f);
        }

        // Store where each of those points can be found on the collider
        int[] indecies = { -1, -1, -1 };
        for (i = 0; i < collider.points.Length; i++)
        {
            Vector2 a = collider.points[i];
            for (j = 0; j < triangle_points.Length; j++)
            {
                Vector2 b = triangle_points[j];

                if (MyVectorMath.Equals(a, b, 1))
                {
                    indecies[j] = i;
                }
            }
        }

        // Check what kind of removal is happening - adding or removing a point, and which point
        if (indecies[0] != -1)
        {
            if (indecies[1] != -1)
            {
                if (indecies[2] != -1)
                {
                    // Single-sided connection
                    // This means that one point will be removed from the collider
                    new_points = new Vector2[collider.points.Length - 1];
                    // Default to be a mid connection
                    target_triangle_point = 0;

                    // Check for right connection (x+0, x+2, x+1) or (x+1, x+2, x+0)
                    if ((indecies[2] == indecies[0] - 1 || (indecies[2] == collider.points.Length - 1 && indecies[0] == 0))     // Pointing Downward
                    ||  (indecies[2] == indecies[1] - 1 || (indecies[2] == collider.points.Length - 1 && indecies[1] == 0)))    // Pointing Upward
                    {
                        // Remove left triangle point from the collider
                        target_triangle_point = 2;
                    }
                    // Check for left connection (x+2, x+1, x+0) or (x+0, x+1, x+2)
                    if ((indecies[1] == indecies[2] - 1 || (indecies[1] == collider.points.Length - 1 && indecies[2] == 0))     // Pointing Downward
                    ||  (indecies[1] == indecies[0] - 1 || (indecies[1] == collider.points.Length - 1 && indecies[0] == 0)))     // Pointing Upward
                    {
                        // Remove right triangle point from the collider
                        target_triangle_point = 1;
                    }

                    // Replace array with an array without the removed point
                    j = 0;
                    for (i = 0; i < collider.points.Length; i++)
                    {
                        if (i != indecies[target_triangle_point])
                        {
                            new_points[j] = collider.points[i];
                            j++;
                        }
                    }
                    collider.points = new_points;
                    // All done.
                    return;
                }
                else
                {
                    // Triangle points 0 and 1 are connected to the group
                    // Therefore we are adding Triangle point 2 to the group
                    target_triangle_point = 2;
                    add_before = Mathf.Max(indecies[0], indecies[1]);
                }
            }
            else // if (indecies[2] != -1) // This should definitely be true this case.
            {
                // Triangle points 0 and 2 are connected to the group
                // Therefore we are adding Triangle point 1 to the group
                target_triangle_point = 1;
                add_before = Mathf.Max(indecies[0], indecies[2]);

            }
        }
        else //if (indecies[1] != -1 && indecies[2] != -1) // This should definitely be true in this case.
        {
            // Triangle points 1 and 2 are connected to the group
            // Therefore we are adding Triangle point 0 to the group
            target_triangle_point = 0;
            add_before = Mathf.Max(indecies[1], indecies[2]);
        }

        // Make an indenting removal
        new_points = new Vector2[collider.points.Length + 1];
        j = 0;
        for (i = 0; i < new_points.Length; i++)
        {
            if (i == add_before)
            {
                new_points[i] = triangle_points[target_triangle_point];
            }
            else
            {
                new_points[i] = collider.points[j];
                j++;
            }
        }
        collider.points = new_points;
    }

    public void Remove(int x, int y)
    {
        if(triangles[x,y] != null)
        {
            //TODO Check for splitting
            // Triangle might be the only thing connecting 2 clusters together.
            n_triangles--;
            UpdateDrag();
            triangles[x, y] = null;
            RemoveTriangleFromCollider(x, y);
        }
    }
    public void Remove(Triangle t)
    {
        for (int x = 0; x < x_length; x++)
        {
            for (int y = 0; y < y_length; y++)
            {
                if(triangles[x,y] == t)
                {
                    Remove(x, y);
                    return;
                }
            }
        }
    }

    // Ensure that the array will fit a triangle at the parsed positon
    bool ResizeArray(int x_fit, int y_fit)
    {
        int x_max = x_length - 1 - x_origin;
        int x_min = -x_origin;
        int y_max = y_length - 1 - y_origin;
        int y_min = -y_origin;

        int x_mod = 0, y_mod = 0;   // Directions in which the array has to be resized

        if (x_fit > x_max)
        {
            x_mod = +1;
        }
        else if (x_fit < x_min)
        {
            x_mod = -1;
        }
        if (y_fit > y_max)
        {
            y_mod = +1;
        }
        else if (y_fit < y_min)
        {
            y_mod = -1;
        }


        if (x_mod == 0 && y_mod == 0)
        {
            // The triangle will fit in the array without resizing.
            return true;
        }
        else
        {
            // resize array to fit new triangle
            Triangle[,] newTriangles;
            short x_origin_mod = 0, y_origin_mod = 0;
            if (x_mod == -1)
            {
                x_origin_mod = 1;
                x_origin++;
            }
            if (y_mod == -1)
            {
                y_origin_mod = 1;
                y_origin++;
            }

            newTriangles = new Triangle[x_length + Mathf.Abs(x_mod),
                                        y_length + Mathf.Abs(y_mod)];

            for (int x = 0; x < x_length; x++)
            {
                for (int y = 0; y < y_length; y++)
                {
                    newTriangles[x+x_origin_mod, y+y_origin_mod] = triangles[x, y];
                }
            }
            triangles = newTriangles;
            x_length += Mathf.Abs(x_mod);
            y_length += Mathf.Abs(y_mod);
            return true;
        }
    }

    bool IsTouchingGroup(int x, int y, bool up)
    {
        if(triangles.Length == 1 && triangles[0,0] == null)
        {
            x_origin = -x;
            y_origin = -y;
            return true;
        }

        int x_in_array = x + x_origin,
            y_in_array = y + y_origin;
        // Check if the position is close enough to be touching the group.
        if (x_in_array < -1 || x_in_array > x_length
         || y_in_array < -1 || y_in_array > y_length)   // Completely OOB
        {
            return false;
        }
        else if ((x_in_array < 0 || x_in_array > x_length - 1)      // On the corners
              && (y_in_array < 0 || y_in_array > y_length - 1))
        {
            return false;
        }

        // Check the triangles surrounding the position to see if any are there.
        if(y_in_array >= 0 && y_in_array < y_length)
        {
            if (x_in_array > 0)
            {
                if (triangles[x_in_array - 1, y_in_array] != null)
                {
                    return true;
                }
            }
            if (x_in_array < x_length - 1)
            {
                if (triangles[x_in_array + 1, y_in_array] != null)
                {
                    return true;
                }
            }
        }
        if (x_in_array >= 0 && x_in_array < x_length)
        {
            if (up)
            {
                if (y_in_array > 0)
                {
                    if (triangles[x_in_array, y_in_array - 1] != null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (y_in_array < y_length - 1)
                {
                    if (triangles[x_in_array, y_in_array + 1] != null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /* Splits this TileGroup in 2.
     * @return true if successful, false if failed
     */
    public Rigidbody2D Split(Ant2 splitter)
    {
        for(int x = 0; x < x_length; x++)
        {
            for(int y = 0; y < y_length; y++)
            {
                if (triangles[x,y].Touching(splitter))
                {
                    TriangleGroup new_group = Instantiate(triangleGroupPrefab);
                    triangles[x, y].JoinGroup(new_group, x, y);
                }
            }
        }
        return rigidbody;   
    }

    public void Lift(float strength)
    {
        amount_lifted += strength;
        UpdateDrag();
    }

    public void Drop(float strength)
    {
        amount_lifted -= strength;
        UpdateDrag();
    }

    public void UpdateDrag()
    {
        rigidbody.drag = default_drag * Mathf.Max(0, n_triangles - amount_lifted);
        rigidbody.angularDrag = default_angular_drag * Mathf.Max(0, n_triangles - amount_lifted);
    }
}
