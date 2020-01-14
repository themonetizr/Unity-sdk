using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Monetizr.UI.Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr.UI
{
    public class SelectionManager : MonoBehaviour, IThemable
    {
        public MonetizrUI ui;
        public List<SelectorOption> Options;
        public SelectorOption SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                if (_waitingForNext) return; //Avoid bugs from spamming selections.
                _selectedOption = value;
                foreach (var option in Options)
                {
                    if (option.gameObject.GetInstanceID() != _selectedOption.gameObject.GetInstanceID())
                    {
                        if(option.OptionNameText.color != _fontDisabledColor)
                            option.OptionNameText.color = _fontDeselectedColor;
                        option.SetEmphasisLines(false);
                    } 
                }
                _selectedOption.OptionNameText.color = _fontSelectedColor;
                _selectedOption.SetEmphasisLines(true);
                //SelectionBar.anchoredPosition = Utility.UIUtilityScript.SwitchToRectTransform(_selectedOption.GetComponent<RectTransform>(), SelectionListLayout);
                var dd = ui.ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
                dd.OptionText.text = _selectedOption.OptionNameText.text;
                dd.SelectedOption = _selectedOption.OptionNameText.text;
                Canvas.ForceUpdateCanvases();
                StartCoroutine(SelectNextEnumerator());
            }
        }

        private bool _waitingForNext = false;

        public void AnimateToNext()
        {
            StartCoroutine(SelectNextEnumerator(0f));
        }
        private IEnumerator SelectNextEnumerator(float delay = 0.2f)
        {
            if (_waitingForNext) yield break;
            _waitingForNext = true;
            yield return new WaitForSecondsRealtime(delay);
            FaderAnimator.SetBool("Faded", true);
            yield return new WaitForSecondsRealtime(0.13f);
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
            
            yield return new WaitForSecondsRealtime(delay);
            FaderAnimator.SetBool("Faded", true);
            yield return new WaitForSecondsRealtime(0.13f);
            PreviousSelect();
        }

        public Text OptionText;
        public GameObject SelectionPanel;
        public RectTransform SelectionListLayout;
        public Animator FaderAnimator;
        public Animator SelectorAnimator;
        public CanvasGroup SelectionCanvasGroup;
        private Color _fontDisabledColor;
        private Color _fontSelectedColor;
        private Color _fontDeselectedColor;
        public LayoutElement Header;
        public Text breadcrumbsText;
        public GameObject backButton;

        public float VerticalSelectionHeight = 100;
        public float HorizontalSelectionHeight = 120;
        public GameObject VerticalCloseButton;
        public GameObject HorizontalCloseButton;
        public float VerticalHeaderHeight = 100;
        public float HorizontalHeaderHeight = 120;

        public LayoutElement breadcrumbsHeader;
        public float verticalBreadcrumbsHeight = 70;
        public float horizontalBreadcrumbsHeight = 100;

        public RectTransform scrollContents;

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

        public void Apply(ColorScheme scheme)
        {
            _fontDisabledColor = scheme.GetColorForType(ColorScheme.ColorType.Disabled);
            _fontDeselectedColor = scheme.GetColorForType(ColorScheme.ColorType.PrimaryText);
            _fontSelectedColor = scheme.GetColorForType(ColorScheme.ColorType.Acccent);
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
            breadcrumbsHeader.minHeight = portrait ? verticalBreadcrumbsHeight : horizontalBreadcrumbsHeight;
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

            var slideEffectPos = scrollContents.anchoredPosition;
            slideEffectPos.y -= 100f;
            scrollContents.anchoredPosition = slideEffectPos;
            
            Canvas.ForceUpdateCanvases(); //Necessary for getting correct position for SelectionBar

            foreach (var variant in variants)
            {
                var option = Options[i];
                option.OptionNameText.text = variant;
                option.isActive = true;
                if (currentDropdown.SelectedOption == variant)
                {
                    option.OptionNameText.color = _fontSelectedColor;
                    option.SetEmphasisLines(true, true);
                }
                else
                {
                    option.OptionNameText.color = _fontDeselectedColor;
                    option.SetEmphasisLines(false, true);
                }
                
                //Check if variant chain can continue from here
                var variantDictionary = currentDropdown.GetVariantBreadcrumbs(new Dictionary<string, string>());
                variantDictionary[optionName] = variant;

                var allVariantList = ui.ProductPage.product.GetAllVariantsForOptions(variantDictionary);

                if (allVariantList == null)
                {
                    option.isActive = false;
                    option.OptionNameText.color = _fontDisabledColor;
                    option.priceText.text = "Unavailable";
                }
                else
                {
                    option.priceText.text = Product.GetFormattedPriceRangeForVariants(allVariantList);
                }

                i++;
            }
        }

        public void ShowSelection()
        {
            SelectorAnimator.SetBool("Opened", true);
            ui.ProductPage.HideMainLayout();
        }

        public void HideSelection(bool updateVariant = true)
        {
            SelectorAnimator.SetBool("Opened", false);
            if(updateVariant)
                ui.ProductPage.UpdateVariant();
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