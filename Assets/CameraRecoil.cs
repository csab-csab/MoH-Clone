using System;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
   private Vector3 _currentRotation;
   private Vector3 _targetRotation;
   
   //Controls the values that get added to target rotation; higher the val, more recoil
   [SerializeField]private Vector3 _recoil;
   //Controls how quickly camera returns to normal position after recoil was applied
   [SerializeField]private float _returnSpeed;
   //Controls how violently we transition into recoil
   [SerializeField]private float _snappiness;
   

   public void ApplyRecoilProfile(Vector3 recoilProfile, float returnSpeed, float snappiness)
   {
      throw new NotImplementedException();
   }

   private void Update()
   {
      //Target rotation always trying to move back to zero; return to base pos
      _targetRotation = Vector3.Slerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
      _currentRotation =  Vector3.Slerp(_currentRotation, _targetRotation, _snappiness * Time.fixedDeltaTime);
      transform.localRotation = Quaternion.Euler(_currentRotation);
   }

   public void ApplyRecoil()
   {
      //Add recoil to values to target value; randomising z and y recoil
      _targetRotation += new Vector3(_recoil.x, UnityEngine.Random.Range(-_recoil.y, _recoil.y), UnityEngine.Random.Range(-_recoil.z, _recoil.z));
   }
}
