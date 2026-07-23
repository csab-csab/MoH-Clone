using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
  //Cached property index because string look-ups are expensive
  private static readonly int HolsterSpeed = Animator.StringToHash("Holster Speed");
  private static readonly int Holster = Animator.StringToHash("Holster");
  private static readonly int EquipSpeed = Animator.StringToHash("Equip Speed");
  private static readonly int Equip = Animator.StringToHash("Equip");

  [Header("Script References")]
  [Space(5)]
  [SerializeField] private InputHandler inputHandler;
  [SerializeField] private CanvasController canvasController;
  
  [Header("Reference to camera pivot transform; this is where we raycast")]
  [SerializeField]private Transform cameraTransform;

  /// <summary>
  /// Reference to currently running weapon switch coroutine.
  /// This can be used to cancel coroutine if switch is interrupted.
  /// </summary>
  private Coroutine _weaponSwitchRoutine;
  
  public enum Weapons
  {
    M1Garand,
    M1911
  };
  
  #region Weapon Switch Variables
  [Header("Weapon Switch Variables")]
  [SerializeField]Animator animator;
  private bool _isAllowedToSwitch = true;
  [SerializeField] private Weapons primarySlot;
  [SerializeField] private Weapons secondarySlot;
  [SerializeField] private Weapons currentlyEquipped;
  #endregion

  private Dictionary<Weapons, Weapon> _weaponRefs = new Dictionary<Weapons, Weapon>();
  
  // This "Property" acts as a shortcut to the actual script of the currently equipped weapon
  //It works by looking up the currently value of _weaponRefs[currentlyEquipped] and assigning the value of
  //activeWeaponScript to that
  //IMPORTANT!!! When updating currently equipped, the activeWeaponScript value gets automatically updated!
  private Weapon activeWeaponScript => _weaponRefs[currentlyEquipped];
  
  private void Awake()
  {
   IntialiseWeaponManager();
  }

  private void Start()
  {
    //Displays the name of the currently equipped weapon when starting the game
    canvasController.UpdateWeaponNameText(activeWeaponScript.ReturnWeaponName());
    canvasController.UpdateAmmmoText(activeWeaponScript.ReturnAmmo()._currentAmmo, 
      activeWeaponScript.ReturnAmmo()._carriedAmmo);
  }
  
  private void Update()
  {
    CheckReload();

    if (inputHandler.scrollInputNorm != Vector2.zero)
    {
      CallSwitchWeaponRoutine((int)inputHandler.scrollInputNorm.y);
    }
  }

  #region Intialisation

  private void IntialiseWeaponManager()
  {
    //Find all weapons that are children of this object
    //(true) includes objects that are currently disabled
    var childWeapons = GetComponentsInChildren<Weapon>(true);
    
    foreach(var w in childWeapons)
    {
      //Add a copy of the weapon to the dictionary if it doesn't exist
      _weaponRefs.TryAdd(w.weaponIdentity, w);
      
      //Also add canvas script reference
      if (canvasController != null)
      {
        w.AssignCanvasControllerReference(canvasController);
      }
    }
    
    //Subscribe to input events so input can trigger appropriate functions in this script
    if (activeWeaponScript.ReturnTypeOfFire() == WeaponData.FireType.Single)
    {
      inputHandler.OnFireTriggered += TriggerSingleShot;
    }

    inputHandler.OnReloadTriggered += TriggerReload;
    inputHandler.OnSwitchToPrimaryBtnPressed += CallSwitchWeaponRoutine;
    inputHandler.OnSwitchToSecondaryBtnPressed += CallSwitchWeaponRoutine;

    _isAllowedToSwitch = true;
  }

  #endregion
  
  /// <summary>
  /// Checks and triggers reload automatically if mag is empty and there is enough ammo to reload
  /// </summary>
  private void CheckReload()
  {
    if (activeWeaponScript.isEmpty && !activeWeaponScript.isOutOfAmmo)
    {
      TriggerReload();
    }
  }

  /// <summary>
  /// Switches weapon based on integer parameter. Positive val = Primary, Negative val = Secondary
  /// </summary>
  /// <param name="value"></param>
  private IEnumerator SwitchWeapon(int value)
  {
    if(!_isAllowedToSwitch || activeWeaponScript.IsReloading()) yield break; 
    
    _isAllowedToSwitch = false;
    
    //Unsub from fire event 
    inputHandler.OnFireTriggered -= TriggerSingleShot;

    //Get the weapon to equip next...
    //if value is positive, switch to primary else switch to secondary
    Weapons nextWeapon = value > 0 ? primarySlot : secondarySlot;

    //Get reference to next weapon to equip from dictionary, use out
    //to be able to access script and return equip time
    if(_weaponRefs.TryGetValue(nextWeapon, out Weapon equipNext))
    {
      //disable shooting during the switch
      activeWeaponScript.SetAllowedToShoot(false);
    
      float animSpeedMultiplier = activeWeaponScript.ReturnHolsterAnimDuration()/activeWeaponScript.ReturnHolsterTime();
      
      animator.SetFloat(HolsterSpeed, animSpeedMultiplier);
      animator.SetTrigger(Holster);
      
      //timer for swap delay
      //this is better than wait for seconds because we don't generate garbage
      float timer = 0f;
      while (timer < activeWeaponScript.ReturnHolsterTime())
      {
        timer += Time.deltaTime;
        yield return null;
      }

      activeWeaponScript.gameObject.SetActive(false);
      
      //Equip next weapon
      currentlyEquipped = nextWeapon;
    
      //if new weapon is single shot, sub to single-shot fire event
      if (activeWeaponScript.ReturnTypeOfFire() == WeaponData.FireType.Single)
      {
        inputHandler.OnFireTriggered += TriggerSingleShot;
      }

      //Update weapon ui such as weapon name and ammo
      canvasController.UpdateWeaponNameText(activeWeaponScript.ReturnWeaponName());
      canvasController.UpdateAmmmoText(activeWeaponScript.ReturnAmmo()._currentAmmo, 
        activeWeaponScript.ReturnAmmo()._carriedAmmo);
    
      activeWeaponScript.SetupOrSwapAnimator();
      activeWeaponScript.gameObject.SetActive(true);
      
      animSpeedMultiplier = equipNext.ReturnEquipAnimDuration()/equipNext.ReturnEquipTime();
      
      animator.SetFloat(EquipSpeed, animSpeedMultiplier);
      animator.SetTrigger(Equip);
      
      timer = 0;
      while (timer < equipNext.ReturnEquipTime())
      {
        timer += Time.deltaTime;
        yield return null;
      }
      
      //re-enable shooting
      activeWeaponScript.SetAllowedToShoot(true);
    }

    _isAllowedToSwitch = true;
    
    _weaponSwitchRoutine =  null;
  }
  
  #region Functions Triggered by events
  /// <summary>
  /// Triggers shoot function once on currently active weapon script
  /// </summary>
  private void TriggerSingleShot() 
  {
    activeWeaponScript.Shoot(cameraTransform);
  }

  /// <summary>
  /// Triggers Reload Function on currently active weapon script
  /// </summary>
  private void TriggerReload()
  {
    StartCoroutine(activeWeaponScript.Reload());
  }
  
  //Needs function so event can be called to due to delegate containing arguments
  /// <summary>
  /// Use this to trigger coroutine through the event
  /// </summary>
  /// <param name="v"></param>
  private void CallSwitchWeaponRoutine(int v)
  {
    //This prevents the user from switching to the same weapon
    if (v > 0 && currentlyEquipped == primarySlot ||
        v < 0 && currentlyEquipped == secondarySlot)
    {
      return;
    }
    
    _weaponSwitchRoutine = StartCoroutine(SwitchWeapon(v));
  }

  #endregion
  
}
