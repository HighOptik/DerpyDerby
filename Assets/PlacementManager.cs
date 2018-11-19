using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour {

    public Car_Controller CCScript;
    public List<GameObject> AllRaceCars;
    public List<float> Placements;
    public float playerLap;
    public float comp1Lap;

    void Update()
    {
        playerLap = AllRaceCars[0].GetComponent<Car_Controller>().lapCount;
        comp1Lap = AllRaceCars[1].GetComponent<Car_Controller>().lapCount;         
    }
}
