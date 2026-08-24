using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
  [Header("Script References")]
  [SerializeField]private PlayerInteract playerInteract;
  [SerializeField]private InputHandler inputHandler;
  [SerializeField]private InputDeviceTracker inputDeviceTracker;
  private InputDeviceTracker.CurrentInputDevice _currentInputDevice => inputDeviceTracker.ReturnCurrentInputDevice();
  
  #region Glyphs
  [Header("Glyphs References")]
  [SerializeField]private TMP_Asset mkSpriteAtlas;

  [System.Serializable]
  struct BindGlyphDictionary
  {
    [SerializeField]private string _bindName;
    [SerializeField]private int _atlasIndex;
    
    // Public getters to read the private serialized data
    public string bindName => _bindName;
    public int atlasIndex => _atlasIndex;
  }
  
  [SerializeField]private BindGlyphDictionary[] bindGlyphDictionary;
  
  //dictionary reference
  private Dictionary<string, int> _bindGlyphLookupDictionary = new Dictionary<string, int>();
  
  #endregion
  
  
  [Header("Text References")]
  [Space(10)]
  [Header("Weapon UI References")]
  [SerializeField]private Image crosshair;
  [SerializeField]private TMP_Text weaponName;
  [SerializeField] private TMP_Text ammoText;
  
  [Header("Crosshair References")]
  [SerializeField]private Color standardColor;
  [SerializeField]private Color enemyColor;
  
  [Header("Interact UI References")]
  [SerializeField]private TextMeshProUGUI interactText;

 

  
  private void Start()
  {
    //Add each element from inspector assigned array to
    //dictionary
    foreach (var bindGlyphEntry in bindGlyphDictionary)
    {
      _bindGlyphLookupDictionary.TryAdd(bindGlyphEntry.bindName, bindGlyphEntry.atlasIndex);
    }
  }

  void Update()
  {
    UpdateCrosshair(playerInteract.ReturnIsLookingAtEnemy());
  }

  private void UpdateCrosshair(bool isLookingAtEnemy)
  {
    crosshair.color = isLookingAtEnemy ? enemyColor : standardColor;
  }

  public void UpdateWeaponNameText(string newName)
  {
    weaponName.text = newName; 
  }

  public void DisplayReloadText()
  {
    ammoText.color = Color.white;
    ammoText.text = "Reloading...";
  }
  
  public void UpdateAmmoText(int currentAmmo, int carriedAmmo)
  {
    if (currentAmmo <= 0)
    {
      ammoText.text = "Out Of Ammo!";
      ammoText.color = Color.red;
      return;
    }

    if (ammoText.color == Color.red)
    {
      ammoText.color = Color.white;
    }
    ammoText.text = $"{currentAmmo}/{carriedAmmo}";
  }

  /// <summary>
  /// Used for showing interact prompts, includes embedded button glyphs
  /// </summary>
  /// <param name="inputActionMapName"> Input action map declared input action asset ("eg: player, weapon)</param>
  /// <param name="promptVerb"> Press or hold</param>
  /// <param name="actionName"> Interact</param>
  /// <param name="displayActionName">Specific action used for displaying to player,
  ///   such as open when looking at door</param>
  /// <param name="pressed"> Replaces glyph with pressed down glyph</param>
  public void ShowPrompt(string inputActionMapName, string promptVerb, 
    string actionName, string displayActionName = null, bool pressed = false)
  {
    InputActionMap inputActionMap =  inputHandler.inputActionAsset.FindActionMap(inputActionMapName);

    if (inputActionMap == null)
    {
      Debug.LogError($"{inputActionMapName} not found, check you have checked map name correctly.");
      return;
    }
    
    InputAction currentAction =  inputActionMap.FindAction(actionName);
    
    if (currentAction == null)
    {
      Debug.LogError($"No current action found for {inputActionMapName}");
      return;
    }

    string bindName = "";
    string spriteTag = "";
    string promptText = "";
    
    switch (_currentInputDevice)
    {
      case InputDeviceTracker.CurrentInputDevice.Keyboard:
        bindName = currentAction.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions,
          group: "Keyboard&Mouse");
        break;
      
      case InputDeviceTracker.CurrentInputDevice.XboxPad:
        bindName = currentAction.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions,
          group: "Gamepad") + "Xbox";
        break;
      
      case InputDeviceTracker.CurrentInputDevice.PsPad:
        bindName = currentAction.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions
          ,group: "Gamepad") + "PS";
        break;
    }
    
    if (_bindGlyphLookupDictionary.TryGetValue(bindName, out var spriteIndex))
    {
      if (pressed)
      {
        spriteIndex++;
      }
      spriteTag = $"<voffset=0.2em><space=0.5em><size=120%><sprite={spriteIndex}></size><space=-0.5em></voffset>";
    }

    string finalAction = string.IsNullOrWhiteSpace(displayActionName) ? actionName : displayActionName;
    promptText = $"{promptVerb} {spriteTag} to {finalAction}";
    
    interactText.text = promptText;
  }

  public void ClearPromptText()
  {
    interactText.text = string.Empty;
  }
}
