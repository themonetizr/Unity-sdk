using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
{
    public class ImageViewer : MonoBehaviour
    {
        public MonetizrUI ui;
        public Animator ImageViewerAnimator;
        public CanvasGroup ViewerCanvasGroup;
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

        public bool IsOpen()
        {
            return ViewerCanvasGroup.alpha >= 0.01f;
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
            if(!root)
                ScrollSnap.PushLayoutElement(newImage.GetComponent<LayoutElement>());

            //Add a new dot for indicators
            var newDot = Instantiate(DotPrefab, DotContainer, false);
            newDot.SetActive(true);
            var dim = newDot.GetComponent<Image>();
            dim.color = DotInactiveColor;
            _dots.Add(dim);
        }

        public void RemoveImages()
        {
            Transform[] imgs = ContentTransform.GetComponentsInChildren<Transform>();
            foreach(var i in imgs)
            {
                if (i == ContentTransform) continue;
                GameObject go = i.gameObject;
                if(go != RootImage)
                {
                    Destroy(go);
                }
            }

            Image[] dots = _dots.ToArray();
            foreach (var i in dots)
            {
                GameObject go = i.gameObject;
                Destroy(go);
            }
            _dots.Clear();
        }

        public void HideViewer()
        {
            ImageViewerAnimator.SetBool("Opened", false);
            ui.ProductPage.ShowMainLayout();
        }

        public void ShowViewer()
        {
            ImageViewerAnimator.SetBool("Opened", true);
            ui.ProductPage.HideMainLayout();
            ScrollSnap.MoveToIndex(0);
            ChangeDot();
        }
    }
}