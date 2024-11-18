using UnityEngine;

public class LogitechAutomatic : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;

    public RCC_Settings carSettings;
    public RCC_CarControllerV3 carController;
    public RCC_LogitechSteeringWheel steeringWheel;

    public float xAxis, GasInput, BreakInput, ClutchInput;
    public bool HShift = true;
    bool isInGear;
    public int currentGear;


    // Start is called before the first frame update
    void Start()
    {
        //carSettings = GetComponent<RCC_Settings>();
        carController = GetComponent<RCC_CarControllerV3>();


        LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER);
        LogitechGSDK.LogiPlayDamperForce(0, 70);
    }

    // Update is called once per frame
    void Update()
    {
        carSettings.useAutomaticGear = true;
        carController.canGoReverseNow = false;

        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
            CarController(rec);
            HShifter(rec);

            steeringWheel.useHShifter = HShift;
            if (HShift == steeringWheel.useHShifter)
            {
                Debug.Log("Gear being used");
            }

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

            #region Manual pedals input
            //        //xAxis = rec.lX / 32767; //-1 0 1

            //        //if (rec.lY > 0)
            //        //{
            //        //    GasInput = 0;
            //        //}
            //        //else if (rec.lY < 0)
            //        //{
            //        //    GasInput = rec.lY / -32767;
            //        //}

            //        //if (rec.lRz > 0)
            //        //{
            //        //    BreakInput = 0;
            //        //}
            //        //else if (rec.lRz < 0)
            //        //{
            //        //    BreakInput = rec.lRz / -10240;
            //        //}

            //        //if (rec.rglSlider[0] > 0) // Clutch input
            //        //{
            //        //    ClutchInput = 0;
            //        //}
            //        //else if (rec.rglSlider[0] < 0)
            //        //{
            //        //    ClutchInput = rec.rglSlider[0] / -32768;
            //        //}

            //        //BreakInput = carController.brakeInput;
            //        //if (BreakInput == rec.lRz / -10240)
            //        //{
            //        //    carController.brakeInput = 0;
            //        //    // carController.speed = 0;
            //        //    carController.cutGas = true;
            //        //    Debug.Log("Breaks are working");
            //        //}
            #endregion

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
    public bool indicatorsOn;

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

    void HShifter(LogitechGSDK.DIJOYSTATE2ENGINES shifter)
    {
        isInGear = false;
        steeringWheel.atNGear = isInGear;
        if (isInGear == steeringWheel.atNGear)
        {
            Debug.Log("Gear is being fed from other feed");
        }

        for (int i = 0; i < 128; i++)
        {
            if (shifter.rgbButtons[i] == 128)
            {
                if (i == 12)
                {
                    carSettings.autoReverse = false;
                    isInGear = true;
                    carSettings.LogiSteeringWheel_handbrakeKB = 0;
                    carController.engineRunning = false;
                    Debug.Log("Automatic in Park (P)");
                    carController.GearShiftDown();
                    
                }

                if (i == 14)
                {
                    carSettings.autoReverse = false;
                    carController.canGoReverseNow = false;
                    carController.engineRunning = true;
                    carController.direction = 1;
                    carSettings.useAutomaticGear = true;
                    Debug.Log("Automatic in Drive (D)");
                    carController.GearShiftUp();
                    isInGear = true;
                    if (carController.speed == 0)
                    {
                        carController.currentGear++;
                        carController.GearShiftUp();
                    }
                }

                if (i == 15)
                {
                    Debug.Log("Automatic in Neutral (N)");
                    isInGear = true;
                    carController.throttleInput = 0;
                    carController.cutGas = true;
                    
                }

                if (i == 18)
                {
                    Debug.Log("Automatic in Revers (R)");
                    carSettings.autoReverse = true;
                    carController.canGoReverseNow = true;
                    carController.direction = -1;
                    carController.currentGear = -1;
                    carController.GearShiftDown();
                    if (carController.speed <= 0)
                    {
                        carController.currentGear--;
                    }

                    //carController.brakeInput = .9f;
                    isInGear = true;
                    carController.engineRunning = true;
                    carSettings.useAutomaticGear = false;
                }
                else
                {
                    isInGear = false;

                    Debug.Log("Gear is not read");
                }

                currentGear = i;
            }
        }
    }
}


