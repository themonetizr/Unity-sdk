using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class VariantsDropdown : MonoBehaviour
{

    List<string> _options;
    public Dropdown Dropdown;
    public string OptionName;
    public Text OptionText;
    public string SelectedOption;

    public void Init(List<string> options, string optionName)
    {
        _options = options;
        OptionName = optionName;
        OptionText.text = optionName;
        SelectedOption = options.FirstOrDefault();
        Dropdown.options.Clear();
        Dropdown.options = options.Select(x => new Dropdown.OptionData(x)).ToList();
        Dropdown.onValueChanged.AddListener((a) => SelectValue(a));
    }

    public void SelectValue(int index)
    {
        SelectedOption = Dropdown.options.ElementAt(index).text;
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
