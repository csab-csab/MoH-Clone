using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    [SerializeField]private int targetFrameRate;

    void Start()
    {
        ChangeFrameRate(targetFrameRate);
    }
    
    private static void ChangeFrameRate(int targetFrameRate)
    {
        Application.targetFrameRate = targetFrameRate;
    }

    //Gets called whenever a value gets changed in the inspector
    private void OnValidate()
    {
        ChangeFrameRate(targetFrameRate);
    }
}
