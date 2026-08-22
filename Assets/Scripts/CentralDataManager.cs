using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

//This script is responsible for getting stats and settings from jsons in streaming assets folder and then
//providing methods so that other scripts can access them
public class CentralDataManager : MonoBehaviour
{
    public static CentralDataManager instance {get; private set;}
    
    #region Enemy Stats File Variables
    [Header("Enemy stats")]
    [SerializeField]private string enemyBaseStatsFilename = "Default_Enemy_Stats.json";
    [SerializeField]private EnemyBaseStats enemyBaseStats;
    #endregion
    
    #region Presentation Settings File Variables
    [Header("Presentation Settings")]
    [SerializeField]private string presentationSettingsFilename = "presentation_settings.json";
    [SerializeField] private PresentationSettingsData presentationSettingsData;
    #endregion
    

    public void Awake()
    {
        #region Singleton Setup

        //Singleton setup ensures only once instance of this exists
        if (instance != null && instance != this)
        {
            Destroy(instance);
            return;
        }

        instance = this;
        
        #endregion
        
        #region Call load data methods
        LoadEnemyBaseStatsJson();
        LoadSettingsJson();
        #endregion
    }

    #region Load data methods
    private void LoadEnemyBaseStatsJson()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, enemyBaseStatsFilename);

        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON file not found: " + jsonPath);
            return;
        }
        
        string jsonContent = File.ReadAllText(jsonPath);
        enemyBaseStats = JsonUtility.FromJson<EnemyBaseStats>(jsonContent);
    }
    
    private void LoadSettingsJson()
    {
        //path to our presentation_settings.json file
        string jsonPath = Path.Combine(Application.streamingAssetsPath, presentationSettingsFilename);

        if (!File.Exists(jsonPath))
        {
            if (!File.Exists(jsonPath))
            {
                Debug.LogError("JSON file not found: " + jsonPath);
                return;
            }
        }

        //Read file content and store it as a string
        string jsonContent = File.ReadAllText(jsonPath);
        //this automatically maps our JSON file to our wrapper class above
        presentationSettingsData = JsonUtility.FromJson<PresentationSettingsData>(jsonContent);
    }
    #endregion
    
    #region Return Wrapper Instances
    public EnemyBaseStats ReturnEnemyBaseStats()
    {
        if (enemyBaseStats == null)
        {
            Debug.LogError("Base stats is null");
            return null;
        }
       
        return enemyBaseStats;
    }

    public PresentationSettingsData ReturnPresentationSettingsData()
    {
        if (presentationSettingsData == null)
        {
            Debug.LogError("Presentation settings data is null");
            return null;
        }
        
        return presentationSettingsData;
    }
    #endregion
}
