using SmartifyOS.SaveSystem;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class SystemSettingsPage : BaseSettingsPage
    {
        [SerializeField] private Button resetSaveDataButton;
        
        private void Awake()
        {
            resetSaveDataButton.onClick += () => 
            { 

                ModalWindow.Create().Init("Are you sure", "Are you sure you want to reset all your data?", ModalWindow.ModalType.YesNo, () => SaveManager.Remove(), () =>{});
            };
        }
    }

}