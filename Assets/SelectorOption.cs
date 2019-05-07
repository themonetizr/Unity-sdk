using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorOption : MonoBehaviour
{

    public Text OptionNameText;
    public Animator Animator;
    public SelectionManager SelectionManager;
    public Image DisableImage;
    public bool IsActive { get; set; }

    public void SetSelected()
    {
        if (!IsActive)
            return;

        SelectionManager.SelectedOption = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
