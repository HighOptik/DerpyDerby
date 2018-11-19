using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpControl : MonoBehaviour
{

    public void BoostCar(Car_Controller carscript)
    {
        StartCoroutine(_Boost(carscript));
    }

    public IEnumerator _Boost(Car_Controller carscript)
    {
        GetComponent<Rigidbody>().velocity *= 1.5f;
        yield return new WaitForSeconds(0f);

    }
}

