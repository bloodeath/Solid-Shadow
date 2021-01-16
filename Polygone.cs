using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Polygone : MonoBehaviour
{
    private MeshFilter m_mfMeshFilter;
    private PolygonCollider2D m_pcPolygonCollider2D;

    public List<Vector3> m_lsPoints;
    public List<int> m_lsTriangles;

    private void Start()
    {
        m_mfMeshFilter = GetComponent<MeshFilter>();
        m_pcPolygonCollider2D = GetComponent<PolygonCollider2D>();
        generateObject();
    }


    private void generateObject()
    {
        m_mfMeshFilter.mesh.SetVertices(m_lsPoints);
        m_mfMeshFilter.mesh.SetTriangles(m_lsTriangles.ToArray(), 0);

        List<Vector2> points = new List<Vector2>();
        foreach (Vector3 v3 in m_lsPoints)
            points.Add(v3);

        m_pcPolygonCollider2D.SetPath(0, points);
    }
}
