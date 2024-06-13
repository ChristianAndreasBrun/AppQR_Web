using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public partial class UpdateManager : MonoBehaviour
{
    string urlGetSingleProduct = "http://localhost:8080/appqr/singleproduct.php";
    string urlUpdateProduct = "http://localhost:8080/appqr/updateproduct.php";

    public AppManager manager;
    [SerializeField] GameObject updatePanel;
    [SerializeField] TMP_InputField referenceField, descriptionField, priceField, nameField;
    [SerializeField] TMP_Dropdown categoryDropdown;

    [Header("List of Products")]
    public Product[] allProducts;

    [Header("Info Fields")]
    public TextMeshProUGUI nameFieldText;
    public TextMeshProUGUI categoryFieldText;
    public TextMeshProUGUI descriptionFieldText;
    public TextMeshProUGUI priceFieldText;

    [Header("Alert")]
    [SerializeField] GameObject alertPanel;
    [SerializeField] TextMeshProUGUI alertText;


    public void GetProduct()
    {
        if (referenceField.text.Equals("") == false)
        {
            StartCoroutine(c_GetSingleProduct());
            updatePanel.SetActive(true);
        }
    }

    public void UpdateProduct()
    {
        if (!string.IsNullOrEmpty(referenceField.text))
        {
            StartCoroutine(c_UpdateProduct());
        }
    }


    IEnumerator c_GetSingleProduct()
    {
        WWWForm form = new WWWForm();
        form.AddField("ref", referenceField.text);
        
        UnityWebRequest request = UnityWebRequest.Post(urlGetSingleProduct, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
            yield return 0;
        }
        else
        {
            string jsonData = request.downloadHandler.text;
            if (jsonData.Equals("none"))
            {
                Debug.Log($"El producto con referencia: {referenceField.text}, no existe.");
                ShowAlert("El producto con la referencia indicada no existe", Color.magenta);
            }
            else
            {
                RootObject obj = JsonUtility.FromJson<RootObject>("{\"products\":" + jsonData + "}");
                allProducts = obj.products;
                if (allProducts.Length > 0)
                {
                    nameFieldText.text = allProducts[0].name;
                    categoryFieldText.text = allProducts[0].category;
                    descriptionFieldText.text = allProducts[0].description;
                    priceFieldText.text = allProducts[0].price.ToString("00.00") + " $";
                }
                else
                {
                    Debug.Log("No se encontraron productos en la base de datos.");
                    ShowAlert("El producto no se encuentra en la base de datos", Color.red);
                }

                manager.SetProduct();
            }
        }
    }

    IEnumerator c_UpdateProduct()
    {
        string resultPrice = priceField.text.Replace(",", ".");

        WWWForm form = new WWWForm();
        form.AddField("reference", referenceField.text);
        form.AddField("name", nameField.text);
        form.AddField("description", descriptionField.text);
        form.AddField("price", resultPrice);
        form.AddField("category", categoryDropdown.options[categoryDropdown.value].text);

        UnityWebRequest request = UnityWebRequest.Post(urlUpdateProduct, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error al conectar con el servidor para actualizar el producto.");
            ShowAlert("Error de conexión al actualizar", Color.red);
            ClearForm();
        }
        else
        {
            string result = request.downloadHandler.text;
            if (result.Contains("correct"))
            {
                Debug.LogError("Error al actualizar el producto en la base de datos.");
                ShowAlert("El producto no se ha actualizado correctamente", Color.magenta);
                ClearForm();
            }
            else
            {
                Debug.Log("El producto se ha actualizado correctamente.");
                ShowAlert("El producto se ha actualizado correctamente", Color.green);
                ClearForm();
                updatePanel.SetActive(false);
            }
        }
    }


    public void ClearForm()
    {
        nameField.text = string.Empty;
        descriptionField.text = string.Empty;
        priceField.text = string.Empty;
        categoryDropdown.value = 0;
    }

    void ShowAlert(string message, Color textColor, float timeToClose = 3f)
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

[System.Serializable]
public class Product
{
    public int id;
    public string name;

    [TextArea(3, 10)]
    public string description;

    public string category;
    public float price;
    public string reference;
}

[System.Serializable]
public class RootObject
{
    public Product[] products;
}