using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scenes
/// </summary>
public class ScenesManager : MonoBehaviour
{

    #region Make class into a DontDestroyOnLoad singleton
    private static ScenesManager _instance;

    public static ScenesManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion 

    /// <summary>
    /// Load the configuration scene
    /// </summary>
    public void LoadConfigurationScene()
    {
        SceneManager.LoadScene("ConfigurationScene");
    }

    /// <summary>
    /// Load the eyetracking processor scene
    /// </summary>
    public void LoadEyetrackingProcessorScene()
    {
        SceneManager.LoadScene("EyetrackingProcessing");
    }

    /// <summary>
    /// Load the processing complete scene
    /// </summary>
    public void LoadprocessingCompleteScene()
    {
        SceneManager.LoadScene("ProcessingComplete");
    }
}
