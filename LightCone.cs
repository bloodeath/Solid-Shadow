using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCone : LightSources
{
    public float opening = 180.0f;
    public float orientation = 0.0f;

    private void Start()
    {
        GetSegments();
    }

    private void Update()
    {
        cast();
    }

    //le but de cette fonction est de déterminé l'ensemble des rayons qui vont être projetté sur les segments
    private void GetRays()
    {
        float angle;
        m_lsRays = new List<Rayon>();
        Rayon ray;
        Vector2 vec = new Vector2(Mathf.Cos((orientation - opening) * Mathf.Deg2Rad), Mathf.Sin((orientation - opening) * Mathf.Deg2Rad));

        foreach (Segment seg in m_lsSegments)
        {
            ray = new Rayon(transform.position, seg.origin - (Vector2)transform.position);
            angle = (Vector2.SignedAngle(vec, ray.direction));

            if (angle <= opening * 2 && angle >= 0)
                defineRay(ray);
        }

        if (m_lsSegments.Count > 0)
        {
            ray = new Rayon(transform.position, m_lsSegments[m_lsSegments.Count - 1].direction - (Vector2)transform.position);
            angle = (Vector2.SignedAngle(vec, ray.direction));

            if (angle <= opening * 2 && angle >= 0)
                defineRay(ray);
        }

        foreach (Segment seg in m_lsNonStaticSegment)
        {
            ray = new Rayon(transform.position, seg.origin - (Vector2)transform.position);
            angle = (Vector2.SignedAngle(vec, ray.direction));

            if (angle <= opening * 2 && angle >= 0)
                defineRay(ray);
        }

        if (m_lsNonStaticSegment.Count > 0)
        {
            ray = new Rayon(transform.position, m_lsNonStaticSegment[m_lsNonStaticSegment.Count - 1].direction - (Vector2)transform.position);
            angle = (Vector2.SignedAngle(vec, ray.direction));

            if (angle <= opening * 2 && angle >= 0)
                defineRay(ray);
        }

        ray = new Rayon(transform.position, new Vector2(Mathf.Cos((opening + orientation) * Mathf.Deg2Rad), Mathf.Sin((opening + orientation) * Mathf.Deg2Rad)));
        defineRay(ray);

        ray = new Rayon(transform.position, new Vector2(Mathf.Cos((-opening + orientation) * Mathf.Deg2Rad), Mathf.Sin((-opening + orientation) * Mathf.Deg2Rad)));
        defineRay(ray);

    }
    
    public override void cast()
    {
        GetNonStaticSegments();
        GetRays();
        Vector2 vec = new Vector2(Mathf.Cos((orientation - opening) * Mathf.Deg2Rad), Mathf.Sin((orientation - opening) * Mathf.Deg2Rad));
        m_lsRays = MergeSortClass.MergeSort(m_lsRays, vec);

        m_lsPoints = new List<Vector2>();
        GetIntersections();
    }
}
