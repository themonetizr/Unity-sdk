using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Monetizr.UI.Theming;
using Monetizr.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
	public class SelectionManagerBigScreen : MonoBehaviour, IThemable {
        public MonetizrUI ui;
        public List<SelectorOptionBigScreen> options;
        private SelectorOptionBigScreen _selectedOption;
        private SelectorOptionBigScreen _currentSelectedOption;
        public SelectorOptionBigScreen SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                if (_waitingForNext) return; //Avoid bugs from spamming selections.
                _selectedOption = value;
                foreach (var option in options)
                {
                    if (option.gameObject.GetInstanceID() != _selectedOption.gameObject.GetInstanceID())
                    {
                        if(option.optionNameText.color != _fontDisabledColor)
                            option.optionNameText.color = _fontDeselectedColor;
                    } 
                }
                _selectedOption.optionNameText.color = _fontSelectedColor;
                var dd = ui.ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
                dd.OptionText.text = _selectedOption.optionNameText.text;
                dd.SelectedOption = _selectedOption.optionNameText.text;
                ui.ProductPage.UpdateVariant();
                Canvas.ForceUpdateCanvases();
                dd.BigScreenAlternate.GetComponent<HorizontalLayoutGroup>().enabled = false;
                dd.BigScreenAlternate.GetComponent<HorizontalLayoutGroup>().enabled = true;
                StartCoroutine(SelectNextEnumerator());
            }
        }

        private bool _waitingForNext = false;
        public BigScreenSelectorAnimator animator;
        
        public void AnimateToNext()
        {
            StartCoroutine(SelectNextEnumerator(0f));
        }
        private IEnumerator SelectNextEnumerator(float delay = 0.2f)
        {
            if (_waitingForNext) yield break;
            _waitingForNext = true;
            //yield return new WaitForSeconds(delay);
            animator.ExitToNext(_selectedOption, GetNextX(), 0.4f);
            yield return null; //Wait a few frames because execution order
            yield return null; //can be unpredictable
            while (animator.Animating)
                yield return null;

            //yield return new WaitForSeconds(0.13f);
            NextSelect();
        }
        
        public void AnimateToPrevious()
        {
            StartCoroutine(SelectPreviousEnumerator(0f));
        }
        private IEnumerator SelectPreviousEnumerator(float delay = 0.2f)
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            var nextDd = _allDropdowns.ElementAtOrDefault(current - 1);
            if (!nextDd || nextDd == null)
            {
                //Messy, but we need to check if we can go back before fading out.
                yield break;
            }

            if (_waitingForNext) yield break;
            _waitingForNext = true;
            
            //yield return new WaitForSeconds(delay);
            animator.ExitToNext(_selectedOption, GetPreviousX(), 0.4f);
            yield return null; //Wait a few frames because execution order
            yield return null; //can be unpredictable
            while (animator.Animating)
                yield return null;
            //yield return new WaitForSeconds(0.13f);
            PreviousSelect();
        }
        
        public GameObject selectionPanel;
        public RectTransform selectionListRect;
        public CanvasGroup selectionCanvasGroup;
        private Color _fontDisabledColor;
        private Color _fontSelectedColor;
        private Color _fontDeselectedColor;

        string _optionName;
        private VariantsDropdown _currentDropdown;
        private List<VariantsDropdown> _allDropdowns;

        private void Start()
        {
        }

        private void OnDestroy()
        {
        }

        public bool IsOpen()
        {
            return selectionCanvasGroup.blocksRaycasts;
        }

        public void Apply(ColorScheme scheme)
        {
            _fontDisabledColor = scheme.GetColorForType(ColorScheme.ColorType.Disabled);
            _fontDeselectedColor = scheme.GetColorForType(ColorScheme.ColorType.PrimaryText);
            _fontSelectedColor = scheme.GetColorForType(ColorScheme.ColorType.Acccent);
        }

        private float GetXForPosition(int idx)
        {
            var dd = _allDropdowns.ElementAtOrDefault(idx);
            if (!dd || dd == null) return selectionListRect.anchoredPosition.x;

            var dropdownRect = dd.BigScreenAlternate.GetComponent<RectTransform>();
            var newPos = UIUtility.SwitchToRectTransform(dropdownRect, selectionListRect);
            //newPos.y += dropdownRect.rect.height / 2f;
            newPos.x -= 7.5f;
            return newPos.x;
        }

        private float GetNextX()
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            return GetXForPosition(current + 1);
        }
        
        private float GetPreviousX()
        {
            int current = _allDropdowns.IndexOf(_currentDropdown);
            return GetXForPosition(current - 1);
        }
        
        private void UpdatePosition()
        {
            var dropdownRect = _currentDropdown.BigScreenAlternate.GetComponent<RectTransform>();
            var newPos = UIUtility.SwitchToRectTransform(dropdownRect, selectionListRect);
            newPos.y += dropdownRect.rect.height / 2f;
            newPos.x -= 7.5f;
            selectionListRect.anchoredPosition = newPos;
        }

        public void InitOptions(List<string> variants, string optionName, VariantsDropdown currentDropdown, List<VariantsDropdown> allDropdowns)
        {
            if (animator.Animating) return;
            int i = 0;
            _optionName = optionName;
            _currentDropdown = currentDropdown;
            _allDropdowns = allDropdowns;
            _waitingForNext = false;
            for (int j=0;j<options.Count;j++)
            {
                options[j].gameObject.SetActive(j < variants.Count);
            }

            Canvas.ForceUpdateCanvases(); //Necessary for getting correct position for SelectionBar

            foreach (var variant in variants)
            {
                var option = options[i];
                var button = option.GetComponent<Button>();
                option.optionNameText.text = variant;
                if (currentDropdown.SelectedOption == variant)
                {
                    option.optionNameText.color = _fontSelectedColor;
                    ui.SelectWhenInteractable(button);
                    _currentSelectedOption = option;
                }
                else
                {
                    option.optionNameText.color = _fontDeselectedColor;
                }
                
                //Check if variant chain can continue from here
                var variantDictionary = currentDropdown.GetVariantBreadcrumbs(new Dictionary<string, string>());
                variantDictionary[optionName] = variant;

                var allVariantList = ui.ProductPage.product.GetAllVariantsForOptions(variantDictionary);

                if (allVariantList == null)
                {
                    option.optionNameText.color = _fontDisabledColor;
                    option.priceText.text = "";
                    button.interactable = false;
                }
                else
                {
                    option.priceText.text = Product.GetFormattedPriceRangeForVariants(allVariantList);
                    button.interactable = true;
                }

                var nextOption = options.ElementAtOrDefault(i + 1);
                var previousOption = options.ElementAtOrDefault(i - 1);

                var curNav = button.navigation;
                curNav.selectOnDown = nextOption == null ? null : nextOption.GetComponent<Button>();
                curNav.selectOnUp = previousOption == null ? null : previousOption.GetComponent<Button>();
                button.navigation = curNav;
                
                i++;
            }
            
            UpdatePosition();
            animator.Entry(_currentSelectedOption);
        }

        public void ShowSelection()
        {
            //selectionPanel.SetActive(true);
            selectionCanvasGroup.alpha = 1f;
            selectionCanvasGroup.interactable = true;
            selectionCanvasGroup.blocksRaycasts = true;
        }

        public void HideSelection(bool blockOnWaitingForNext = false)
        {
            HideSelection(blockOnWaitingForNext, true);
        }

        public void HideSelection(bool blockOnWaitingForNext, bool updateVariant = true)
        {
            if (animator.Animating) return;
            if (_waitingForNext && blockOnWaitingForNext) return;
            //selectionPanel.SetActive(false);
            //selectionCanvasGroup.alpha = 0f;
            animator.Exit(_currentSelectedOption);
            //selectionCanvasGroup.interactable = false;
            selectionCanvasGroup.blocksRaycasts = false;
            Button ddButton = null;
            try
            {
                ddButton = _currentDropdown.BigScreenAlternate.GetComponent<Button>();
            }
            catch
            {
                //Debug.LogWarning("Tried to get big screen dropdown, but _currentDropdown was not assigned!");
            } 
            ui.SelectWhenInteractable(ddButton);
            if(updateVariant)
                ui.ProductPage.UpdateVariant();
        }

        public bool HideSelectionCurrentTest(VariantsDropdown from)
        {
            if (from == _currentDropdown)
            {
                HideSelection(true);
                return true;
            }

            return false;
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
