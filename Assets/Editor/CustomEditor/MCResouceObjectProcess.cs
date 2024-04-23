using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;


[CustomEditor(typeof(MCResouceObject))]
public class MCResouceObjectProcess:Editor
{
 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();  
 
        MCResouceObject script = (MCResouceObject)target;


        if (GUILayout.Button("Process Texture"))
        {
            script.ProcessTexture();
        }
        if (GUILayout.Button("Process"))
        {
            script.Process();
        }
        if (GUILayout.Button("ProcessSaving")){
            script.ProcessSaving();
        }
    }

 
}
