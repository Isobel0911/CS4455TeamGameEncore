// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class UIInventory : MonoBehaviour
// {
//     void Start()
//     {
//         gameObject.SetActive(false);
//     }

//     void Update()
//     {
//         ToggleActive();
//     }

//     private bool GetActiveStatus()
//     {
//         return gameObject.activeSelf;
//     }

//     private void ToggleActive()
//     {
//         if (Input.GetKeyDown(KeyCode.Tab))
//         {
//             if (gameObject == null)
//             {
//                 Debug.LogError("inventory is null");
//             }
//             else
//             {
//                 gameObject.SetActive(!GetActiveStatus());
//             }
//         }
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))] 
public class UIInventory : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("PauseMenuToggle: CanvasGroup component not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp (KeyCode.Tab)) {
            if (canvasGroup.interactable) {         //disable in-game menu, enable game
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                Time.timeScale = 1f;
            }
            else                                    //enable in-game menu, disable game
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
            }
        }
    }
}
