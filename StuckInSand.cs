using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StuckInSand : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;

    public RCC_CarControllerV3 controller;
    public LogitechMechanic logiMech;
    public NewScenario scenarioManager;
    public LMVControlMonitor lmvController;

    public GameObject vehicle;
    public GameObject uiPopup;
    public GameObject scenarioCamera;
    public GameObject correctRange;
    public GameObject scenario;
    public GameObject manager;
    public GameObject[] bakkieWheels;
    public GameObject[] bmWheels;

    public Rigidbody rb;

    public WheelCollider[] wheelColliders;

    public bool entered = false;
    public bool buttonPressed = false;

    //---------LOGITECH CONFIGURE---------------//
    //INDEX NAMES
    int a, b, c, d;
    //---------------end of logitech-----------//


    // Start is called before the first frame update
    void Start()
    {
        //---------LOGITECH CONFIGURE---------------//
        //Steering Wheel
        if (Joystick.all[0].name == "Logitech G29 Driving Force Racing Wheel")
        {
            a = 0;
        }
        else if (Joystick.all[1].name == "Logitech G29 Driving Force Racing Wheel")
        {
            a = 1;
        }
        else if (Joystick.all[2].name == "Logitech G29 Driving Force Racing Wheel")
        {
            a = 2;
        }
        else if (Joystick.all[3].name == "Logitech G29 Driving Force Racing Wheel")
        {
            a = 3;
        }
        ////Extreme 3D PRO 
        //if (Joystick.all[0].name == "Logitech Extreme 3D pro")
        //{
        //    b = 0;
        //}
        //else if (Joystick.all[1].name == "Logitech Extreme 3D pro")
        //{
        //    b = 1;
        //}
        //else if (Joystick.all[2].name == "Logitech Extreme 3D pro")
        //{
        //    b = 2;
        //}
        //else if (Joystick.all[3].name == "Logitech Extreme 3D pro")
        //{
        //    b = 3;
        //}

        ////Extreme 3D PRO 1
        //if (Joystick.all[0].name == "Logitech Extreme 3D pro1")
        //{
        //    c = 0;
        //}
        //else if (Joystick.all[1].name == "Logitech Extreme 3D pro1")
        //{
        //    c = 1;
        //}
        //else if (Joystick.all[2].name == "Logitech Extreme 3D pro1")
        //{
        //    c = 2;
        //}
        //else if (Joystick.all[3].name == "Logitech Extreme 3D pro1")
        //{
        //    c = 3;
        //}

        ////HandBrake
        //if (Joystick.all[0].name == "Logitech Logitech Attack 3")
        //{
        //    d = 0;
        //}
        //else if (Joystick.all[1].name == "Logitech Logitech Attack 3")
        //{
        //    d = 1;
        //}
        //else if (Joystick.all[2].name == "Logitech Logitech Attack 3")
        //{
        //    d = 2;
        //}
        //else if (Joystick.all[3].name == "Logitech Logitech Attack 3")
        //{
        //    d = 3;
        //}
        //---------------end of logitech-----------//

        scenarioCamera.SetActive(false);
        correctRange.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(a))
        {
            LogitechGSDK.LogiSteeringInitialize(true);
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(a);
            SteeringWheelButton(rec);

            if (logiMech.controller_Inputs.handbrakeInput == Mathf.Clamp01((1f - Mathf.Abs(rec.lRz / -32768f))) && lmvController.currentRange == 3)
            {
                scenarioManager.passed = true;
            }
            else if (logiMech.controller_Inputs.handbrakeInput != Mathf.Clamp01((1f - Mathf.Abs(rec.lRz / -32768f))) && lmvController.currentRange != 3)
            {
                scenarioManager.failed = true;
            }
        }

        if (scenarioManager.scenarioItems.activeInHierarchy == false)
        {
            rb.position = new Vector3(846.8f, 39.162f, 2294.8f);
            uiPopup.SetActive(false);
            scenarioCamera.SetActive(false);
            rb.constraints = RigidbodyConstraints.None;
        }

        if (this.gameObject.activeInHierarchy == false)
        {
            for (int i = 0; i < bakkieWheels.Length; i++)
            {
                bakkieWheels[i].SetActive(false);
            }

            for (int j = 0; j < bmWheels.Length; j++)
            {
                bmWheels[j].SetActive(true);
            }
        }
    }

    void SteeringWheelButton(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {

        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128)
            {
                if (i == 3)
                {
                    rb.constraints = RigidbodyConstraints.None;
                    rb.position = new Vector3(846.8f, 38.873f, 2294.8f);
                    uiPopup.SetActive(false);
                    scenarioCamera.SetActive(false);
                    scenario.SetActive(false);
                    buttonPressed = true;
                    manager.SetActive(true);
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Sand_Scenario();
            entered = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.position = new Vector3(846.8f, 38, 2294.8f);
            uiPopup.SetActive(true);
            scenarioCamera.SetActive(true);
            for (int i = 0; i < bakkieWheels.Length; i++)
            {
                bakkieWheels[i].SetActive(true);
            }

            for (int j = 0; j < bmWheels.Length; j++)
            {
                bmWheels[j].SetActive(false);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && scenarioManager.passed || scenarioManager.failed)
        {
            rb.position = new Vector3(846.8f, 39.162f, 2294.8f);
            uiPopup.SetActive(false);
            scenarioCamera.SetActive(false);
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void Sand_Scenario()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (entered)
            {
                wheelColliders[i].mass = 100;
                wheelColliders[i].radius = 0.1f;
                wheelColliders[i].wheelDampingRate = 0;
                wheelColliders[i].suspensionDistance = 0;
                uiPopup.SetActive(true);
            }

            if (buttonPressed == true)
            {
                wheelColliders[i].mass = 40;
                wheelColliders[i].radius = 0.4f;
                wheelColliders[i].wheelDampingRate = 1;
                wheelColliders[i].suspensionDistance = 0.2f;
                uiPopup.SetActive(false);
            }
        }
    }

}
