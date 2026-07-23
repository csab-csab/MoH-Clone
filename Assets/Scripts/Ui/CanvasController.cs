using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
  [Header("Text References")]
  [Space(10)]
  [Header("Weapon UI References")]
  [SerializeField]private TMP_Text weaponName;
  [SerializeField] private TMP_Text ammoText;


  public void UpdateWeaponNameText(string newName)
  {
    weaponName.text = newName; 
  }

  public void DisplayReloadText()
  {
    ammoText.text = "Reloading...";
  }
  
  public void UpdateAmmmoText(int currentAmmmo, int carriedAmmmo)
  {
    if (currentAmmmo <= 0)
    {
      ammoText.text = "OutOfAmmo!";
      return;
    }
    ammoText.text = $"{currentAmmmo}/{carriedAmmmo}";
  }
  
}
