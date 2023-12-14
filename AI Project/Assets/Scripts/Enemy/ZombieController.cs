using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public string coreTag = "Core"; // Tag van het doelobject
    public float damageInterval = 2f; // Aantal seconden tussen elke schade
    public float damageRange = 3f; // Maximale afstand waarbinnen schade wordt toegebracht
    private NavMeshAgent navMeshAgent;
    private Transform target;
    private float timer = 0f;
    public bool bruteZombie = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        FindCore();

        if (target == null)
        {
            Debug.LogError("Core object not found with tag: " + coreTag);
        }
        else
        {
            SetDestination();
        }
    }

    void FindCore()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag(coreTag);
        if (coreObject != null)
        {
            target = coreObject.transform;
        }
    }

    void SetDestination()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Optioneel: vernieuw de bestemming als het doelobject beweegt
            if (Vector3.Distance(transform.position, target.position) > 1.0f)
            {
                SetDestination();
            }

            // Voer schade alleen uit als de zombie dichtbij genoeg is
            float distanceToCore = Vector3.Distance(transform.position, target.position);
            if (distanceToCore <= damageRange)
            {
                timer += Time.deltaTime;
                if (timer >= damageInterval)
                {
                    DamageCore();
                    timer = 0f;
                }
            }
        }
        else
        {
            // Stop de zombie als het doelobject ontbreekt
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
        }
    }

    void DamageCore()
    {
        if(bruteZombie) CoreHealth.coreStaticHealth -= 20;
        else CoreHealth.coreStaticHealth -= 5;
        Debug.Log("De Core is beschadigd!");
    }
}
