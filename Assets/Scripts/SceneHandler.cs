using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scenes
/// </summary>
public class SceneHandler : MonoBehaviour
{
    /// <summary>
    /// Scene for configuration
    /// </summary>
    [SerializeField] SceneAsset configuration;

    /// <summary>
    /// Scene for processing of eyetracking data
    /// </summary>
    [SerializeField] SceneAsset eyetrackingProcessor;

    /// <summary>
    /// Scene for when processing is complete
    /// </summary>
    [SerializeField] SceneAsset processingComplete;

    #region Make class into a DontDestroyOnLoad singleton
    private static SceneHandler _instance;

    public static SceneHandler Instance { get { return _instance; } }

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
    public void loadConfigurationScene()
    {
        SceneManager.LoadScene(configuration.name);
    }

    /// <summary>
    /// Load the eyetracking processor scene
    /// </summary>
    public void loadEyetrackingProcessorScene()
    {
        SceneManager.LoadScene(eyetrackingProcessor.name);
    }

    /// <summary>
    /// Load the processing complete scene
    /// </summary>
    public void loadprocessingCompleteScene()
    {
        SceneManager.LoadScene(processingComplete.name);
    }
}
