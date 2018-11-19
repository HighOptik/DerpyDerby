using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_Controller : MonoBehaviour
{
    public PlacementManager PMScript;
    public AudioSource engineSFX;
    public Text PlacementText;
    public PickUpControl PUCScript;
    public Transform waypointContainer;
    public List<Transform> waypoints;
    public int currentWaypoint = 0;
    public float topSpeed = 150;
    public float currentSpeed;
    public float maxReverseSpeed = -50;
    public float maxTurnAngle = 10;
    public float maxBrakeTorque = 100;
    public float maxTorque = 10;
    public float decelerationTorque = 30;
    public Vector3 centerOfMassAdjustment = new Vector3(0f, -0.9f, 0f);
    public float spoilerRatio = 0.1f;
    public float handbrakeForwardSlip = 0.04f;
    public float handbrakeSidewaysSlip = 0.08f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public Transform wheelTransformFL;
    public Transform wheelTransformFR;
    public Transform wheelTransformBL;
    public Transform wheelTransformBR;
    private Rigidbody body;
    public float lapCount;
    public float lapMax;
    public float inputSteer;
    public float inputTorque;
    public bool applyHandbrake;
    public bool compPlayer;
    public bool gameOver;
    public int placement;
    public CanvasGroup gameOverCanvas;


    void Start()
    {
        engineSFX = GetComponent<AudioSource>();
        PUCScript = GetComponent<PickUpControl>();
        //get the waypoints from the track.
        if (compPlayer)
        {
            GetWaypoints();
        }
        body = GetComponent<Rigidbody>();

    }

    void SetSlipValues(float forward, float sideways)
    {
        WheelFrictionCurve tempStruct = wheelBR.forwardFriction;
        tempStruct.stiffness = forward;
        wheelBR.forwardFriction = tempStruct;
        tempStruct = wheelBR.sidewaysFriction;
        tempStruct.stiffness = sideways;
        wheelBR.sidewaysFriction = tempStruct;

        tempStruct = wheelBL.forwardFriction;
        tempStruct.stiffness = forward;
        wheelBL.forwardFriction = tempStruct;
        tempStruct = wheelBL.sidewaysFriction;
        tempStruct.stiffness = sideways;
        wheelBL.sidewaysFriction = tempStruct;
    }

    void GetWaypoints()
    {
        foreach (Transform c in waypointContainer)
        {
            waypoints.Add(c);
        }
        //initialize the waypoints array so that is has enough space to store the nodes.
        Transform[] potentialWaypoints = waypointContainer.GetComponentsInChildren<Transform>();

        //loop through the list and copy the nodes into the array.
        //start at 1 instead of 0 to skip the WaypointContainer’s transform.
        for (int i = 1; i < potentialWaypoints.Length; ++i)
        {
            waypoints[i - 1] = potentialWaypoints[i];
        }
    }
    void FixedUpdate()
    {
        if(gameOver)
        {
            FadeInGOCanvas();
        }
        if (!compPlayer)
        {
            if (PMScript.playerLap >= PMScript.comp1Lap)
            {
                PlacementText.text = "1".ToString();
            }
            else if (PMScript.playerLap <= PMScript.comp1Lap)
            {
                PlacementText.text = "2";
            }
        }
        if (compPlayer)
        {
            if (PMScript.playerLap >= PMScript.comp1Lap)
            {
                PlacementText.text = "2".ToString();
            }
            else if (PMScript.playerLap <= PMScript.comp1Lap)
            {
                PlacementText.text = "1";
            }
        }
        //calculate turn angle
        if (compPlayer)
        {
            Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z));
            inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

            //Spoilers add down pressure based on the car’s speed. (Upside-down lift)
            Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
            body.AddForce(-transform.up * (localVelocity.z * spoilerRatio), ForceMode.Impulse);

            //calculate torque.		
            if (Mathf.Abs(inputSteer) < 0.5f)
            {
                //when making minor turning adjustments speed is based on how far to the next point.
                inputTorque = (RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude);
                applyHandbrake = false;
            }
            else
            {
                //we need to make a hard turn, if moving fast apply handbrake to slide.
                if (body.velocity.magnitude > 10)
                {
                    applyHandbrake = true;
                }
                //if not moving forward backup and turn opposite.
                else if (localVelocity.z < 0)
                {
                    applyHandbrake = false;
                    inputTorque = -1;
                    inputSteer *= -1;
                }
                //let off the gas while making a hard turn.
                else
                {
                    applyHandbrake = false;
                    inputTorque = 0;
                }
            }

            //set slip values
            //if close enough, change waypoints.
            if (RelativeWaypointPosition.magnitude < 25)
            {
                if (compPlayer)
                {
                    currentWaypoint++;
                    if (currentWaypoint >= waypoints.Count)
                    {
                        Car_Controller cc = this;
                        currentWaypoint = 0;
                        lapCount++;
                        if (lapCount >= lapMax)
                        {
                            gameOver = true;
                            FadeInGOCanvas();
                        }

                    }
                }
            }

            //front wheel steering
            wheelFL.steerAngle = inputSteer * maxTurnAngle;
            wheelFR.steerAngle = inputSteer * maxTurnAngle;

            //calculate max speed in KM/H (optimized calc)
            currentSpeed = wheelBL.radius * wheelBL.rpm * Mathf.PI * 0.12f;
            if (currentSpeed < topSpeed && currentSpeed > maxReverseSpeed)
            {
                //rear wheel drive.
                wheelBL.motorTorque = inputTorque * maxTorque;
                wheelBR.motorTorque = inputTorque * maxTorque;
            }
            else
            {
                //can't go faster, already at top speed that engine produces.
                wheelBL.motorTorque = 0;
                wheelBR.motorTorque = 0;
            }
        }else if (!compPlayer)
        {
            //front wheel steering
            wheelFL.steerAngle = Input.GetAxis("Horizontal") * maxTurnAngle;
            wheelFR.steerAngle = Input.GetAxis("Horizontal") * maxTurnAngle;

            //calculate max speed in KM/H (optimized calc)
            currentSpeed = wheelBL.radius * wheelBL.rpm * Mathf.PI * 0.12f;
            if (currentSpeed < topSpeed && currentSpeed > maxReverseSpeed)
            {
                //rear wheel drive.
                wheelBL.motorTorque = Input.GetAxis("Vertical") * maxTorque;
                wheelBR.motorTorque = Input.GetAxis("Vertical") * maxTorque;
                engineSFX.PlayOneShot(engineSFX.GetComponent<AudioClip>());
            }
            else
            {
                //can't go faster, already at top speed that engine produces.
                wheelBL.motorTorque = 0;
                wheelBR.motorTorque = 0;
            }

            //Spoilers add down pressure based on the car’s speed. (Upside-down lift)
            Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
            GetComponent<Rigidbody>().AddForce(-transform.up * (localVelocity.z * spoilerRatio), ForceMode.Impulse);

            //Handbrake controls
            if (Input.GetButton("Jump"))
            {
                applyHandbrake = true;
                wheelFL.brakeTorque = maxBrakeTorque;
                wheelFR.brakeTorque = maxBrakeTorque;
                if (GetComponent<Rigidbody>().velocity.magnitude > 1)
                {
                    SetSlipValues(handbrakeForwardSlip, handbrakeSidewaysSlip);
                }
                else
                {
                    SetSlipValues(1f, 1f);
                }
            }
            else
            {
                applyHandbrake = false;
                wheelFL.brakeTorque = 0;
                wheelFR.brakeTorque = 0;
                SetSlipValues(1f, 1f);
            }

            //apply deceleration when not pressing the gas or when breaking in either direction.
            if (!applyHandbrake && ((Input.GetAxis("Vertical") <= -0.5f && localVelocity.z > 0) || (Input.GetAxis("Vertical") >= 0.5f && localVelocity.z < 0)))
            {
                wheelBL.brakeTorque = decelerationTorque + maxTorque;
                wheelBR.brakeTorque = decelerationTorque + maxTorque;
            }
            else if (!applyHandbrake && Input.GetAxis("Vertical") == 0)
            {
                wheelBL.brakeTorque = decelerationTorque;
                wheelBR.brakeTorque = decelerationTorque;
            }
            else
            {
                wheelBL.brakeTorque = 0;
                wheelBR.brakeTorque = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boost")
        {
            PUCScript.BoostCar(this);
            Destroy(other.gameObject);
        }
        if (other.tag == "CheckPoint" && !compPlayer)
        {
            other.GetComponentInParent<LapCheckPointManager>().CheckCheckPoints(other);
        }
    }
    public void FadeInGOCanvas()
    {
        gameOverCanvas.alpha += 1 * Time.deltaTime;
    }
}

