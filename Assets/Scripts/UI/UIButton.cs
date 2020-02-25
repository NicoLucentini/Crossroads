using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [Header("References")]
    public Button button;  
    public RectTransform rect;
    public Text text;
    public Image image;

    [Header("Status")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0);

    [Header("Settings")]
    public bool fitScreen;

    private void Start()
    {
        if (fitScreen)
        {
            CanvasScaler c = FindObjectOfType<CanvasScaler>();
            float newScaleX = Screen.width / c.referenceResolution.x;
            float newScaleY = Screen.height / c.referenceResolution.y;

            rect.localScale = new Vector3(rect.localScale.x * newScaleX, rect.localScale.y * newScaleY, 1);
        }
    }

    public void SetOffset(Vector3 off)
    {
        offset = off;
    }
    public void SetImage(Sprite sp)
    {
        image.sprite = sp;
    }
    public void SetOnClick(System.Action func)
    {
        button.onClick.AddListener(() => func());
    }
    public void SetText(string val)
    {
        text.text = val;
    }
    public void ShowText(bool on)
    {
        text.enabled = on;
    }
    public void LateUpdate()
    {
        if (target == null) return;

        Vector2 v = RectTransformUtility.WorldToScreenPoint(Camera.main, target.transform.position + offset);
        rect.position = new Vector3(v.x, v.y, 0);
    }
}
