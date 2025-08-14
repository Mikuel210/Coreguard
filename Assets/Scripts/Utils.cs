using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public static class Utils
{
    public static GameObject popupPrefab;

    public static void Popup(string text, Vector3 position, Color? color = null, float size = 0.01f)
    {
        if (color == null) color = Color.white;
        
        GameObject canvas = MonoBehaviour.Instantiate(popupPrefab, position + Vector3.up, Quaternion.identity);
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * size;
        
        TextMeshProUGUI textMesh = canvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color.Value;

        float startingTime = 0.75f;
        float time = startingTime;
        
        FunctionUpdater.Create(() => {
            rectTransform.position += new Vector3(0, Time.unscaledDeltaTime / startingTime);
            textMesh.color = new(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - Time.unscaledDeltaTime / startingTime);
            
            time -= Time.unscaledDeltaTime;
            if (time <= 0f)
            {
                Object.Destroy(canvas);
                return true;
            }

            return false;
        }, "WorldTextPopup");
    }
}
