using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class ImageViewer : MonoBehaviour
    {
        public GameObject ViewerObject;
        public GameObject RootImage;
        public Transform ContentTransform;
        public ScrollSnap ScrollSnap;

        public void AddImage(Sprite spr, bool root = false)
        {
            GameObject newImage = RootImage;
            if(!root)
                newImage = Instantiate(RootImage, ContentTransform.transform, false);
            var img = newImage.GetComponent<Image>();
            img.sprite = spr;
            /*if(!root)
                ScrollSnap.PushLayoutElement(newImage.GetComponent<LayoutElement>());*/
        }

        public void CloseViewer()
        {
            ViewerObject.SetActive(false);
        }

        public void OpenViewer()
        {
            ViewerObject.SetActive(true);
            ScrollSnap.SnapToIndex(0);
        }
    }
}