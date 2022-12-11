using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO.MemoryMappedFiles;

[CustomEditor(typeof(VisionCone))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        VisionCone cone = (VisionCone)target; 
        Handles.color = Color.white;
        Handles.DrawWireArc(cone.transform.position, Vector3.up, Vector3.forward, 360, cone.mRadius);
        Vector3 viewAngleA = cone.DirFromAngle(-cone.mVisionConeAngle / 2.0f, false);
        Vector3 viewAngleB = cone.DirFromAngle(cone.mVisionConeAngle / 2.0f, false);
        Handles.DrawLine(cone.transform.position, cone.transform.position + viewAngleA * cone.mRadius);
        Handles.DrawLine(cone.transform.position, cone.transform.position + viewAngleB * cone.mRadius);

        Handles.color = Color.red;
        foreach(GameObject visibleTarget in cone.mSensedObjects)
        {
            Handles.DrawLine(visibleTarget.transform.position, cone.transform.position);
        }    
    }
}
