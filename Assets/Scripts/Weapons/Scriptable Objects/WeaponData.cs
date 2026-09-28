using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
   //the name that is displayed to the player
   [Header("Weapon Properties")]
   public string weaponName;
   public float damage;
   public float range;

   public enum FireType
   {
      Single,
      FullAuto
   };
   
   public FireType fireType;
   
   public float fireRate;
   
   public int magSize;
   public int maxCarryCapacity;
   public float reloadTime;

   public float equipTime;
   public float holsterTime;


   //Controls the values that get added to target rotation; higher the val, more recoil
    public Vector3 recoil;
   //Controls how quickly camera returns to normal position after recoil was applied
   public float returnSpeed;
   //Controls how violently we transition into recoil
   public float snappiness;
}
