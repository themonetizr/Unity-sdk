using UnityEngine;
using System.Reflection;

namespace MagneticScrollView
{
    /// <summary>
    /// MonoBehaviour class used to check whether the object dimensions has been changed.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    internal class ChangeCheckCallback : MonoBehaviour
    {
        //[HideInInspector]
        private MagneticScrollRect magneticScrollView;

        void OnEnable ()
        {
            magneticScrollView = GetComponentInParent<MagneticScrollRect> ();
            hideFlags = HideFlags.HideInInspector;
        }

        private void OnRectTransformDimensionsChange ()
        {
            // Debug.Log ("Dimension Changed", gameObject);
            
            if (gameObject.activeInHierarchy && enabled && magneticScrollView != null)            
                magneticScrollView.ArrangeElements ();
        }
    }
}
