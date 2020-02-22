using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIComplete : MonoBehaviour {

    public Button button;
    public Transform target;
    public RectTransform rect;
    public Text text;
    public Image image;
    public Vector3 offset = new Vector3(0, 5, 0);

    public void Show(bool but, bool img, bool txt)
    {
        button.gameObject.SetActive(but);
        text.gameObject.SetActive(txt);
        image.gameObject.SetActive(img);
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
   
    public void LateUpdate()
    {
        if (target == null) return;

        Vector2 v = RectTransformUtility.WorldToScreenPoint(Camera.main, target.transform.position + offset);
        rect.position = new Vector3(v.x, v.y, 0);
    }
}
