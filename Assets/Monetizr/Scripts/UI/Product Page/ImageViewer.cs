using System;
using System.Collections;
using System.Collections.Generic;
using Monetizr.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
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
        public GridLayoutGroup contentLayout;

        private List<Image> _dots = new List<Image>();
        public Color DotActiveColor;
        public Color DotInactiveColor;

        public RectTransform ScrollView;
        public float ViewYPivotVertical = 0.5f;
        public float ViewYPivotHorizontal = 0.4f;
        public float ViewScaleVertical = 1f;
        public float ViewScaleHorizontal = 1.4f;
        public GameObject[] VerticalButtons;
        public GameObject[] HorizontalButtons;

        public ScrollSnap[] syncScrollSnaps;

        public bool cropImages = false;

        private void Start()
        {
            ScrollSnap.onRelease.AddListener(ChangeDot);
            ScrollSnap.onLerpComplete.AddListener(UpdateSynced);
            ui.ScreenOrientationChanged += UpdateLayout;
            ui.ScreenResolutionChanged += UpdateCellSize;
            UpdateLayout(Utility.UIUtility.IsPortrait());
        }

        private void OnDestroy()
        {
            ui.ScreenOrientationChanged -= UpdateLayout;
            ui.ScreenResolutionChanged -= UpdateCellSize;
        }

        public bool IsOpen()
        {
            if (ViewerCanvasGroup == null) return true;
            return ViewerCanvasGroup.alpha >= 0.01f;
        }

        public bool IsPermanent()
        {
            return ViewerCanvasGroup == null;
        }

        public void JumpToFirstImage()
        {
            ScrollSnap.SnapToIndex(0);
        }

        public void PreviousImage()
        {
            ScrollSnap.SnapToPrev();
        }

        public void NextImage()
        {
            ScrollSnap.SnapToNext();
        }

        public void UpdateLayout(bool portrait)
        {
            var newPivot = ScrollView.pivot;
            newPivot.y = portrait ? ViewYPivotVertical : ViewYPivotHorizontal;
            ScrollView.pivot = newPivot;
            ScrollView.localScale
                = Vector3.one * (portrait ? ViewScaleVertical : ViewScaleHorizontal);

            foreach (var go in VerticalButtons)
                go.SetActive(portrait);

            foreach (var go in HorizontalButtons)
                go.SetActive(!portrait);
        }

        public void UpdateCellSize()
        {
            //This is now also applicable to the fullscreen viewer
            //if (ImageViewerAnimator != null) return;
            
            Canvas.ForceUpdateCanvases();
            contentLayout.cellSize = new Vector2(ScrollView.rect.width, ScrollView.rect.height);
            Canvas.ForceUpdateCanvases();
            ScrollSnap.RedoAwake();
        }

        public void ChangeDot(int to)
        {
            //int to = ScrollSnap.CurrentIndex;
            if (to >= _dots.Count) return; //Not enough dots, shouldn't happen.

            int i = 0;
            foreach(var d in _dots)
            {
                d.color = (i == to) ? DotActiveColor : DotInactiveColor;
                i++;
            }
        }

        public int DotCount()
        {
            return _dots.Count;
        }

        public void UpdateSynced()
        {
            int idx = ScrollSnap.CurrentIndex;
            
            foreach(var s in syncScrollSnaps)
                s.MoveToIndex(idx);
        }

        public void AddImage(Sprite spr, bool root = false)
        {
            if (spr == null) return;

            GameObject newImage = RootImage;
            if(!root)
                newImage = Instantiate(RootImage, ContentTransform.transform, false);
            var img = newImage.GetComponent<Image>();
            img.sprite = cropImages ? UIUtility.CropSpriteToSquare(spr) : spr;
            if(!root)
                ScrollSnap.PushLayoutElement(newImage.GetComponent<LayoutElement>());

            //Add a new dot for indicators
            var newDot = Instantiate(DotPrefab, DotContainer, false);
            newDot.SetActive(true);
            var dim = newDot.GetComponent<Image>();
            dim.color = DotInactiveColor;
            _dots.Add(dim);
            ChangeDot(ScrollSnap.CurrentIndex);
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
            if (ImageViewerAnimator == null) return;
            ImageViewerAnimator.SetBool("Opened", false);
            ui.ProductPage.ShowMainLayout();
        }

        public void ShowViewer()
        {
            UpdateCellSize();
            if (ImageViewerAnimator == null) return;
            ImageViewerAnimator.SetBool("Opened", true);
            ui.ProductPage.HideMainLayout();
            //ChangeDot(ScrollSnap.CurrentIndex);
        }
    }
}