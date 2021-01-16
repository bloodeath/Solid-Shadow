using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSphere : LightSources
{

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
        m_lsRays = new List<Rayon>();
        Rayon ray;

        foreach (Segment seg in m_lsSegments)
        {
            ray = new Rayon(transform.position, seg.origin - (Vector2)transform.position); 
            defineRay(ray);
        }

        if (m_lsSegments.Count > 0)
        {
            ray = new Rayon(transform.position, m_lsSegments[m_lsSegments.Count - 1].direction - (Vector2)transform.position);
            defineRay(ray);
        }

        foreach (Segment seg in m_lsNonStaticSegment)
        {
            ray = new Rayon(transform.position, seg.origin - (Vector2)transform.position);
            defineRay(ray);
        }

        if (m_lsNonStaticSegment.Count > 0)
        {
            ray = new Rayon(transform.position, m_lsNonStaticSegment[m_lsNonStaticSegment.Count - 1].direction - (Vector2)transform.position);
            defineRay(ray);
        }

    }

   
    public override void cast()
    {
        GetNonStaticSegments();
        GetRays();
        m_lsRays = MergeSortClass.MergeSort(m_lsRays, Vector2.right);

        m_lsPoints = new List<Vector2>();
        GetIntersections();
    }
}
