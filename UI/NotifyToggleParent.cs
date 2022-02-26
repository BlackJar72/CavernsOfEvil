using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyToggleParent : MonoBehaviour
{
    public Toggle parent;

    private ToggleImageSelect parentScript;

    public void Start()
    {
        parentScript = GetComponent<ToggleImageSelect>();
    }

    public void OnToggleClicked()
    {
        parentScript.OnToggleClicked();
    }
}
