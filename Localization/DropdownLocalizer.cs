using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;


namespace CevarnsOfEvil {

    public class DropdownLocalizer : MonoBehaviour {
        [SerializeField] TMP_Dropdown dropdown;
        [SerializeField] string tableId;
        [SerializeField] string[] keys;


        void Start() {
            LocalizeOptions();
        }


        //FIXME: This is wildly inefficient, but I know of no other way
        //FIXME: -- if there is an event I could listen for to call this, I'd sure like to know!
        //(No, none of the events I can find fit this -- they all seem to be for localizing one thing based on one key.)
        void Update() {
            LocalizeOptions();
        }


        public void LocalizeOptions() {//*
            //Debug.Log("LocalizeOptions() was called.");
            int num = dropdown.options.Count;
        #if UNITY_EDITOR
            if(keys.Length != num) Debug.Log("WARNING: The number of keys does not match the number of options! " +
                "Num Keys = " + keys.Length + "; Num Options = " + num);
        #endif
            num = Mathf.Min(num, keys.Length);
            for(int i = 0; i < num; i++) {
                dropdown.options[i].text = LocalizationManager.GetTranslation(tableId, keys[i]);
            }
            dropdown.RefreshShownValue();
        //*/
        }
    }

}
