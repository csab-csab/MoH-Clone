using UnityEngine;

//Responsible for setting the camera and arm rigs position when crouching
public class CameraOffsetter : MonoBehaviour
{
   private Transform _camPivotTransform;
   
   [SerializeField] private Transform normalCamPos;
   [SerializeField]private Transform crouchCamPos;
   //The speed at which the camera moves from one positon to the other
   private float _cameraMoveSpeed;
   
   private PlayerMovement _movementScript;
   
   private void Awake()
   {
      _camPivotTransform = this.transform;
      _movementScript = GetComponentInParent<PlayerMovement>();
      _cameraMoveSpeed = _movementScript.ReturnCrouchSpeed();
   }

   private void OnEnable()
   {
      _movementScript.OnCrouchChanged += HandleCrouchPos;
   }

   private void OnDisable()
   {
      _movementScript.OnCrouchChanged -= HandleCrouchPos;
   }
   
   //Offsets camera and arm rig if player is crouching
   private void HandleCrouchPos(bool isCrouching)
   {
     var targetPosition = isCrouching? crouchCamPos.position : normalCamPos.position;  
     
      _camPivotTransform.position = new Vector3(targetPosition.x, 
         Mathf.MoveTowards(_camPivotTransform.position.y, targetPosition.y, 
            _cameraMoveSpeed * Time.deltaTime), _camPivotTransform.position.z);
   }
}
