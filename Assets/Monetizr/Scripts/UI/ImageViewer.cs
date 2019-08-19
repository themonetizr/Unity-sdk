using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class ImageViewer : MonoBehaviour
    {
        public ProductPageScript ProductPageScript;
        public GameObject ViewerObject;
        public GameObject RootImage;
        public Transform ContentTransform;
        public ScrollSnap ScrollSnap;
        public GameObject DotPrefab;
        public Transform DotContainer;

        private List<Image> _dots = new List<Image>();
        public Color DotActiveColor;
        public Color DotInactiveColor;

        private void Start()
        {
            ScrollSnap.onLerpComplete.AddListener(() => ChangeDot());
        }

        public void ChangeDot()
        {
            int to = ScrollSnap.CurrentIndex;
            if (to >= _dots.Count) return; //Not enough dots, shouldn't happen.

            int i = 0;
            foreach(var d in _dots)
            {
                d.color = (i == to) ? DotActiveColor : DotInactiveColor;
                i++;
            }
        }

        public void AddImage(Sprite spr, bool root = false)
        {
            if (spr == null) return;

            GameObject newImage = RootImage;
            if(!root)
                newImage = Instantiate(RootImage, ContentTransform.transform, false);
            var img = newImage.GetComponent<Image>();
            img.sprite = spr;
            /*if(!root)
                ScrollSnap.PushLayoutElement(newImage.GetComponent<LayoutElement>());*/

            //Add a new dot for indicators
            var newDot = Instantiate(DotPrefab, DotContainer, false);
            newDot.SetActive(true);
            var dim = newDot.GetComponent<Image>();
            dim.color = DotInactiveColor;
            _dots.Add(dim);
        }

        public void CloseViewer()
        {
            ViewerObject.SetActive(false);
            ProductPageScript.ShowMainLayout();
        }

        public void OpenViewer()
        {
            ViewerObject.SetActive(true);
            ProductPageScript.HideMainLayout();
            ScrollSnap.MoveToIndex(0);
            ChangeDot();
        }
    }
}