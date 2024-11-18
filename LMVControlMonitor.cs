using UnityEngine;
using UnityEngine.InputSystem;

public class LMVControlMonitor : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;
    public LogitechMechanic logiManual;
    public LogitechAutomatic logiAuto;
    [SerializeField]
    private RCC_CarControllerV3 carControl;
    private RCC_Settings settings;
    private RCC_WheelCollider[] wheelColliders;
    public TestingINput testInputs;
    //public LogitechButtonsFunctionallity buttonFunctionality;

    public enum RangeModes
    {
        Standard2H, LowRange4L, HighRange4H
    }
    public RangeModes rang = RangeModes.Standard2H;
    public enum DriveModes : int
    {
        modeNormal = 1, modeHigh = 2, modeLow = 3
    }
    public DriveModes driveMode = DriveModes.modeNormal;

    public int range;
    public int currentRange;
    public int maxRange = 3;
    public int minRange = 1;

    [SerializeField] private float rayLeng = 1.0f;
    public float slopeThreshold = 0.5f;
    public float speed;
    public float torque;
    public float rpm;
    public float maxInclineAngle = 20.0f;
    public float gears;
    public float lowRGear = 3;
    public float maxRPM;

    [SerializeField] private LayerMask groundLayer;
    public string[] targetTag;
    public string currentMode;

    [SerializeField]
    public bool clutchEngaged = false;
    public bool nobEngaged = false;
    public bool canSwitch;
    public bool needToSwitch = false;
    private bool ispaused = false;
    private bool gear0, gear1, gear2, gear3, gear4, gear5, gearR;

    public Rigidbody rb;

    public WheelCollider wheelCollider;

    [SerializeField]
    private Transform vehicle;
    public Transform turner;

    public GameObject Knob;
    public GameObject revWarning;
    public GameObject brakesWarning;
    public GameObject otherLocation;

    Vector3 initialLocalEulerAngles;
    Quaternion targetRotation = Quaternion.identity;

    //---------LOGITECH CONFIGURE---------------//
    //INDEX NAMES
    int a, b, c, d;
    //---------------end of logitech-----------//

    //public TMP_Text revText;
    //public TMP_Text brakeText;

    private void Start()
    {  //---------LOGITECH CONFIGURE---------------//
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

        //rb = GetComponent<Rigidbody>();

        ModeSwitcher();

        initialLocalEulerAngles = Knob.transform.localEulerAngles;
    }

    public void Update()
    {
        speed = carControl.speed;
        rpm = carControl.engineRPM;
        torque = carControl.tractionHelperStrength;
        gears = carControl.currentGear;
        currentMode = driveMode.ToString();
        maxRPM = carControl.maxEngineRPM;


        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(a))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(a);

            CarModeSwitch(rec);
            GearModeShifter(rec);
        }

        #region KeyboardActivation
        if (Input.GetKey(KeyCode.K))
        {
            ModeSwitcher();
            driveMode = DriveModes.modeHigh;

            Debug.Log("High mode");
        }

        if (Input.GetKey(KeyCode.N))
        {
            ModeSwitcher();
            driveMode = DriveModes.modeNormal;
        }

        if (Input.GetKey(KeyCode.M))
        {
            ModeSwitcher();
            driveMode = DriveModes.modeLow;
        }
        #endregion

        #region WheelDetection
        Vector3 wheelPosition;
        Quaternion wheelRotation;
        wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);

        RaycastHit hit;
        if (Physics.Raycast(wheelPosition, -transform.up, out hit, 10f))
        {
            float inclinationAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (inclinationAngle > maxInclineAngle)
            {
                Debug.Log("The wheel has started an inclination");
            }
        }
        #endregion

        #region Clutchbool
        if (logiManual.ClutchInput == 1)
        {
            clutchEngaged = true;
            canSwitch = true;
            needToSwitch = true;
            if (range == 1)
            {
                driveMode = DriveModes.modeNormal;
                ModeSwitcher();
                Debug.Log("Switch to mode of one");
            }

            if (range == 2)
            {
                driveMode = DriveModes.modeHigh;
                ModeSwitcher();
                Debug.Log("Switch to mode of two");
            }

            if (range == 3)
            {
                driveMode = DriveModes.modeLow;
                ModeSwitcher();
                Debug.Log("Switch to mode of three");
            }
        }
        else
        {
            clutchEngaged = false;
            canSwitch = false;
            needToSwitch = false;
            nobEngaged = false;
        }
        #endregion

        SwitchDriveMode();
        WarningUpdate();
        if (rpm > 2200)
        {
            revWarning.SetActive(true);
            //revText.gameObject.SetActive(true);
        }
        else
        {
            revWarning.SetActive(false);
            //revText.gameObject.SetActive(false);
        }

        if (carControl.brakeInput >= .7f && carControl.brakeInput <= 1f && (rpm < 2200))
        {
            Debug.Log("Both Images should go on");
        }
        //Debug.Log(carControl.brakeInput);
        //Debug.Log(rpm);

        //if (buttonFunctionality.highRangeOn == true && range == 1)
        //{
        //    Debug.LogWarning("I am able to change");
        //}
    }

    public void CarModeSwitch(LogitechGSDK.DIJOYSTATE2ENGINES rec)  // Logitech steering wheel buttons
    {
        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128)
            {
                if (i == 22 && clutchEngaged == true) //To the right 
                {
                    currentRange = range;
                    if (range == 1)
                    {
                        //  testInputs.HighRangeSelected(true);
                        //testInputs.normalRangeAction.performed += ctx => testInputs.HighRangeSelected(engaged: true);
                        range = currentRange + 1;
                    }

                    if (range >= 2 && nobEngaged == true)
                    {
                        // testInputs.HighRangeSelected(true);
                        //testInputs.normalRangeAction.performed += ctx => testInputs.HighRangeSelected(engaged: true);
                        range += 1;
                    }
                    Debug.Log("Nob to the right");
                }

                if (i == 23 && clutchEngaged == true) // To the left
                {
                    currentRange = range;
                    if (range == 3 && nobEngaged == true)
                    {
                        // testInputs.LowRangeSelector(true);
                        //testInputs.lowRangeAction.performed += ctx => testInputs.LowRangeSelector(engaged: true);
                        range = currentRange - 1;
                    }

                    if (range <= 2 && nobEngaged == false)
                    {
                        // testInputs.LowRangeSelector(true);
                        //testInputs.lowRangeAction.performed += ctx => testInputs.LowRangeSelector(engaged: true);
                        range -= 1;
                    }
                    Debug.Log("Nob to the left");
                }

                if (i == 24)
                {
                    nobEngaged = true;
                    //testInputs.MiddleButtonSelected(true);
                    //testInputs.middleButtonAction.performed += ctx => testInputs.MiddleButtonSelected(engaged: true);
                    Debug.Log("Middle button pressed");
                }
            }
        }
    }

    public void GearModeShifter(LogitechGSDK.DIJOYSTATE2ENGINES shifter) // Logitech gears access
    {
        for (int i = 0; i < 128; i++)
        {
            if (shifter.rgbButtons[i] == 128)
            {
                if (i == 14)
                {
                    if (i == 14 && driveMode == DriveModes.modeLow)
                    {
                        carControl.fuelInput = 1f;
                        carControl.engineRunning = true;
                        //carControl.ChangeGear(0);
                        carControl.poweredWheels = 4;
                        carControl.direction = 1;
                        //carControl.currentGear = 0;
                    }
                    Debug.Log(gears);
                }

                if (i == 12)
                {
                    if (i == 12 && driveMode == DriveModes.modeHigh)
                    {
                        carControl.currentGear = 1;
                    }

                    if (i == 12 && driveMode == DriveModes.modeNormal)
                    {
                        carControl.currentGear = 1;
                    }
                }
            }
        }
    }

    public void ModeSwitcher()
    {
        switch (driveMode)
        {
            case DriveModes.modeNormal:
                rang = RangeModes.Standard2H;
                carControl.gearShiftUpRPM = 800;
                carControl.maxEngineTorque = 1250;
                carControl.maxEngineTorqueAtRPM = 1800;
                carControl.brakeTorque = 9000 * 2;
                carControl.maxspeed = 120;
                range = 1;

                Debug.Log("2H mode active");
                break;

            case DriveModes.modeLow:
                rang = RangeModes.LowRange4L;
                carControl.gearShiftUpRPM = 800;
                carControl.maxEngineTorque = 2500;
                carControl.maxEngineTorqueAtRPM = 2500;
                carControl.brakeTorque = 9000 * 2;
                carControl.maxspeed = 120;
                range = 3;

                Debug.Log("4L mode active");
                break;

            case DriveModes.modeHigh:
                rang = RangeModes.HighRange4H;
                carControl.gearShiftUpRPM = 800;
                carControl.maxEngineTorque = 3000;
                carControl.maxEngineTorqueAtRPM = 3600;
                carControl.brakeTorque = 9000 * 2;
                carControl.maxspeed = 120;
                range = 2;

                Debug.Log("4H mode active");
                break;
            default:
                Debug.Log("In normal state");
                break;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit rayHit;

        if (turner != null)
        {
            Vector3 turnerPosition = turner.position;
            Vector3 turnerUp = turner.forward;

            if (Physics.Raycast(turnerPosition, turnerUp, out rayHit, Mathf.Infinity))
            {
                Debug.DrawRay(turnerPosition, turnerUp * rayHit.distance, Color.green);
                Debug.Log("Raycast was hit");

                string hitTag = rayHit.collider.tag;

                for (int i = 0; i < targetTag.Length; i++)
                {
                    if (hitTag == targetTag[0]) // H2
                    {
                        Debug.Log(targetTag[0]);
                        break;
                    }

                    if (hitTag == targetTag[1]) // H4
                    {
                        Debug.Log(targetTag[1]);
                        break;
                    }

                    if (hitTag == targetTag[2]) // L4
                    {
                        Debug.Log(targetTag[2]);
                        break;
                    }
                }
            }
        }
    }

    public void SwitchDriveMode()
    {
        if (range == 1)
        {
            //testInputs.highRangeAction.performed += ctx => testInputs.NormalRangeSelected(engaged: true);
            ModeSwitcher();
            driveMode = DriveModes.modeNormal;
            KnobRotate();
        }

        if (range == 2)
        {

            ModeSwitcher();
            driveMode = DriveModes.modeHigh;
            KnobRotate();
        }

        if (range == 3)
        {

            ModeSwitcher();
            driveMode = DriveModes.modeLow;
            KnobRotate();
        }

        if (range >= maxRange)
        {
            range = maxRange;
        }
        else if (range <= minRange)
        {
            range = minRange;
        }

        Debug.Log("Switched to" + driveMode + "mode");
    }

    public void KnobRotate()
    {
        Vector3 targetLocalEulerAngles = initialLocalEulerAngles;
        if (driveMode == DriveModes.modeNormal)
        {
          //  targetLocalEulerAngles += new Vector3(-11, 90, -90);
            Knob.transform.localRotation = Quaternion.Euler(-11, 90, -90);
        }
        else if (driveMode == DriveModes.modeHigh)
        {
           // targetLocalEulerAngles += new Vector3(-90, 90, -90);
            Knob.transform.localRotation = Quaternion.Euler(-90, 90, -90);
        }
        else if (driveMode == DriveModes.modeLow)
        {
           // targetLocalEulerAngles += new Vector3(-172, 90, -90);
            Knob.transform.localRotation = Quaternion.Euler(-172, 90, -90);
        }

        //Knob.transform.localEulerAngles = targetLocalEulerAngles;

    }

    public void WarningUpdate()
    {


        #region High_Rev_Warning
        if (gears == 0 && rpm >= 2100)
        {
            gear0 = true;
            revWarning.SetActive(true);
            //revText.gameObject.SetActive(true);
            //if (gear0 == true)
            //{
            //    warning.SetActive(true);
            //}
            Debug.LogWarning("THE REV IS HIGH AND THE GEARS NEED TO BE CHANGED TO 1");
        }

        if (gears == 1 && rpm >= 2100)
        {
            gear1 = true;
            revWarning.SetActive(true);
            //revText.gameObject.SetActive(true);
            //if (gear1 == true)
            //{
            //    warning.SetActive(true);
            //}
            Debug.LogWarning("THE REV IS HIGH AND THE GEARS NEED TO BE CHANGED TO 2");
        }

        if (gears == 2 && rpm >= 2100)
        {
            gear2 = true;
            revWarning.SetActive(true);
            // revText.gameObject.SetActive(true);
            //if (gear2 == true)
            //{
            //    warning.SetActive(true);
            //}
            Debug.LogWarning("THE REV IS HIGH AND THE GEARS NEED TO BE CHANGED TO 3");
        }

        if (gears == 3 && rpm >= 2100)
        {
            gear3 = true;
            revWarning.SetActive(true);
            //revText.gameObject.SetActive(true);
            //if (gear3 == true)
            //{
            //    warning.SetActive(true);
            //}
            Debug.LogWarning("THE REV IS HIGH AND THE GEARS NEED TO BE CHANGED TO 4");
        }

        if (gears == 4 && rpm >= 2100)
        {
            gear4 = true;
            revWarning.SetActive(true);
            // revText.gameObject.SetActive(true);
            //if (gear4 == true)
            //{
            //    warning.SetActive(true);
            //}
            Debug.LogWarning("THE REV IS HIGH AND THE GEARS NEED TO BE CHANGED TO 5");
        }

        if (gears == 5 && rpm >= 2100)
        {
            revWarning.SetActive(true);
            gear5 = true;
            // revText.gameObject.SetActive(true);
            //if (gear5 == true)
            //{
            //    warning.SetActive(true);
            //}
            Debug.LogWarning("THE REV IS HIGH AND THE GEARS NEED TO BE CHANGED");
        }
        #endregion

        #region Low_Rev_Management
        if (gears == 0 && rpm <= 2100)
        {
            gear0 = true;
            //revText.gameObject.SetActive(false);
            revWarning.SetActive(false);
            //if (gear0 == true)
            //{
            //    warning.SetActive(false);
            //}
            Debug.LogWarning("THE REV IS UNDER THE MAXIMUM AND CAN CHANGE TO 1");
        }

        if (gears == 1 && rpm <= 2100)
        {
            gear1 = true;
            //revText.gameObject.SetActive(false);
            revWarning.SetActive(false);
            //if (gear1 == true)
            //{
            //    warning.SetActive(false);
            //}
            Debug.LogWarning("THE REV IS UNDER THE MAXIMUM AND CAN CHANGE TO 2");
        }

        if (gears == 2 && rpm <= 2100)
        {
            gear2 = true;
            //revText.gameObject.SetActive(false);
            revWarning.SetActive(false);
            //if (gear2 == true)
            //{
            //    warning.SetActive(false);
            //}
            Debug.LogWarning("THE REV IS UNDER THE MAXIMUM AND CAN CHANGE TO 3");
        }

        if (gears == 3 && rpm <= 2100)
        {
            gear3 = true;
            // revText.gameObject.SetActive(false);
            revWarning.SetActive(false);
            //if (gear3 == true)
            //{
            //    warning.SetActive(false);
            //}
            Debug.LogWarning("THE REV IS UNDER THE MAXIMUM AND CAN CHANGE TO 4");
        }

        if (gears == 4 && rpm <= 2100)
        {
            gear4 = true;
            // revText.gameObject.SetActive(false);
            revWarning.SetActive(false);
            //if (gear4 == true)
            //{
            //    warning.SetActive(false);
            //}
            Debug.LogWarning("THE REV IS UNDER THE MAXIMUM AND CAN CHANGE TO 5");
        }

        if (gears == 5 && rpm <= 2100)
        {
            gear5 = true;
            //revText.gameObject.SetActive(false);
            revWarning.SetActive(false);
            //if (gear5 == true)
            //{
            //    warning.SetActive(false);
            //}
            Debug.LogWarning("THE REV IS UNDER THE MAXIMUM");
        }
        #endregion

        #region HARSH_BRAKING_DETECTION

        if (carControl.brakeInput >= .7f && carControl.brakeInput <= 1.5f)
        {
            Debug.Log("WARNING ON");
            brakesWarning.SetActive(true);
            //brakeText.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("WARNING OFF");
            brakesWarning.SetActive(false);
            //brakeText.gameObject.SetActive(false);
        }
        #endregion

        if (carControl.brakeInput >= .7f && carControl.brakeInput <= 1f && rpm >= 2100)
        {
            brakesWarning.SetActive(false);
            otherLocation.SetActive(true);
        }
        else
        {
            otherLocation.SetActive(false);
        }
    }
}
