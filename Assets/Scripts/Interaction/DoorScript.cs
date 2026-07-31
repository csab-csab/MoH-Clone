using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorScript : MonoBehaviour
{ 
    [SerializeField]private bool isOpen = false;
    
    [Header("Door Properties")]
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private float animationSpeed = 2f;

    [Header("Open/Close Properties")] 
    private Quaternion _targetRotation;
    private bool _isMoving  = false;

    [Tooltip("Input desired y-rotation here")]
    [SerializeField]float targetRotation;

    void Start()
    {
        _targetRotation =  Quaternion.Euler(new Vector3(transform.localEulerAngles.x,targetRotation,transform.localEulerAngles.z));
        InteractWithDoor();
    }

    //using an IENUMRATOR instead of a regular function as this doesn't freeze the main thread while executing the while
    private IEnumerator AnimateDoor()
    {
        _isMoving = true;
        while (Quaternion.Angle(transform.localRotation, _targetRotation ) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetRotation, Time.deltaTime * animationSpeed);
            yield return null;
        } 
        
        transform.localRotation = _targetRotation;
        _isMoving = false;
    }
    
    /// <summary>
   /// Triggers open Door
   /// </summary>
   /// <returns>true if door can be opened, false if door requires a key player doesn't have</returns>
   public bool InteractWithDoor(bool hasKey = false)
   {

       if (!hasKey && requiresKey || _isMoving)
       {
           return false;
       }
      
       StartCoroutine(AnimateDoor());
       return true;
   }
}
