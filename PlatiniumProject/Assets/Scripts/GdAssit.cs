using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GdAssit : EditorWindow
{
    [MenuItem("ToolBox/ Gd assist window")]
    static void InitWindow()
    {
        GdAssit window = GetWindow<GdAssit>();
        window.titleContent = new GUIContent("GD assist");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Alignement");
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("HorizontalSnap"))
            {
                SnapHorizontal();
            }
            if (GUILayout.Button("VerticalSnap"))
            {
                SnapVertical();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("GapAlignement");

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("HorizontalGapAdjust"))
            {
                SnapHorizontalGap();
            }
            if (GUILayout.Button("VerticalGapAdjust"))
            {
                SnapVerticalGap();
            }
        }
        GUILayout.EndHorizontal();


    }

    private void SnapVerticalGap()
    {
        if (Selection.objects.Length <= 1)
            return;

        List<float> poss = new List<float>();
        List<GameObject> gos = new List<GameObject>();

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            GameObject go = Selection.objects[i] as GameObject;
            gos.Add(go);
            poss.Add(go.transform.position.y);
        }
        float sum = 0;
        for (int i = 1; i < poss.Count; ++i)
        {
            float dif = poss[i - 1] - poss[i];
            sum += dif;
        }
        sum /= poss.Count - 1;

        for (int i = 0; i < gos.Count; ++i)
        {
            Undo.RecordObject(gos[i].transform, "snap object");
            gos[i].transform.position = new Vector3(gos[0].transform.position.x, gos[0].transform.position.y + (sum * i), gos[0].transform.position.z);
        }
    }

    private void SnapHorizontal()
    {
        if (Selection.objects.Length <= 1)
            return;

        List<float> poss = new List<float>();
        List<GameObject> gos = new List<GameObject>();

        for(int i = 0; i < Selection.objects.Length; i++)
        {
            GameObject go = Selection.objects[i] as GameObject;
            gos.Add(go);
            poss.Add(go.transform.position.y);
        }
        float sum = 0;
        for(int i = 0; i < poss.Count; ++i)
        {
            sum += poss[i];
        }
        sum /= poss.Count;

        foreach(GameObject go in gos)
        {
            Undo.RecordObject(go.transform, "snap object");
            go.transform.position = new Vector3(go.transform.position.x, sum, go.transform.position.z);
        }
    }

    private void SnapHorizontalGap()
    {
        if (Selection.objects.Length <= 1)
            return;

        List<float> poss = new List<float>();
        List<GameObject> gos = new List<GameObject>();

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            GameObject go = Selection.objects[i] as GameObject;
            gos.Add(go);
            poss.Add(go.transform.position.x);
        }
        float sum = 0;
        for (int i = 1; i < poss.Count; ++i)
        {
            float dif = poss[i - 1] - poss[i];
            sum += dif;
        }
        sum /= poss.Count - 1;

        for(int i = 0; i < gos.Count; ++i)
        {
            Undo.RecordObject(gos[i].transform, "snap object");
            gos[i].transform.position = new Vector3(gos[0].transform.position.x + (sum * i), gos[0].transform.position.y, gos[0].transform.position.z);
        }
    }

    private void SnapVertical()
    {
        if (Selection.objects.Length <= 1)
            return;

        List<float> poss = new List<float>();
        List<GameObject> gos = new List<GameObject>();

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            GameObject go = Selection.objects[i] as GameObject;
            gos.Add(go);
            poss.Add(go.transform.position.x);
        }
        float sum = 0;
        for (int i = 0; i < poss.Count; ++i)
        {
            sum += poss[i];
        }
        sum /= poss.Count;

        foreach (GameObject go in gos)
        {
            Undo.RecordObject(go.transform, "snap object");
            go.transform.position = new Vector3(sum, go.transform.position.y, go.transform.position.z);
        }
    }
}
