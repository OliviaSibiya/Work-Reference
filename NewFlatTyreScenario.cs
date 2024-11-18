using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFlatTyreScenario : MonoBehaviour
{
    public WheelCollider flatWheel;
    public NewScenario manager;
    public RCC_CarControllerV3 carController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (manager.scenarioItems.activeInHierarchy == false)
        //{
        //    carController.driftingNow = false;
        //    flatWheel.radius = 0.4f;
        //}

        if (manager.scenarioItems.activeInHierarchy == false)
        {
            flatWheel.radius = 0.36f;
            carController.driftingNow = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            carController.driftingNow = true;
            flatWheel.radius = 0.7f;
        }

        //if (other.CompareTag("Player") && carController.speed < 5f)
        //{
        //    carController.driftingNow = false;
        //    flatWheel.radius = 0.4f;
        //}

        //if (other.CompareTag("Player") && manager.passed || manager.failed)
        //{
        //    carController.driftingNow = false;
        //    flatWheel.radius = 0.36f;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && manager.passed || manager.failed)
        {
           // manager.failed = true;
           // manager.UIPopup();
            carController.driftingNow = false;
            flatWheel.radius = 0.36f;
            Debug.LogWarning("THIS IS WORKING:" + "TRIGGER EXIT");
        }
    }
}
