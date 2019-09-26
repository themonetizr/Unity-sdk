﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class SelectionManager : MonoBehaviour
    {
        public MonetizrUI ui;
        public List<SelectorOption> Options;
        public SelectorOption SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                if (_waitingForNext) return; //Avoid bugs from spamming selections.
                _waitingForNext = true;
                _selectedOption = value;
                foreach (var option in Options)
                {
                    if (option.gameObject.GetInstanceID() != _selectedOption.gameObject.GetInstanceID())
                    {
                        if(option.OptionNameText.color != FontDisabledColor)
                            option.OptionNameText.color = FontDeselectedColor;
                        option.SetEmphasisLines(false);
                    } 
                }
                _selectedOption.OptionNameText.color = FontSelectedColor;
                _selectedOption.SetEmphasisLines(true);
                //SelectionBar.anchoredPosition = Utility.UIUtilityScript.SwitchToRectTransform(_selectedOption.GetComponent<RectTransform>(), SelectionListLayout);
                var dd = ui.ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
                dd.OptionText.text = _selectedOption.OptionNameText.text;
                dd.SelectedOption = _selectedOption.OptionNameText.text;

                ui.ProductPage.UpdateVariant();

                StartCoroutine(SelectNextEnumerator());
            }
        }

        private bool _waitingForNext = false;

        private IEnumerator SelectNextEnumerator()
        {
            _waitingForNext = true;
            yield return new WaitForSeconds(0.2f);
            FaderAnimator.SetBool("Faded", true);
            yield return new WaitForSeconds(0.1f);
            NextSelect();
        }
        
        private IEnumerator SelectPreviousEnumerator()
        {
            _waitingForNext = true;
            yield return new WaitForSeconds(0.2f);
            FaderAnimator.SetBool("Faded", true);
            yield return new WaitForSeconds(0.1f);
            PreviousSelect();
        }

        public Text OptionText;
        public GameObject SelectionPanel;
        public RectTransform SelectionListLayout;
        public Animator FaderAnimator;
        public Animator SelectorAnimator;
        public CanvasGroup SelectionCanvasGroup;
        public Color FontDisabledColor;
        public Color FontSelectedColor;
        public Color FontDeselectedColor;
        public LayoutElement Header;
        public Text breadcrumbsText;
        public GameObject backButton;

        public float VerticalSelectionHeight = 100;
        public float HorizontalSelectionHeight = 120;
        public GameObject VerticalCloseButton;
        public GameObject HorizontalCloseButton;
        public float VerticalHeaderHeight = 100;
        public float HorizontalHeaderHeight = 140;

        private SelectorOption _selectedOption;
        string _optionName;
        private VariantsDropdown _currentDropdown;
        private List<VariantsDropdown> _allDropdowns;

        private void Start()
        {
            ui.ScreenOrientationChanged += UpdateLayout;
            UpdateLayout(Utility.UIUtility.IsPortrait());
        }

        private void OnDestroy()
        {
            ui.ScreenOrientationChanged -= UpdateLayout;
        }

        public bool IsOpen()
        {
            return SelectionCanvasGroup.alpha >= 0.01f;
        }

        public void UpdateLayout(bool portrait)
        {
            foreach(var o in Options)
            {
                o.GetComponent<LayoutElement>().minHeight
                    = portrait ? VerticalSelectionHeight : HorizontalSelectionHeight;
            }

            VerticalCloseButton.SetActive(portrait);
            HorizontalCloseButton.SetActive(!portrait);
            
            Header.minHeight = portrait ? VerticalHeaderHeight : HorizontalHeaderHeight;
        }

        public void InitOptions(List<string> variants, string optionName, VariantsDropdown currentDropdown, List<VariantsDropdown> allDropdowns)
        {
            int i = 0;
            string on = optionName.Replace("Select ", "").Replace('\n', ' ');
            OptionText.text = ("Select " + on + ":").ToUpper();
            _optionName = optionName;
            _currentDropdown = currentDropdown;
            _allDropdowns = allDropdowns;
            FaderAnimator.SetBool("Faded", false);
            _waitingForNext = false;
            for (int j=0;j<Options.Count;j++)
            {
                Options[j].gameObject.SetActive(j < variants.Count);
            }

            breadcrumbsText.text = currentDropdown.GetBreadcrumbs("");
            backButton.SetActive(currentDropdown.previous != null);
            Canvas.ForceUpdateCanvases(); //Necessary for getting correct position for SelectionBar

            foreach (var variant in variants)
            {
                var option = Options[i];
                option.OptionNameText.text = variant;
                option.isActive = true;
                if (currentDropdown.SelectedOption == variant)
                {
                    option.OptionNameText.color = FontSelectedColor;
                    option.SetEmphasisLines(true);
                }
                else
                {
                    option.OptionNameText.color = FontDeselectedColor;
                    option.SetEmphasisLines(false);
                }
                
                //Check if such variant exists
                var currentSelection = new Dictionary<string, string>();

                foreach (var d in allDropdowns)
                {
                    if (!string.IsNullOrEmpty(d.OptionName))
                        currentSelection[d.OptionName] = d.SelectedOption;
                }

                currentSelection[optionName] = variant;
                var selectedVariant = ui.ProductPage.product.GetVariant(currentSelection);

                if (selectedVariant == null)
                {
                    option.isActive = false;
                    option.OptionNameText.color = FontDisabledColor;
                    option.priceText.text = "Unavailable";
                }
                else
                {
                    option.priceText.text = selectedVariant.Price.FormattedPrice;
                }
                
                i++;
            }
        }

        public void ShowSelection()
        {
            SelectorAnimator.SetBool("Opened", true);
            ui.ProductPage.HideMainLayout();
        }

        public void HideSelection()
        {
            SelectorAnimator.SetBool("Opened", false);
            ui.ProductPage.ShowMainLayout();
        }

        public void NextSelect()
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            var nextDd = _allDropdowns.ElementAtOrDefault(current + 1);
            if (!nextDd || nextDd == null)
            {
                //Never shall ever anyone delete this line to preserve its original glory (Rudolfs)
                //transform.parent.transform.parent.gameObject.SetActive(false);
                HideSelection();
                return;
            }

            if(nextDd.Options.Count == 0)
            {
                HideSelection();
                return;
            }

            InitOptions(nextDd.Options, nextDd.OptionName, nextDd, _allDropdowns);
        }

        public void PreviousSelect()
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            var nextDd = _allDropdowns.ElementAtOrDefault(current - 1);
            if (!nextDd || nextDd == null)
            {
                //Never shall ever anyone delete this line to preserve its original glory (Rudolfs)
                //transform.parent.transform.parent.gameObject.SetActive(false);
                //HideSelection();
                return;
            }

            InitOptions(nextDd.Options, nextDd.OptionName, nextDd, _allDropdowns);
        }
    }
}