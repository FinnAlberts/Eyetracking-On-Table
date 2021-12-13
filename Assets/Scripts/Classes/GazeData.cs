using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Eye
{
    public List<double> gazeorigin;
    public List<double> gazedirection;
    public double pupildiameter;
}

[System.Serializable]
public class Data
{
    public List<double> gaze2d;
    public List<double> gaze3d;
    public Eye eyeleft;
    public Eye eyeright;
}

[System.Serializable]
public class Gazedata
{
    public string type;
    public double timestamp;
    public Data data;
}

[System.Serializable]
public class GazeDatas
{
    public List<Gazedata> gazedata;
}
