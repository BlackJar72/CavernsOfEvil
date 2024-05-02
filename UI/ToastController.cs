using System;
using TMPro;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class ToastController : MonoBehaviour
    {
        [SerializeField] GameObject toast;
        [SerializeField] TMP_Text toastMessage;

        private bool visible;
        private float timeout;


        // Update is called once per frame
        void Update()
        {
            if(visible && Time.time > timeout)
            {
                ClearToast();
            }
        }


        [Obsolete("Use ToastLocalized(tableIS, key) instead.")]
        public void Toast(string message)
        {
            toast.SetActive(visible = true);
            toastMessage.text = message;
            timeout = Time.time + 3f;
        }


        public void ToastLocalized(string tableId, string key)
        {
            toast.SetActive(visible = true);
            toastMessage.text = LocalizationManager.GetTranslation(tableId, key);
            timeout = Time.time + 3f;
        }


        public void ToastLocalized(string tableId, string key, string appended)
        {
            toast.SetActive(visible = true);
            toastMessage.text = LocalizationManager.GetTranslation(tableId, key) + " " + appended;
            timeout = Time.time + 3f;
        }


        public void ClearToast()
        {
            toastMessage.text = "";
            toast.SetActive(visible = false);
        }
    }

}