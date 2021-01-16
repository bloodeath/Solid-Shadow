using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(EdgeCollider2D))]
public class Shadow : MonoBehaviour
{
    private EdgeCollider2D m_pcEdgeCollider2D;
    private List<Segment> m_lsSegments = new List<Segment>();
    private List<Segment> m_lsNonStaticSegment = new List<Segment>();
    private List<Rayon> m_lsRays = new List<Rayon>();
    private List<Vector2> m_lsPoints = new List<Vector2>();

    public List<PolygonCollider2D> m_lsGameObjects;
    public List<PolygonCollider2D> m_lsNonStaticGameObjects;
    public float opening = 180.0f;
    public float orientation = 0.0f;

    private void Start()
    {
        //on récupére le collider qui nous servira à faire les collisions des ombres
        m_pcEdgeCollider2D = GetComponent<EdgeCollider2D>();
        GetSegments();
    }

    private void Update()
    {
        GetNonStaticSegments();
        GetRays();
        Vector2 vec = new Vector2(Mathf.Cos((orientation - opening) * Mathf.Deg2Rad), Mathf.Sin((orientation - opening) * Mathf.Deg2Rad));
        m_lsRays = MergeSortClass.MergeSort(m_lsRays, vec);

        m_lsPoints = new List<Vector2>();
        GetIntersections();
        m_pcEdgeCollider2D.points = m_lsPoints.ToArray();
    }

    //le but de cette fonctions est de récupéré les segments des différents objets dans la scène
    private void GetSegments()
    {
        m_lsSegments = new List<Segment>();
        //pour tous les objets, on récupére son collider
        foreach (PolygonCollider2D poly in m_lsGameObjects)
        {
            //puis on ajoute tous les segments qui le constitue
            m_lsSegments.Add(new Segment(poly.points[poly.points.Length - 1] + (Vector2)poly.transform.position, poly.points[0] + (Vector2)poly.transform.position));
            for (int i = 0; i < poly.points.Length - 1; i++)
                m_lsSegments.Add(new Segment(poly.points[i] + (Vector2)poly.transform.position, poly.points[i + 1] + (Vector2)poly.transform.position));
        }

        //on ajoute aussi les côté de l'écran
        Vector2 size = Camera.main.ViewportToWorldPoint(Vector2.one);
        m_lsSegments.Add(new Segment(new Vector2(-size.x, -size.y), new Vector2(-size.x, size.y)));
        m_lsSegments.Add(new Segment(new Vector2(-size.x, -size.y), new Vector2(size.x, -size.y)));
        m_lsSegments.Add(new Segment(new Vector2(size.x, -size.y), new Vector2(size.x, size.y)));
        m_lsSegments.Add(new Segment(new Vector2(size.x, size.y), new Vector2(-size.x, size.y)));
    }

    //le but de cette fonctions est de récupéré les segments des différents objets dans la scène qui vont pouvoir bougé
    private void GetNonStaticSegments()
    {
        m_lsNonStaticSegment = new List<Segment>();
        //pour tous les objets, on récupére son collider
        foreach (PolygonCollider2D poly in m_lsNonStaticGameObjects)
        {
            //puis on ajoute tous les segments qui le constitue
            m_lsNonStaticSegment.Add(new Segment(poly.points[poly.points.Length - 1] + (Vector2)poly.transform.position, poly.points[0] + (Vector2)poly.transform.position));
            for (int i = 0; i < poly.points.Length - 1; i++)
                m_lsNonStaticSegment.Add(new Segment(poly.points[i] + (Vector2)poly.transform.position, poly.points[i + 1] + (Vector2)poly.transform.position));
        }
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
           
                if (angle <= opening*2 && angle >= 0)
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

    //une fonction afin de définir trois rayon avec un décalage très faible pour pouvoir avoir des rayons qui ne s'arréte pas au premier points
    private void defineRay(Rayon ray)
    {
        //on ajoute le rayon de base
        m_lsRays.Add(ray);
        //décalage de +0.001 degré
        m_lsRays.Add(new Rayon(transform.position, new Vector2(
            ray.direction.x - ray.direction.y * 0.001f,
            ray.direction.x * 0.001f + ray.direction.y
            )));
        //décalage de -0.001 degré
        m_lsRays.Add(new Rayon(transform.position, new Vector2(
            ray.direction.x - ray.direction.y * -0.001f,
            ray.direction.x * -0.001f + ray.direction.y
            )));
    }

    //le but de cette fonction est de calculé l'intersection entre les rayons et les segments
    private void GetIntersections()
    {
        //initialisation de variable de base
        Intersection intersection, temp;
        //on ajoute le centre de l'objet pour pouvoir donné un côté de rayon projetté 
        m_lsPoints.Add(Vector2.zero);
        //pour tous les rayons on va calculé le point d'intersection
        foreach (Rayon ray in m_lsRays)
        {
            //réinitialisaion afin de pouvoir traite les rayon de manière indépendante
            intersection = new Intersection(Vector2.zero, 1000000);

            //pour chaque segment on récupére l'intersection
            foreach (Segment seg in m_lsSegments)
            {
                temp = GetIntersection(ray, seg);

                //si elle est plus proche, on la garde en mémoire et on retire l'ancienne
                if (temp.param < intersection.param && temp.param != -1)
                {
                    intersection.param = temp.param;
                    intersection.pts = temp.pts;
                }
            }
            //pour chaque segment on récupére l'intersection
            foreach (Segment seg in m_lsNonStaticSegment)
            {
                temp = GetIntersection(ray, seg);

                //si elle est plus proche, on la garde en mémoire et on retire l'ancienne
                if (temp.param < intersection.param && temp.param != -1)
                {
                    intersection.param = temp.param;
                    intersection.pts = temp.pts;
                }
            }
            //si il y a eu une intersection, on ajoute le point
            if (intersection.param != 1000000)
                m_lsPoints.Add(intersection.pts - (Vector2)transform.position);
        }
        // on referme le edge
        m_lsPoints.Add(Vector2.zero);
    }

    //fonction qui permet de récupéré l'intersection entre un rayon et un segment
    private Intersection GetIntersection(Rayon ray, Segment seg)
    {
        Intersection intersection = new Intersection(Vector2.zero, -1);

        Vector2 sdir = seg.direction - seg.origin;
        //on calcule la magnétude du rayon et du segment afin de pouvoir déterminé si ils sont parallèles
        float r_mag = Mathf.Sqrt(ray.direction.x * ray.direction.x + ray.direction.y * ray.direction.y);
        float s_mag = Mathf.Sqrt(sdir.x * sdir.x + sdir.y * sdir.y);

        if (ray.direction.x / r_mag == sdir.x / s_mag && ray.direction.y / r_mag == sdir.y / s_mag)
            return intersection;

        //on calcule la solution du système d'équation entre celle du rayon et celle du segment
        float T2 = (ray.direction.x * (seg.origin.y - ray.origin.y) + ray.direction.y * (ray.origin.x - seg.origin.x)) / ((sdir.x * ray.direction.y) - (sdir.y * ray.direction.x));
        float T1 = (seg.origin.x + sdir.x * T2 - ray.origin.x) / ray.direction.x;

        // si celle ci est négative (donc innexcistante) ou que T2 (la solution pour le segment) n'est pas dans le segment, on stope la fonction
        if (T1 < 0) return intersection;
        if (T2 < -0.0001f || T2 > 1.0001f) return intersection;

        //on retourne le point d'intersection
        intersection = new Intersection();
        intersection.pts = ray.origin + (ray.direction * T1);
        intersection.param = T1;
        return intersection;
    }
}