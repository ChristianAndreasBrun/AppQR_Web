using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class TabUIKey : MonoBehaviour
{
    EventSystem system;
    private void Start()
    {
        system = EventSystem.current;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            }
            else
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            }

            if (next != null)
            {
                TMP_InputField inputfield = next.GetComponent<TMP_InputField>();
                if (inputfield != null)
                {
                    inputfield.OnPointerClick(new PointerEventData(system));
                }

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }
}
