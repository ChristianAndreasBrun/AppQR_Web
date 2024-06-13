using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;

public partial class AppManager : MonoBehaviour
{
    string urlSetProduct = "http://localhost:8080/appqr/setproduct.php";
    string urlUpdateProduct = "http://localhost:8080/appqr/updateproduct.php";

    [SerializeField] TMP_InputField referenceField, descriptionField, priceField, nameField;
    [SerializeField] TMP_Dropdown categoryDropdown;

    [Header("Alert")]
    [SerializeField] GameObject alertPanel;
    [SerializeField] TextMeshProUGUI alertText;


    private void Start()
    {
        
    }

    public void SetProduct()
    {
        if (string.IsNullOrEmpty(nameField.text) || string.IsNullOrEmpty(descriptionField.text) || string.IsNullOrEmpty(priceField.text) || string.IsNullOrEmpty(referenceField.text))
        {
            ShowAlert("Ningun campo puede estar vacio", Color.red);
        }
        else
        {
            StartCoroutine(c_SetProduct());
        }
    }


    IEnumerator c_SetProduct()
    {
        string resultPrice = priceField.text.Replace(",",".");

        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("description", descriptionField.text);
        form.AddField("price", resultPrice);
        form.AddField("reference", referenceField.text);
        form.AddField("category", categoryDropdown.options[categoryDropdown.value].text);

        UnityWebRequest request = UnityWebRequest.Post(urlSetProduct, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            yield return 0;
        }
        else
        {
            string result = request.downloadHandler.text;
            if (result.Contains("exist"))
            {
                ShowAlert("El producto ya existe", Color.magenta);
            }
            else if (result.Contains("correct"))
            {
                ShowAlert("El producto se ha subido correctamente", Color.green);
                ClearForm();
            }
            else
            {
                ShowAlert("El producto no se ha subido correctamente", Color.magenta);
            }
        }
    }


    public void ClearForm()
    {
        nameField.text = string.Empty;
        descriptionField.text = string.Empty;
        priceField.text = string.Empty;
        referenceField.text = string.Empty;
        categoryDropdown.value = 0;
    }

    void ShowAlert(string message, Color textColor, float timeToClose = 10f)
    {
        alertText.text = message;
        alertText.color = textColor;
        Invoke(nameof(CloseAlert), timeToClose);
        alertPanel.SetActive(true);
    }

    public void CloseAlert()
    {
        CancelInvoke(nameof(CloseAlert));
        alertPanel.SetActive(false);
    }
}