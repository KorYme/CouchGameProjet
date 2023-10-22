using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.TerrainTools;
using UnityEngine;
[CustomEditor(typeof(TransitQueue))]
public class TransitQueueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TransitQueue transitQueue = (TransitQueue)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Delete"))
        {
            transitQueue.DeleteAll();
        }

    }
    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
        }
        SceneView.onSceneGUIDelegate += OnScene;
    }

    void OnScene(SceneView scene)
    {
        Event e = Event.current;
        TransitQueue transitQueue = (TransitQueue)target;
        if (e.type == EventType.MouseDown && e.button == 2 && Selection.Contains(transitQueue.gameObject))
        {
            Debug.Log("Middle Mouse was pressed");

            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;

            Ray ray = scene.camera.ScreenPointToRay(mousePos);
            transitQueue.AddSlot(ray.origin);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Do something, ---Example---
            }
            e.Use();
        }
        if(transitQueue.Slots.Count <= 0)
            return;
        
        for (int i = 1; i < transitQueue.Slots.Count; ++i)
        {
            Handles.color = Color.red;
            Handles.DrawLine(transitQueue.Slots[i - 1].transform.position,transitQueue.Slots[i].transform.position);
            Handles.Label(transitQueue.Slots[i - 1].transform.position + new Vector3(0.4f,-0.4f,0f), i.ToString());
        }
    }
}
