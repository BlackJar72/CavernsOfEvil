using UnityEngine;
using TMPro;


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


        public void Toast(string message)
        {
            toast.SetActive(visible = true);
            toastMessage.text = message;
            timeout = Time.time + 3f;
        }


        public void ClearToast()
        {
            toastMessage.text = "";
            toast.SetActive(visible = false);
        }
    }

}