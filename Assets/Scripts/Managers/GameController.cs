using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public bool topIsOpen = true;

    public GameObject horizontalBarrier;
    public GameObject verticalBarrier;


    public List<Renderer> verticalRenderers;
    public List<Renderer> horizontalRenderers;

    public List<GameObject> verticalHalosGreen;
    public List<GameObject> horizontalHalosRed;

    public List<GameObject> verticalHalosRed;
    public List<GameObject> horizontalHalosGreen;

    public System.Action onTap;
    public bool usesBigButton;
    public Button button;

    private void Start()
    {
        if (usesBigButton)
            button.gameObject.SetActive(true);
    }
    public void OnClickTap()
    {
        if (!enabled) return;
        if (onTap != null)
            onTap();
    }

    private void Update()
    {
        if (!usesBigButton)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                OnClickTap();
            }
        }
    }

    public void SetOnTap()
    {
        if (GameManager.instance.mode != GameMode.DRIVER)
        {
            onTap = OnTap;
            //GuiManager.instance.trafficLightTimer.gameObject.SetActive(false);  
        }
        if (GameManager.instance.mode == GameMode.DRIVER)
        {
            onTap = ThrowRay;

            InvokeRepeating("OnTap",0, 15);

          //  GuiManager.instance.trafficLightTimer.gameObject.SetActive(true);
        }
    }

    public void ThrowRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Car car = null;
        if (Physics.Raycast(ray, out hit, 1 << Layers.CARS))
        {
            car = hit.collider.gameObject.GetComponent<Car>();
        }

        if (car != null) { car.BreakOnOff(); }
    }

    IEnumerator CTTimer()
    {
        int i = 15;
        while (i > 0)
        {
            GuiManager.instance.ChangeTrafficLightTimer(i.ToString());
            i--;
            yield return new WaitForSeconds(1);
        }
    }

    public void OnTap()
    {
        topIsOpen = !topIsOpen;

        if (GameManager.instance.mode == GameMode.DRIVER)
        {
            StartCoroutine( CTTimer());
        }


        ModifyBarriers(topIsOpen);
    }
    public void ModifyBarriers(bool topOn)
    {
        horizontalBarrier.SetActive(topOn);
        verticalBarrier.SetActive(!topOn);

        if (topOn)
        {
            foreach (var vr in verticalRenderers)
            {
                vr.material.SetColor("_Color", Color.green);
            }
            foreach (var vr in horizontalRenderers)
            {
                vr.material.SetColor("_Color", Color.red);
            }

        }
        else
        {

            foreach (var vr in verticalRenderers)
            {
                vr.material.SetColor("_Color", Color.red);
            }
            foreach (var vr in horizontalRenderers)
            {
                vr.material.SetColor("_Color", Color.green);
            }

        }

        foreach (var h in verticalHalosGreen)
        {
            h.SetActive(topOn);
        }
        foreach (var h in verticalHalosRed)
        {
            h.SetActive(!topOn);
        }
        foreach (var h in horizontalHalosGreen)
        {
            h.SetActive(!topOn);
        }
        foreach (var h in horizontalHalosRed)
        {
            h.SetActive(topOn);
        }


    }
   
}
