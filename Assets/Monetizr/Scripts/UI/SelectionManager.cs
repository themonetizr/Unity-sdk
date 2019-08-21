using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Monetizr
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
                _selectedOption = value;
                foreach (var option in Options)
                {
                    if (option.gameObject.GetInstanceID() != _selectedOption.gameObject.GetInstanceID())
                    {
                        option.OptionNameText.color = FontDeselectedColor;
                    } 
                }
                _selectedOption.OptionNameText.color = FontSelectedColor;
                SelectionBarAnimator.DoEase(0.2f, 
                    Utility.UIUtilityScript.SwitchToRectTransform(_selectedOption.GetComponent<RectTransform>(), SelectionListLayout).y, 
                    true);
                //SelectionBar.anchoredPosition = Utility.UIUtilityScript.SwitchToRectTransform(_selectedOption.GetComponent<RectTransform>(), SelectionListLayout);
                var dd = ui.ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
                dd.OptionText.text = _selectedOption.OptionNameText.text;
                dd.SelectedOption = _selectedOption.OptionNameText.text;
                StartCoroutine(SelectNextEnumerator());
            }
        }

        private IEnumerator SelectNextEnumerator()
        {
            yield return new WaitForSeconds(0.2f);
            FaderAnimator.SetBool("Faded", true);
            yield return new WaitForSeconds(0.1f);
            NextSelect();
        }

        public Text OptionText;
        public GameObject SelectionPanel;
        public RectTransform SelectionListLayout;
        public RectTransform SelectionBar;
        public SelectionBarAnimator SelectionBarAnimator;
        public Animator FaderAnimator;
        public Animator SelectorAnimator;
        public CanvasGroup SelectionCanvasGroup;
        private Vector2 _selectionBarStartPos;
        public Color FontSelectedColor;
        public Color FontDeselectedColor;
        public RectTransform Header;

        public float VerticalSelectionHeight = 60;
        public float HorizontalSelectionHeight = 100;
        public GameObject VerticalCloseButton;
        public GameObject HorizontalCloseButton;
        public float VerticalHeaderHeight = 100;
        public float HorizontalHeaderHeight = 160;

        private SelectorOption _selectedOption;
        string _optionName;
        private VariantsDropdown _currentDropdown;
        private List<VariantsDropdown> _allDropdowns;

        private void Start()
        {
            _selectionBarStartPos = SelectionBar.anchoredPosition;
            ui.ScreenOrientationChanged += UpdateLayout;
            UpdateLayout(Utility.UIUtilityScript.IsPortrait());
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

            var newSize = SelectionBar.sizeDelta;
            newSize.y = portrait ? VerticalSelectionHeight : HorizontalSelectionHeight;
            SelectionBar.sizeDelta = newSize;

            var newPos = SelectionBar.anchoredPosition;
            if (newPos != _selectionBarStartPos)
            {
                if(!portrait)
                    newPos.y = newPos.y * (HorizontalSelectionHeight / VerticalSelectionHeight);
                else
                    newPos.y = newPos.y / (HorizontalSelectionHeight / VerticalSelectionHeight);
                SelectionBarAnimator.DoEase(0.2f,
                    newPos.y,
                    true);
                //SelectionBar.anchoredPosition = newPos;
            }

            VerticalCloseButton.SetActive(portrait);
            HorizontalCloseButton.SetActive(!portrait);

            var newHeaderSize = Header.sizeDelta;
            newHeaderSize.y = portrait ? VerticalHeaderHeight : HorizontalHeaderHeight;
            Header.sizeDelta = newHeaderSize;
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
            for (int j=0;j<Options.Count;j++)
            {
                Options[j].gameObject.SetActive(j < variants.Count);
            }
            Canvas.ForceUpdateCanvases(); //Necessary for getting correct position for SelectionBar

            foreach (var variant in variants)
            {
                var option = Options[i];
                option.OptionNameText.text = variant;
                if (currentDropdown.SelectedOption == variant)
                {
                    option.OptionNameText.color = FontSelectedColor;
                    if(SelectionBar.anchoredPosition == _selectionBarStartPos)
                        SelectionBar.anchoredPosition = Utility.UIUtilityScript.SwitchToRectTransform(option.GetComponent<RectTransform>(), SelectionListLayout);
                    else
                        SelectionBarAnimator.DoEase(0.2f,
                    Utility.UIUtilityScript.SwitchToRectTransform(option.GetComponent<RectTransform>(), SelectionListLayout).y,
                    true);
                }
                else
                {
                    option.OptionNameText.color = FontDeselectedColor;
                }
                i++;
            }
        }

        public void ShowSelection()
        {
            SelectorAnimator.SetBool("Opened", true);
            SelectionBar.anchoredPosition = _selectionBarStartPos;
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

            InitOptions(nextDd.Options, nextDd.OptionName, nextDd, _allDropdowns);
        }
    }
}