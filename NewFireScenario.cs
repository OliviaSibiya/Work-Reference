using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFireScenario : MonoBehaviour
{
    public ParticleSystem fireParticles;
    public GameObject particleObject;
    public NewScenario manager;
    public RCC_CarControllerV3 carController;

    // Start is called before the first frame update
    void Start()
    {
        fireParticles.Stop();
        particleObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy == false)
        {
            fireParticles.Stop();
            particleObject.SetActive(false);
        }

        if (manager.scenarioItems.activeInHierarchy == false)
        {
            fireParticles.Stop();
            particleObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireParticles.Play();
            particleObject.SetActive(true);
        }

        if (other.CompareTag("Player") && manager.passed || manager.failed)
        {
            fireParticles.Stop();
            particleObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && manager.passed || manager.failed)
        {
            fireParticles.Stop();
            particleObject.SetActive(false);
        }
    }
}
