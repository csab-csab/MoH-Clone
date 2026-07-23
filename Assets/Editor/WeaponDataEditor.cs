using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get Reference to weapon data script obj
        var weaponData = (WeaponData)target;
     
        //Draw Everything else
        
        //header
        EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);
        
        weaponData.weaponName = EditorGUILayout.TextField("Name", weaponData.weaponName);
        weaponData.damage = EditorGUILayout.FloatField("Damage", weaponData.damage);
        weaponData.range = EditorGUILayout.FloatField("Range", weaponData.range);
        
        
        //Draw default type enum field
        weaponData.fireType = (WeaponData.FireType)EditorGUILayout.EnumPopup("Fire Type", weaponData.fireType);

        EditorGUILayout.Space();

        weaponData.fireRate = EditorGUILayout.FloatField("Fire Rate in RPM", weaponData.fireRate);
        
        EditorGUILayout.LabelField("Ammo Properties", EditorStyles.boldLabel);
        
        weaponData.magSize =  EditorGUILayout.IntSlider("Magazine Size", weaponData.magSize, 1, 90);
        weaponData.maxCarryCapacity = EditorGUILayout.IntSlider("Max Carried Ammo", weaponData.maxCarryCapacity, weaponData.magSize, weaponData.magSize * 100);
        
        weaponData.reloadTime = EditorGUILayout.FloatField("Reload Time", weaponData.reloadTime);

        EditorGUILayout.LabelField("Amount of time(sec) it takes to equip weapon");
        weaponData.equipTime = EditorGUILayout.FloatField("Equip Time", weaponData.equipTime);
        EditorGUILayout.LabelField("Amount of time(sec) it takes to put away (holster) weapon");
        weaponData.holsterTime = EditorGUILayout.FloatField("Holster Time", weaponData.holsterTime);
        
        //Save Changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(weaponData);
        }
    }
}
