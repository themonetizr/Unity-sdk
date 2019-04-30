using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class VariantsDropdown : MonoBehaviour
{

    List<string> _options;
    public string OptionName;
    public Text OptionText;
    public Text OptionNameText;
    public string SelectedOption;

    public void Init(List<string> options, string optionName)
    {
        _options = options;
        OptionName = optionName;
        
        SelectedOption = options.FirstOrDefault();
        OptionText.text = SelectedOption;
        OptionNameText.text = optionName.ToUpper();

    }

    public void SelectValue(int index)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
