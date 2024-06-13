using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWebGL : MonoBehaviour
{
    private void OnMouseDown()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        Color newColor = new Color(r, g, b);

        GetComponent<SpriteRenderer>().color = newColor;
    }
}
