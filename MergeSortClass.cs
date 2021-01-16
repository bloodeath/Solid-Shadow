using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MergeSortClass
{
    public static List<Rayon> MergeSort(List<Rayon> unsorted, Vector2 v2)
    {
        if (unsorted.Count <= 1)
            return unsorted;

        List<Rayon> left = new List<Rayon>();
        List<Rayon> right = new List<Rayon>();

        int middle = unsorted.Count / 2;

        for (int i = 0; i < middle; i++)  //Dividing the unsorted list
            left.Add(unsorted[i]);
        
        for (int i = middle; i < unsorted.Count; i++)
            right.Add(unsorted[i]);
        

        left = MergeSort(left, v2);
        right = MergeSort(right, v2);
        return Merge(left, right, v2);
    }

    private static List<Rayon> Merge(List<Rayon> left, List<Rayon> right, Vector2 v2)
    {
        List<Rayon> result = new List<Rayon>();

        while (left.Count > 0 || right.Count > 0)
        {
            if (left.Count > 0 && right.Count > 0)
            {
                float angle1 = Vector2.SignedAngle(v2, left[0].direction);
                float angle2 = Vector2.SignedAngle(v2, right[0].direction);
                

                if (angle1 <= angle2)  //Comparing First two elements to see which is smaller
                {
                    result.Add(left[0]);
                    left.Remove(left[0]);      //Rest of the list minus the first element
                }
                else
                {
                    result.Add(right[0]);
                    right.Remove(right[0]);
                }
            }
            else if (left.Count > 0)
            {
                result.Add(left[0]);
                left.Remove(left[0]);
            }
            else if (right.Count > 0)
            {
                result.Add(right[0]);
                right.Remove(right[0]);
            }
        }
        return result;
    }
}

