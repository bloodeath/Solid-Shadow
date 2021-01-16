using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Intersection
{
    public Vector2 pts;
    public float param;

    public Intersection(Vector2 v1, float f1)
    {
        pts = v1;
        param = f1;
    }
}
public struct Segment
{
    public Vector2 origin, direction;
    public Segment(Vector2 v1, Vector2 v2)
    {
        origin = v1;
        direction = v2;
    }
}
public struct Rayon
{
    public Vector2 origin, direction;
    public Rayon(Vector2 v1, Vector2 v2)
    {
        origin = v1;
        direction = v2;
    }
}