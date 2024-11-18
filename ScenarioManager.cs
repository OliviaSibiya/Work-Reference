using TMPro;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{ // This script is designed to have the vehicle go back to its original state after the player has completed the scenarios

    public NewFlatTyreScenario tyreScenario;
    public NewFireScenario fireScenario;
    public NewBrakeScenario brakeScenario;
   // public NewScenario manager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (manager.scenarioItems.activeInHierarchy != true)
        //{
        //    tyreScenario.flatWheel.radius = 0.36f;
        //    fireScenario.particleObject.SetActive(false);
        //    for (int i = 0; i <  brakeScenario.wheels.Length; i++)
        //    {
        //        if (brakeScenario.wheels[i])
        //        {
        //            brakeScenario.wheels[i].canBrake = true;
        //            brakeScenario.wheels[i].canPower = true;
        //            brakeScenario.wheels[i].brakingMultiplier = 1;
        //        }
        //    }

        //}

        if (tyreScenario.manager.passed == true || tyreScenario.manager.failed == true)
        {
            tyreScenario.flatWheel.radius = 0.36f;
            tyreScenario.carController.driftingNow = false;
        }

        if (fireScenario.manager.passed == true || fireScenario.manager.failed == true)
        {
            fireScenario.particleObject.SetActive(false);
        }

        if (brakeScenario.scenarioManager.passed == true || brakeScenario.scenarioManager.failed == true)
        {
            for (int i = 0; i < brakeScenario.wheels.Length; i++)
            {
                if (brakeScenario.wheels[i])
                {
                    brakeScenario.wheels[i].canBrake = true;
                    brakeScenario.wheels[i].canPower = true;
                    brakeScenario.wheels[i].brakingMultiplier = 1;
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {

    }
    public void OnTriggerStay(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }
    
}
