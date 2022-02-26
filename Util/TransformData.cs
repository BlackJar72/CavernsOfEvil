using UnityEngine;


namespace DLD
{


    [System.Serializable]
    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;


        public TransformData(Transform transform)
        {
            position = transform.localPosition;
            rotation = transform.localRotation;
            scale = transform.localScale;          
        }


        public Transform SetData(ref Transform transform)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;
            return transform;
        }


        public Transform AddData(ref Transform transform)
        {
            transform.localPosition += position;
            transform.localRotation *= rotation;
            return transform;
        }

    }

}