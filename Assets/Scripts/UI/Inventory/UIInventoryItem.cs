using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class UIInventoryItem : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image hoverImage;
    [SerializeField] private Image selectedImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image bgEmptyImage;
    [SerializeField] private Button itemButton;
    [SerializeField] private TextMeshProUGUI itemCount;

    // public UnityAction<ItemSO>

    // Start is called before the first frame update
    void Awake()
    {
        hoverImage.enabled = false;
        selectedImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoverItem()
    {
        hoverImage.enabled = true;
    }

    public void UnhoverItem()
    {
        hoverImage.enabled = false;
    }

    public void Select()
    {
        // The UI element is selected
        selectedImage.enabled = true; // Enable the image
    }

    public void Deselect()
    {
        // The UI element is deselected
        selectedImage.enabled = false; // Disable the image
    }
}
