using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set the path in the end text of the process complete scene
/// </summary>
public class SetEndText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text endText;
    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        endText.text = String.Format("Your data has been processed. Find your data at: {0}", ConfigurationManager.Instance.outputPath);
    }
}
