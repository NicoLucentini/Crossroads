using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPool : MonoBehaviour
{
    public List<Car> actives;
    public List<Car> inactives;
    public int countPerType = 2;

    public CarSpawner spawner;
    public Transform inactivesT;
    public Transform activesT;
    public void Start()
    {
        if(spawner.usesPool)
            Init(spawner.carSpawns);
    }
    public void Init(List<CarSpawn> spawns)
    {
        foreach (var s in spawns)
        {
            if (!s.blocked)
            {
                for (int i = 0; i < countPerType; i++)
                {
                    CreateCar(s.prefab, s.type);
                }
            }
        }
    }
    public void CreateCar(GameObject prefab, CarType type)
    {
        GameObject carGo = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        carGo.SetActive(false);

        Car car = carGo.GetComponent<Car>();
        car.carType = type;
        car.pool = this;
        car.transform.SetParent(inactivesT);
        inactives.Add(car);
    }
    public void InitType(List<CarSpawn> spawns, CarType type)
    {
        foreach (var s in spawns)
        {
            if (!s.blocked && s.type == type)
            {
                CreateCar(s.prefab, s.type);
            }
        }
    }

    public Car GetCar(CarType type)
    {
        
        Car inactiveCar = null;
        foreach (var car in inactives)
        {
            if (car.carType == type)
            {
                inactiveCar = car;
                break;
            }
        }

        if (inactiveCar != null)
        {
            inactives.Remove(inactiveCar);
            actives.Add(inactiveCar);
            inactiveCar.transform.SetParent(activesT);
            inactiveCar.gameObject.SetActive(true);
            return inactiveCar;
        }
        else
        {
            InitType(spawner.carSpawns, type);
            return GetCar(type);
        }
    }
    public void DropCar(Car car)
    {
        car.gameObject.SetActive(false);
        actives.Remove(car);
        inactives.Add(car);
        car.transform.SetParent(inactivesT);
    }
}
