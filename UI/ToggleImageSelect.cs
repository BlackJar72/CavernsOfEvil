using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImageSelect : MonoBehaviour
{
    public Image on, off;

    private Toggle toggle;

    public void Start()
    {
        toggle = GetComponent<Toggle>();
    }


    public void OnToggleClicked()
    {
        on.gameObject.SetActive(toggle.isOn);
        off.gameObject.SetActive(!toggle.isOn);
    }
}
