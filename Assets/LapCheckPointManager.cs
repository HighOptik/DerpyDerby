using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCheckPointManager : MonoBehaviour {
    public Car_Controller PlayerCCScript;
    public List<Transform> waypointList;
    public Transform CheckPointContainer;
    public int checkpointsHit;
     
    // Use this for initialization
    void Start () {

        foreach (Transform c in CheckPointContainer)
        {
            waypointList.Add(c);
        }
    }

    public void CheckCheckPoints(Collider col)
    {
        if (checkpointsHit < waypointList.Count)
        {            
                checkpointsHit++;
            col.enabled = false;

        }
        if(checkpointsHit == waypointList.Count)
        {
           StartCoroutine(ResetCheckPoints());
        }
    }
    public IEnumerator ResetCheckPoints()
    {
        Debug.Log("RESETCHECKPOINTS");
        yield return new WaitForSeconds(1f);
        checkpointsHit = 0;
        PlayerCCScript.lapCount++;
        //STOP GAME TIME ON LAP MAX
        if (PlayerCCScript.lapCount >=1)
        {
            PlayerCCScript.FadeInGOCanvas();
            PlayerCCScript.enabled = false;
        }
        foreach (Transform cp in CheckPointContainer)
        {
            cp.GetComponent<Collider>().enabled = true;
        }
    }
    public void DisableCheckPoint(Collider cpCol)
    {
        cpCol.enabled = false;
    }
}
