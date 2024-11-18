using UnityEngine;

public class LogitechMechanic : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;

    public RCC_Inputs controllerInputs;
    public RCC_CarControllerV3 carController;
    public RCC_InputManager inputManager;
    public RCC_Settings settings;
    public RCC_LogitechSteeringWheel steeringWheel;
    RCC_Light lights;

    public float xAxis, GasInput, BreakInput, ClutchInput;

    public bool HShift = true;
    bool isInGear;
    public bool ClutchIn;

    public int CurrentGear, temp, previousGear;
    public float limit = 0;
    public float Threshold = 1;

    public void Start()
    {
        controllerInputs = new RCC_Inputs();
        carController = GetComponent<RCC_CarControllerV3>();
        inputManager = GetComponent<RCC_InputManager>();
        //settings = GetComponent<RCC_Settings>();
        lights = GetComponent<RCC_Light>();
        //steeringWheel = GetComponent<RCC_LogitechSteeringWheel>();

        controllerInputs.throttleInput = GasInput;
        controllerInputs.brakeInput = BreakInput;
        controllerInputs.steerInput = xAxis;
        controllerInputs.clutchInput = ClutchInput;

        LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER);
        LogitechGSDK.LogiPlayDamperForce(0, 50);
    }

    public void Update()
    {
        settings.useAutomaticGear = false;
        steeringWheel.useForceFeedback = true;


        steeringWheel = GetComponent<RCC_LogitechSteeringWheel>();
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            HShifter(rec);
            CarController(rec);

            steeringWheel.useHShifter = HShift;
            if (HShift == steeringWheel.useHShifter)
            {
                Debug.Log("Gear being used");
            }

            xAxis = rec.lX / 32767; //-1 0 1

            if (rec.lY > 0)
            {
                GasInput = 0;
            }
            else if (rec.lY < 0)
            {
                GasInput = rec.lY / -32767;
            }

            if (rec.lRz > 0)
            {
                BreakInput = 0;
            }
            else if (rec.lRz < 0)
            {
                BreakInput = rec.lRz / -10240;
            }

            if (rec.rglSlider[0] > 0) // Clutch input
            {
                ClutchInput = 0;
            }
            else if (rec.rglSlider[0] < 0)
            {
                ClutchInput = rec.rglSlider[0] / -32768;
            }

            BreakInput = carController.brakeInput;
            if (BreakInput == rec.lRz / -10240)
            {
                carController.brakeInput = 0;
                // carController.speed = 0;
                carController.cutGas = true;
                Debug.Log("Breaks are working");
            }

            if (rec.lX >= 0 && carController.indicatorsOn == RCC_CarControllerV3.IndicatorsOn.Left)
            {
                carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Off;
                Debug.Log("Steering wheel is at 0");
            }
            if (rec.lX <= 0 && carController.indicatorsOn == RCC_CarControllerV3.IndicatorsOn.Right)
            {
                carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Off;
                Debug.Log("Steering wheel is at 0");
            }
        }

        else
        {
            Debug.Log("No Steering Wheel Conected!");
        }
    }

    void HShifter(LogitechGSDK.DIJOYSTATE2ENGINES shifter)
    {
        isInGear = false;
        ClutchIn = false;
        steeringWheel.atNGear = isInGear;
        if (isInGear == steeringWheel.atNGear)
        {
            //Debug.Log("Gear is being fed from other feed");
        }

        for (int i = 0; i < 128; i++)
        {
            if (shifter.rgbButtons[i] == 128)
            {
                if (ClutchInput > 0.5f)
                {
                    if (i == 12 && CurrentGear != 0)
                    {
                        CurrentGear = 0;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftUp();
                        carController.currentGear++;
                        if (carController.speed <= 20)
                        {
                            
                            carController.GearShiftDown();
                        }
                    }
                    else if (i == 13 && CurrentGear != 1)
                    {
                        CurrentGear = 1;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftUp();
                        
                        //if (carController.speed <= 40)
                        //{
                        //    carController.GearShiftDown();
                        //}
                    }
                    else if (i == 14 && CurrentGear != 2)
                    {
                        CurrentGear = 2;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftUp();
                        
                        if (carController.speed <= 50)
                        {
                            
                            carController.GearShiftDown();
                        }
                    }
                    else if (i == 15 && CurrentGear != 3)
                    {
                        CurrentGear = 3;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftUp();
                        if (carController.speed <= 60)
                        {
                            
                            carController.GearShiftDown();
                        }
                    }
                    else if (i == 16 && CurrentGear != 4)
                    {
                        CurrentGear = 4;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftUp();
                        if (carController.speed <= 80)
                        {
                            carController.GearShiftDown();
                        }
                    }
                    else if (i == 17 && CurrentGear != 5)
                    {
                        CurrentGear = 5;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftUp();
                        if (carController.speed <= 120)
                        {
                            carController.GearShiftDown();
                        }
                    }
                    else if (i == 18 && CurrentGear != -1)
                    {
                        CurrentGear = -1;
                        ClutchIn = true;
                        isInGear = true;
                        carController.GearShiftDown();
                        
                        if (carController.speed <= 0)
                        {
                            carController.currentGear--;
                            carController.GearShiftDown();
                        }
                    }

                    else
                    {
                        //isInGear = false;
                        //carController.GearShiftDown();
                        ////CurrentGear = -2;
                        //Debug.Log("Gear is not read");
                    }
                }
                else
                {
                    if (temp != CurrentGear)
                    {
                        previousGear = temp;
                        temp = CurrentGear;
                    }
                }


                if (ClutchInput > 0.5f && i == 12)
                {
                    //Debug.Log("Gear in first");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = CurrentGear;
                    carController.ChangeGear(1);
                    carController.currentGear = CurrentGear;
                    carController.direction = 1;
                }
                else
                {
                    carController.changingGear = false;
                }

                if (ClutchInput > 0.5f && i == 13)
                {
                   //Debug.Log("Gear in second");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = CurrentGear;
                    carController.ChangeGear(2);
                    carController.currentGear = CurrentGear;
                }
                else
                {
                    carController.changingGear = false;
                }

                if (ClutchInput > 0.5f && i == 14)
                {
                   // Debug.Log("Gear in third");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = CurrentGear;
                    carController.ChangeGear(3);
                    carController.currentGear = CurrentGear;
                    steeringWheel.useForceFeedback = false;
                }
                else
                {
                    carController.changingGear = false;
                }

                if (ClutchInput > 0.5f && i == 15)
                {
                   // Debug.Log("Gear in forth");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = CurrentGear;
                    carController.ChangeGear(4);
                    carController.currentGear = CurrentGear;
                }
                else
                {
                    carController.changingGear = false;
                }

                if (ClutchInput > 0.5f && i == 16)
                {
                    //Debug.Log("Gear in fifth");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = CurrentGear;
                    carController.ChangeGear(5);
                    carController.currentGear = CurrentGear;
                }
                else
                {
                    carController.changingGear = false;
                }

                if (ClutchInput > 0.5f && i == 17)
                {
                    //Debug.Log("Gear in sixth");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = CurrentGear;
                    carController.ChangeGear(6);
                    carController.currentGear = CurrentGear;
                }
                else
                {
                    carController.changingGear = false;
                }

                if (ClutchInput > 0.5f && i == 18)
                {
                    //Debug.Log("Gear in reverse");
                    carController.ChangeGear(CurrentGear);
                    carController.changingGear = true;
                    carController.inputs.gearInput = -1;
                    carController.ChangeGear(0);
                    carController.currentGear = CurrentGear;
                    carController.canGoReverseNow = true;
                    carController.direction = -1;
                }
                else
                {
                    carController.canGoReverseNow = false;
                    carController.changingGear = false;
                }
            }
        }

        //if (ClutchIn == false)
        //{

        //    carController.enabled = false ;
        //}
        //else
        //{
        //    carController.enabled = true ;
        //}

    }

    public bool indicatorsOn = false;

    void CarController(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128)
            {
                if (i == 4)
                {
                    carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Right;
                    Debug.Log("Indicator right has been activated");
                }

                if (i == 5)
                {
                    carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Left;
                    Debug.Log("Indicator left has been activated");
                }

                if (i == 3)
                {
                    carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.All;
                    indicatorsOn = true;
                    Debug.Log("Both indicators have been activated");
                }
            }
        }
    }
}
