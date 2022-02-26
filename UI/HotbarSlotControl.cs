using UnityEngine;
using UnityEngine.UI;


namespace DLD
{

    public class HotbarSlotControl : MonoBehaviour
    {
        [SerializeField] GameObject highlight;
        [SerializeField] GameObject itemImage;


        public void Select()
        {
            highlight.SetActive(true);
        }


        public void Deselect()
        {
            highlight.SetActive(false);
        }


        public void Activate()
        {
            itemImage.SetActive(true);
        }


        public void Deactivate()
        {
            itemImage.SetActive(false);
        }


        public void Change(Sprite sprite)
        {
            itemImage.GetComponent<Image>().sprite = sprite;
        }
    }
}