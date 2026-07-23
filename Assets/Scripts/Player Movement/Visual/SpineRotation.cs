using UnityEngine;

//This script is responsible for visually updating the torso and arm rig rotation
public class SpineRotation : MonoBehaviour
{
  [SerializeField] private Transform spineParent;
  [SerializeField] private Transform cameraPivot;

  //used to determine whether script executes or not
  [SerializeField]private bool isEnabled = false;

  void Start()
  {
      if (spineParent != null && cameraPivot != null)
      {
          isEnabled = true;
      }
      else
      {
          isEnabled = false;
      }
  }
  
  //use late update to leave animation unaffected
  private void LateUpdate()
  {
      if (isEnabled)
      {
          // Get the X rotation (pitch)
          float pitch = cameraPivot.localEulerAngles.x;

          // Convert Unity's 0-360 to a -180 to 180 range
          if (pitch > 180) pitch -= 360;

          // Apply it to the spine. 
          // Note: You can multiply pitch by a value (like 0.5f) 
          // so the spine doesn't bend quite as far as the eyes.
          spineParent.localRotation = Quaternion.Euler(pitch, 0, 0);
      }
  }
}
