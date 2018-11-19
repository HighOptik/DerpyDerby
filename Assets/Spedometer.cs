using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spedometer : MonoBehaviour {

    public Text speedometeText;
    public Car_Controller CCScript;
	// Update is called once per frame
	void Update () {
        speedometeText.text ="Speed: " + CCScript.currentSpeed.ToString("00");
	}
}
