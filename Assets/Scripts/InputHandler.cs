using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    /// <summary>
    /// Filepath of video file input field
    /// </summary>
    [SerializeField] TMPro.TMP_InputField videoFileInput;

    /// <summary>
    /// Filepath of gazedata file input field
    /// </summary>
    [SerializeField] TMPro.TMP_InputField gazedataFileInput;

    /// <summary>
    /// Filepath of table configuration file input field
    /// </summary>
    [SerializeField] TMPro.TMP_InputField tableConfigInput;

    /// <summary>
    /// Decimation input field
    /// </summary>
    [SerializeField] TMPro.TMP_InputField decimationInput;

    /// <summary>
    /// Filepath of output folder input field
    /// </summary>
    [SerializeField] TMPro.TMP_InputField outputFolderInput;

    /// <summary>
    /// Put the input of the input fields into the configuration manager
    /// </summary>
    public void ProcessInput()
    {
        // Check if all fields are filled in
        if (String.IsNullOrEmpty(videoFileInput.text) || String.IsNullOrEmpty(gazedataFileInput.text) || String.IsNullOrEmpty(tableConfigInput.text) || String.IsNullOrEmpty(decimationInput.text) || String.IsNullOrEmpty(outputFolderInput.text))
        {
            return;
        }

        // Set video file path in configuration manager
        ConfigurationManager.Instance.videoPath = videoFileInput.text;

        // Set gazedata file path in configuration manager
        ConfigurationManager.Instance.gazedataPath = gazedataFileInput.text;

        // Set decimation in configuration manager
        ConfigurationManager.Instance.decimation = int.Parse(decimationInput.text, CultureInfo.InvariantCulture);

        // Set output file path in configuration manager
        if (outputFolderInput.text.Substring(outputFolderInput.text.Length - 1) == "\\" || outputFolderInput.text.Substring(outputFolderInput.text.Length - 1) == "/")
        {
            ConfigurationManager.Instance.outputPath = outputFolderInput.text + "results.txt";
        } else
        {
            ConfigurationManager.Instance.outputPath = outputFolderInput.text + "/results.txt";
        }

        // Get lines from table configuration file
        List<string> lines = System.IO.File.ReadLines(tableConfigInput.text).ToList();

        // Set table scale in configuration manager
        float tableScaleX = float.Parse(lines[0].Split('=')[1], CultureInfo.InvariantCulture) * 0.1f;
        float tableScaleY = float.Parse(lines[1].Split('=')[1], CultureInfo.InvariantCulture) * 0.1f;
        ConfigurationManager.Instance.tableScale = new Vector2(tableScaleX, tableScaleY);

        // Set scale calibration in configuration managers
        ConfigurationManager.Instance.scaleCalibration = float.Parse(lines[2].Split('=')[1], CultureInfo.InvariantCulture);

        // Set tag size in configuration managers
        ConfigurationManager.Instance.tagSize = float.Parse(lines[3].Split('=')[1], CultureInfo.InvariantCulture);

        // Get a list of the digital Apriltags in the table configuration file (format is apriltagID,xPosition,zPosition)
        List<string> digitalApriltagsInputList = lines[4].Split('=')[1].Split(';').ToList();

        // For each digital Apriltag in the table configuration file..
        foreach (string digitalApriltagInput in digitalApriltagsInputList)
        {
            // Split the input digital Apriltag into the 3 values (apriltagID, xPosition and zPosition)
            List<string> digitalApriltagInputData = digitalApriltagInput.Split(',').ToList();

            // Get the digital Apriltag ID
            int apriltagID = int.Parse(digitalApriltagInputData[0], CultureInfo.InvariantCulture);

            // Create a Vector3 for the digital Apriltag offset
            Vector3 offset = new Vector3(
                float.Parse(digitalApriltagInputData[1], CultureInfo.InvariantCulture), 
                0, 
                float.Parse(digitalApriltagInputData[2], CultureInfo.InvariantCulture)
            );

            // Add the digital Apriltag to the table configuration file
            ConfigurationManager.Instance.digitalApriltags.Add(new DigitalApriltag(apriltagID, offset));
        }

        ScenesManager.Instance.LoadEyetrackingProcessorScene();
    }

    public void SelectVideo()
    {
        // TODO: Select video button
    }
}
