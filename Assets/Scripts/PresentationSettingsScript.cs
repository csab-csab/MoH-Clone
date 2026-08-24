using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;


//Class Wrapper that mirrors our JSON structure
[System.Serializable]
public class PresentationSettingsData
{
    public int target_frame_rate;

    [System.Serializable]
    public struct TargetResolutionStruct
    {
        public int width;
        public int height; 
    }
    
    //dont rename; must match json
    public TargetResolutionStruct target_resolution;
   
}


public class PresentationSettingsScript : MonoBehaviour
{
    private PresentationSettingsData _presentationSettingsData;
    
    // => automatically maps these values to the wrapper class's
    private int targetFrameRate => _presentationSettingsData.target_frame_rate;

    private int resolutionWidth => _presentationSettingsData.target_resolution.width ;
    private int resolutionHeight => _presentationSettingsData.target_resolution.height;
    
    void Start()
    { 
        _presentationSettingsData = CentralDataManager.instance.ReturnPresentationSettingsData();
        
        ChangeResolution();
        ChangeFrameRate();
    }

    private void ChangeResolution()
    {
        Screen.SetResolution(resolutionWidth, resolutionHeight, FullScreenMode.FullScreenWindow);
    }
    
    private void ChangeFrameRate()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
