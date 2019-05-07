using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
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
                    option.Animator.SetTrigger("Deselect");
            }
            _selectedOption.Animator.SetTrigger("Select");
            var dd = ProductPage.Dropdowns.FirstOrDefault(x => x.OptionName == _optionName);
            dd.OptionText.text = _selectedOption.OptionNameText.text;
            dd.SelectedOption = _selectedOption.OptionNameText.text;
        }
    }
    public Text OptionText;
    public ProductPageScript ProductPage;
    private SelectorOption _selectedOption;
    string _optionName;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitOptions(List<string> variants, string optionName)
    {
        int i = 0;
        OptionText.text = $"Select {optionName}";
        _optionName = optionName;

        foreach (var option in Options)
        {
            option.DisableImage.gameObject.SetActive(true);
            option.Animator.SetTrigger("Deselect");
        }

        foreach (var variant in variants)
        {
            var option = Options[i];
            option.OptionNameText.text = variant;
            option.DisableImage.gameObject.SetActive(false);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
