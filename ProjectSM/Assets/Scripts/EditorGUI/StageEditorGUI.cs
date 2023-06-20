using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageEditor))]
public class StageEditorGUI : Editor
{
    private void OnSceneGUI()
    {
        Texture spawnPointTexture = Resources.Load("Development/GUI/StageSpawnPoint") as Texture;
        Texture stageTexture = Resources.Load("Development/GUI/Stage") as Texture;
        Handles.color = Color.magenta;
        Stage stage = ((StageEditor)target).stage;

        EditorGUI.BeginChangeCheck();

        Handles.Label(stage.transform.position, stageTexture);
        Vector3[] newPos = new Vector3[stage.spawnPoints.Length];

        for (int i = 0; i < newPos.Length; i++)
        {
            newPos[i] = Handles.PositionHandle(stage.spawnPoints[i], Quaternion.identity);
            Handles.Label(newPos[i], spawnPointTexture);
        }
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(stage, "Changed Stage GUI");
            for(int i = 0; i < newPos.Length; i++) 
            {
                stage.spawnPoints[i] = newPos[i];
            }
        }
    }
}
