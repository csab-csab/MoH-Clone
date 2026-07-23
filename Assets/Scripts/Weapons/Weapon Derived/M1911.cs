using UnityEngine;
using System.Collections;

public class M1911 : Weapon
{
    private void Awake()
    {
        this.weaponIdentity = WeaponManager.Weapons.M1911;
    }
    
    public override void Shoot(Transform raycastFrom)
    {
        if (CurrentAmmo <= 0)
        {
            return;
        }
        
        base.Shoot(raycastFrom);

        base.EjectShellCasing(raycastFrom);
        
        if (CurrentAmmo > 0)
        {
            return;
        }
    }

    public override IEnumerator Reload()
    {
        StartCoroutine(base.Reload());
        yield return null;
    }
}
