using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorScript : MonoBehaviour
{ 
    //Used to keep track of _target rotation so doors can be opened and closed
    [Tooltip("This means door can be opened and closed")]
    [SerializeField]private bool canBeToggled = true;
    private bool _firstOpen = true;  
    
    [Header("Door Properties")]
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private float animationSpeed = 180f;

    [Header("Open/Close Properties")] 
    private Quaternion _targetRotation;
    private bool _isMoving  = false;

    [Tooltip("Input desired y-rotation here")]
    [SerializeField]float targetRotation;
    private float _initialRotation;

    void Start()
    {
        _initialRotation = transform.localRotation.eulerAngles.y;
        _targetRotation =  Quaternion.Euler(new Vector3(transform.localEulerAngles.x,targetRotation,transform.localEulerAngles.z));
    }

    //using an IENUMRATOR instead of a regular function as this doesn't freeze the main thread while executing the while
    private IEnumerator AnimateDoor()
    {
        //Determined whether door should be opened or closed
        if (canBeToggled && !_firstOpen)
        {
           //swap target and initial rotation based on if the door needs to be closed or opened
           (targetRotation, _initialRotation) =  (_initialRotation, targetRotation);
           
           //Recalculate _targetRotation with the newly swapped angle
           _targetRotation = Quaternion.Euler(transform.localEulerAngles.x, targetRotation, transform.localEulerAngles.z);
        }
        
        _isMoving = true;
        while (Quaternion.Angle(transform.localRotation, _targetRotation ) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetRotation, Time.deltaTime * animationSpeed);
            yield return null;
        } 
        
        transform.localRotation = _targetRotation;
        _isMoving = false;

        if (_firstOpen)
        {
            _firstOpen = false;  
        }
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
