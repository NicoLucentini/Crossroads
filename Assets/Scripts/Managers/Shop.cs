using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Shop : MonoBehaviour
{
    

    int index;
    int maxIndex;
    

    [Header("UI")]
    public Text carText;
    public Text carCost;
    public GameObject blockedIcon;
    public GameObject unblockButton;
    public Image carImage;

    public Text moneyText;
    public List<Sprite> carSprites;

    public static System.Action onCarBuyed;

    private void Start()
    {
        LoadCarSprites();
    }

    //DESPUES FIJARME PORQUE PARECE QUE SI SE PUEDE HACER DESDE EL INSPECTOR..
    public void LoadCarSprites()
    {
        carSprites = Resources.LoadAll<Sprite>("CarSprites").ToList();

        var cars = GameManager.instance.spawner.carSpawns;

        
        cars[0].carSprite = carSprites[4];//normal
        cars[1].carSprite = carSprites[8];//hatch
        cars[2].carSprite = carSprites[9];//micro
        cars[3].carSprite = carSprites[0];//cargo
        cars[4].carSprite = carSprites[2];//bus
        cars[5].carSprite = carSprites[7];//coupe
        cars[6].carSprite = carSprites[3];//mpv
        cars[7].carSprite = carSprites[5];//pickup
        cars[8].carSprite = carSprites[10];//station
        cars[9].carSprite = carSprites[6];//van

    }

    public void Next()
    {
        //sacar los autos especiales...4 - 1
        if (index < GameManager.instance.spawner.carSpawns.Count - 5)
            index++;

        GetStatus();
    }
    public void Back()
    {
        if (index > 0)
            index--;

        GetStatus();
    }

    public void GetStatus()
    {
        CarSpawn spawn = GameManager.instance.spawner.carSpawns[index];

        carText.text = spawn.type.ToString();
        carCost.text = spawn.blocked ?  "Cost " + spawn.unblockCost : "Owned";
        carImage.sprite = spawn.carSprite;
        blockedIcon.SetActive(spawn.blocked);
        unblockButton.SetActive(spawn.blocked);
    }

    public void Buy()
    {
        //TODO hacer el chequeo de recursos
        CarSpawn spawn = GameManager.instance.spawner.carSpawns[index];

        if (GameManager.instance.profile.points > spawn.unblockCost)
        {
            spawn.blocked = false;
            GameManager.instance.profile.blockedData[index] = false;
            blockedIcon.SetActive(spawn.blocked);
            unblockButton.SetActive(spawn.blocked);
            Debug.Log("Car buyed " + spawn.type);
            
            GameManager.instance.profile.points -= spawn.unblockCost;
            moneyText.text = "Points " + GameManager.instance.profile.points;
            
            onCarBuyed?.Invoke();
            GameManager.instance.playerManager.SaveLocalData();
        }
    }
}
