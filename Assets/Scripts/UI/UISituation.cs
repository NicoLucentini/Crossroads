using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISituation : MonoBehaviour
{
    [Header("References")]
    public Image maskImage;
    public Image image;
    public RectTransform rect;
    public Text text;

    [Header("Status")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, 0);

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

    public void SetText(string txt)
    {
        text.text = txt;
    }
    public void SetTextColor(Color c)
    {
        text.color = c;
    }
    public void Show(bool mask, bool img, bool txt)
    {
        maskImage.gameObject.SetActive(mask);
        image.gameObject.SetActive(img);
        text.gameObject.SetActive(txt);
    }
    public void SetOffset(Vector3 off)
    {
        offset = off;
    }
    Vector3 textOffset;
    public void SetTextOffset(Vector3 off)
    {
        textOffset = off;
        text.GetComponent<RectTransform>().position += textOffset;
    }


    public void SetFill(float amt)
    {
        maskImage.fillAmount = amt;
    }
    public void SetMain(Sprite sp)
    {
        image.sprite = sp;
    }
    public void SetMask(Sprite sp)
    {
        maskImage.sprite = sp;
    }
    public void LateUpdate()
    {
        if (target == null) return;

        Vector2 v = RectTransformUtility.WorldToScreenPoint(Camera.main, target.transform.position + offset);
        rect.position = new Vector3(v.x, v.y, 0);
    }
    public void SetPos(Vector3 pos)
    {
        Vector2 v = RectTransformUtility.WorldToScreenPoint(Camera.main, pos + offset);
        rect.position = new Vector3(v.x, v.y, 0);
    }
    public void Anim()
    {
        if(gameObject.activeInHierarchy)
        StartCoroutine(CtAnim());
    }
    IEnumerator CtAnim()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            rect.position += new Vector3(0, 25 * Time.deltaTime, 0);
            
            rect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
