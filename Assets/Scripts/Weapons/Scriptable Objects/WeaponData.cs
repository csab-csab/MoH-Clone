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
}
