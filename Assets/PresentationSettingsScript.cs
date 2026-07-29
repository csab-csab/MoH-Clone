using System.IO;
using UnityEngine;
using UnityEngine.Serialization;


//Class Wrapper that mirrors our JSON structure
[System.Serializable]
public class PresentationSettingsData
{
    public int target_frame_rate;

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
    [SerializeField] private PresentationSettingsData presentationSettingsData;
    
    // => automatically maps these values to the wrapper class's
    private int targetFrameRate => presentationSettingsData.target_frame_rate;

    private int resolutionWidth => presentationSettingsData.target_resolution.width ;
    private int resolutionHeight => presentationSettingsData.target_resolution.height;
    
    void Awake()
    {
        LoadSettingsFromFile();
    }

    private void LoadSettingsFromFile()
    {
        //path to our presentation_settings.json file
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "presentation_settings.json");

        if (File.Exists(jsonPath))
        {
            //Read file content and store it as a string
           string jsonContent = File.ReadAllText(jsonPath);

           //this automatically maps our JSON file to our wrapper class above
           presentationSettingsData = JsonUtility.FromJson<PresentationSettingsData>(jsonContent);
           
           ChangeResolution();
           ChangeFrameRate();
        }
        else
        {
            Debug.LogError($"{jsonPath}: This File or path could not be found!");
        }
    }
    
    private void ChangeResolution()
    {
        Screen.SetResolution(resolutionWidth, resolutionHeight, FullScreenMode.Windowed);
    }
    
    private void ChangeFrameRate()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
