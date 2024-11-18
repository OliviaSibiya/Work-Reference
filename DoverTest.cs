using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoverTest : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;
    //---------LOGITECH CONFIGURE---------------//
    //INDEX NAMES
    int a, b, c, d;
    //---------------end of logitech-----------//

    [SerializeField] private PlayAfterVoice playAfterVO;
    [SerializeField] private AudioClip[] voList;
    public float NextDelay = 1.0f;
    private float delay;

    // Test 1 Group - Circles
    [SerializeField] private Transform[] circleList;
    [SerializeField] private Transform[] colourList;
    

    // Test 2 Group - Audio
    [SerializeField] private AudioClip[] audioList;
    [SerializeField] public AudioSource audioSource;
    

    // Test 3 Group - Pedals
    [SerializeField] private Transform[] pedalList;
    [SerializeField] private Transform pedalColour;
    

    public bool test1Active;
    public bool test2Active;
    public bool test3Active;
    public bool phase1Active;
    public bool phase2Active;

    private int test1Length;
    private int test2Length;
    private int test3Length;
    private int phase1Length;
    private int phase2Length;

    private int testIncrement;
    private int phaseIncrement;

    private bool audio1Active;
    private bool audio2Active;
    private bool pedal1Active;
    private bool pedal2Active;
    private bool inputActive;

    private bool test2VO; // 0
    private bool test3VO; // 1
    private bool phase2VO; // 2

    //private float delay;

    private Coroutine keyRoutine;

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

        LogitechGSDK.LogiSteeringInitialize(false);

        test1Length = 10;
        test2Length = 4;
        test3Length = 4;

        phase1Length = test1Length + test2Length + test3Length;
        phase2Length = 10;

        inputActive = true;

        test1Active = false;
        test2Active = false;
        test3Active = false;

        phase1Active = true;
        phase2Active = false;

        audio1Active = false;
        audio2Active = false;

        test2VO = false;
        test3VO = false;

        delay = NextDelay;
    }

    public void StartExperience()
    {
        test1Active = true;
        test2Active = false;
        test3Active = false;
        phase1Active = true;
        phase2Active = false;
        testIncrement = 0;
        phaseIncrement = 0;
        Test1();
    }

    private void Test1()
    {
        int circleIndex = Random.Range(0, 10);
        int colourIndex = Random.Range(0, 5);

        colourList[colourIndex].gameObject.SetActive(true);
        colourList[colourIndex].position = circleList[circleIndex].position;
    }

    private void Test2()
    {
        int index = Random.Range(0, 2);
        audio1Active = index == 0;
        audio2Active = index == 1;

        audioSource.clip = audioList[index];
        audioSource.Play();
    }

    private void Test3()
    {
        int index = Random.Range(0, 2);
        pedal1Active = index == 0;
        pedal2Active = index == 1;

        pedalColour.position = pedalList[index].transform.position;
        pedalColour.gameObject.SetActive(true);
    }

    private void RandomTest()
    {
        int index = Random.Range(0, 3);

        switch (index)
        {
            case 0:
                Test1();
                break;
            case 1:
                Test2();
                break;
            case 2:
                Test3();
                break;
            default:
                Debug.Log("SWITCH ERROR");
                break;
        }
    }

    private IEnumerator DoNextDelayCo()
    {
        inputActive = false; // KICK THE BABY

        foreach (Transform item in colourList)
        {
            item.gameObject.SetActive(false);
        }

        audioSource.Stop();
        audio1Active = false;
        audio2Active = false;

        pedalColour.gameObject.SetActive(false);
        pedal1Active = false;
        pedal2Active = false;

        if (test1Active && testIncrement == test1Length)
        {
            //test2VO = false;
            audioSource.clip = voList[0];
            audioSource.Play();
            NextDelay = audioSource.clip.length + 0.5f;
        }

        if (test2Active && testIncrement == test2Length)
        {
            test3VO = false;
            audioSource.clip = voList[1];
            audioSource.Play();
            NextDelay = audioSource.clip.length + 0.5f;
        }

        if (test3Active && phaseIncrement == phase1Length)
        {
            phase2VO = false;
            audioSource.clip = voList[2];
            audioSource.Play();
            NextDelay = audioSource.clip.length + 0.5f;
        }

        yield return new WaitForSeconds(NextDelay);

        if (phase1Active)
        {            
            DoNextPhase1();
        }
        else if (phase2Active)
        {
            Debug.Log("Phase 2 Active");
            DoNextPhase2();
        }
    }

    private void DoNextPhase1()
    {
        if (phase1Active && phaseIncrement <= phase1Length)
        {
            //if (phaseIncrement >= phase1Length)
            //{
            //    phase1Active = false;
            //    phaseIncrement = 0;
            //}

            if (test1Active)
            {
                if (testIncrement < test1Length)
                {
                    testIncrement++;
                    phaseIncrement++;
                    Test1();
                }
                else
                {
                    test1Active = false;
                    test2Active = true;
                    NextDelay = delay;
                    testIncrement = 0;
                    Test2();
                }
            }
            else if (test2Active)
            {
                if (testIncrement < test2Length)
                {
                    testIncrement++;
                    phaseIncrement++;
                    Test2();
                }
                else
                {
                    test2Active = false;
                    test3Active = true;
                    NextDelay = delay;
                    testIncrement = 0;
                    Test3();
                }
            }
            else if (test3Active)
            {
                if (testIncrement < test3Length)
                {
                    testIncrement++;
                    phaseIncrement++;
                    Test3();
                }
                else
                {
                    test3Active = false;
                    phase1Active = false;
                    phase2Active = true;
                    NextDelay = delay;
                    testIncrement = 0;
                    phaseIncrement = 0;
                    RandomTest();
                }
            }
        }

        inputActive = true;
    }

    private void DoNextPhase2()
    {
        if (phase2Active && phaseIncrement <= phase2Length)
        {
            Debug.Log("Still in Phase 2");
            if (phaseIncrement >= phase2Length)
            {
                Debug.Log("END YAYYYYYYYYYYYYYYYYYYYYYYYY");
                // END
                // Play final voice over (Congratulations VO)
                // Yield until VO is done
                // Auto-load Main Menu
                playAfterVO.PlayVO(voList[3], 1.0f);

                ////SceneManager.LoadScene("MenuLayout");
            }
            else
            {
                Debug.Log("Random Test");
                phaseIncrement++;
                RandomTest();
            }
        }

        inputActive = true;
    }

    void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(a) && inputActive)
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(a);

            ButtonSelect(rec);
        }
    }

    public void ButtonSelect(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        if (rec.rglSlider[0] < 0) // This is the clutch 
        {
            if (pedal1Active)
            {
                keyRoutine = StartCoroutine(DoNextDelayCo());
            }
            
            Debug.Log("Clutch");
        }

        if (rec.lY < 0) // This is the throttle
        {
            if (pedal2Active)
            {
                keyRoutine = StartCoroutine(DoNextDelayCo());
            }

            Debug.Log("Throttle");
        }

        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128)// Access to Logitech Steeringwheel
            {
                if (i == 0) // This is for the colour red.(This is X)
                {
                    if (colourList[3].gameObject.activeSelf)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("X BUTTON");
                }

                if (i == 1) // This is for the colour green. (This is square)
                {
                    if (colourList[1].gameObject.activeSelf)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("SQUARE BUTTON");
                }

                if (i == 2) // This is for the colour blue. (This is O)
                {
                    if (colourList[2].gameObject.activeSelf)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("O BUTTON");
                }

                if (i == 3) // This is for the colour orange. (This is triangle)
                {
                    if (colourList[4].gameObject.activeSelf)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("TRIANGLE BUTTON");
                }

                if (i == 23) // This is for the colour white.(This is the return button with red dial)
                {
                    if (colourList[0].gameObject.activeSelf)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("RETURN BUTTON");
                }

                if (i == 7) // This is the L2 on the steering wheel. This is for the sound indicators.
                {
                    if (audio1Active)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("L2 BUTTON");
                }

                if (i == 6) // This is the R2 on the steering wheel. This is for the sound indicators.
                {
                    if (audio2Active)
                    {
                        keyRoutine = StartCoroutine(DoNextDelayCo());
                    }

                    Debug.Log("R2 BUTTON");
                }
            }
        }
    }
}
