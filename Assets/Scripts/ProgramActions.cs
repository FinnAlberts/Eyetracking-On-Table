using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramActions : MonoBehaviour
{
    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitApplication()
    {
        // Check if application is being run in editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
