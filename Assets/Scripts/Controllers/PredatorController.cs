using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

//Controller for predator agent
public class PredatorController : AgentController
{
    [SerializeField]
    public int mNumberGeneticPoints = 100;
    [SerializeField]
    public PredatorAttributes mAttributes;
    public GOBScript mGOB;
    public NavMeshAgent mNavMeshAgent;
    public GameObject mCurrentTarget;
    public PredatorSensing mSensingManager;
    public GameObject mMate;
    public PredatorGeneticManager mGeneticManager;

    private void Awake()
    {
        mAgentSpawner = GameObject.FindGameObjectWithTag("PredatorSpawner").GetComponent<AgentSpawner>();
        mEnergyTag = "Prey";
        mRigidBody= GetComponent<Rigidbody>();
        mGOB = GetComponent<GOBScript>();
        mNavMeshAgent= GetComponent<NavMeshAgent>();
        mSensingManager = GetComponentInChildren<PredatorSensing>();
        mNavMeshAgent.speed = mAttributes.mSpeed;
    }

    private void Start()
    {
        mGeneticManager = new PredatorGeneticManager(mNumberGeneticPoints);
        Initalize();
    }

    private void Update()
    {
        Move(mCurrentTarget);
        UpdateEnergyLevels();
    }
    //Initializes a predators genetic attributes and resets energy level
    //This is important for when a new predator is spawned from two parent predators
    void Initalize()
    {
        mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        BindGeneticAttributesEvent();
        mGeneticManager.BroadcastAllAttributes();
    }
    //Updates the material of the predator to match their current GOB action
    public void ChangeAppearance(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }
    //Moves the agent
    //Specifically moves the agent towards the current target if the currentt target is set
    //Otherwise checks if the agent is moving towards a wall, if so it will turn the agent around
    public void Move(GameObject target)
    {
        if (mCurrentTarget != null)
        {
            if (target.CompareTag(mEnergyTag))
            {
                //This sets the predators target location as slightly in front of the prey. This accounts for predator stopping and stuttering when getting very close to agent
                //but not actually colliding with the agent
                mNavMeshAgent.SetDestination(mCurrentTarget.transform.position + mCurrentTarget.transform.forward * 2.0f);
            }
            else
            {
                mNavMeshAgent.SetDestination(mCurrentTarget.transform.position);
            }
            
        }
        else
        {
            //Speed is set based on genetic traits
            mNavMeshAgent.speed = mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Speed].mAttribute;
            if (mSensingManager.IsFacingWall())
            {
                mNavMeshAgent.SetDestination(transform.position + (5.0f * -transform.forward));
            }
            else
            {
                mNavMeshAgent.SetDestination(transform.position + transform.forward * 5.0f);
            }
        }
    }
    //Updates the attributes when a predator eats a prey agent
    protected override void ObjectConsumed(float val)
    {
        mAttributes.mTotalObjectsEatten++;
        mAttributes.mEnergyLevel += val;
    }
    //Updates the GOB system when a hunted prey agent is successfully eaten.
    //Returns the prey agent to object pool and deactivates it
    void OnPreyEatten(PreyController prey)
    {
        mGOB.mCurrentAction.StopAction();
        ObjectConsumed(prey.mAttributes.mEnergyGivenWhenEaten);
        prey.mAgentSpawner.ReturnAgentToPool(prey.gameObject);
        mGOB.mActionSuccessful = true;
        mGOB.SelectNewAction();
    }
    //Checks if the predator has no remaining energy
    //If so kills predator otherwise reduces energy with delta time
    protected override void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            KillPredator();
        }
        else
        {
            mAttributes.mEnergyLevel -= Time.deltaTime;
        }
    }
    //Called when predator has run out of energy
    //Returns the predator to it's object pool
    public void KillPredator()
    {
        StopAllCoroutines();
        mAgentSpawner.ReturnAgentToPool(gameObject);
    }

    //Spawns a new agent when two parents have successfully reproduced
    //The new agent is set to slightly behind the parent position
    public GameObject SpawnAgent(GameObject otherParent)
    {
        if (otherParent == null) 
        {
            return null;
        }
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        //Need null check because if max number of prey agents have been spawned SpawnAgent will return a null GameObject
        if (temp == null)
            return null;

        temp.transform.position = transform.position - transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        PredatorController controller = temp.GetComponent<PredatorController>();
        if (controller) 
        {
            controller.OnSpawn(this, otherParent);
        }
        return temp;
    }

    //Sets attributes back to starting values when action is completed or duration has expired
    public void ResetAttributes()
    {
        mNavMeshAgent.isStopped = false;
        mAttributes.mMateFound = false;
    }

    //Initializes the genetic attributes with half of the attribute points from one parent and the other half from the second parent
    //Slightly mutates the genetic attribute points by adding one point to a random attribute and removing one point from a random attribute
    //Initializes GOB behavior
    void OnSpawn(PredatorController parent1Controller, GameObject parent2)
    {
        mGeneticManager = new PredatorGeneticManager(GetParentGeneticPoints(parent1Controller, parent2));
        UpdatePoints();
        Initalize();
        MutateGeneticAttributes();
        mGOB.Initalize();
        mGOB.SelectNewAction();
        mAttributes.mCurrentGeneration = mAttributes.mCurrentGeneration + 1;
    }
    //Checks if the predator has collided with a prey agent
    //If it has and it is currently hunting then the prey is eaten.
    //If it is not hunting do not want to eat it
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mEnergyTag))
        {
            PreyController prey = collision.gameObject.GetComponent<PreyController>();
            if (prey)
            {
                if (mGOB.mCurrentAction != null && mGOB.mCurrentAction is HuntAction)
                {
                    OnPreyEatten(prey);
                }
            }
        }
    }
    //Binds genetic attributes action to the necessary functions
    //When an attributes value is changed the OnAttributeChanged action is called
    //This will update any listening functions with the correct new attribute value
    void BindGeneticAttributesEvent()
    {
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Speed].OnAttributeChanged += SetSpeed;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.AngularSpeed].OnAttributeChanged += SetAngularSpeed;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.MateSensingRadius].OnAttributeChanged += mSensingManager.SetMateSensingRadius;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.WallSensing].OnAttributeChanged += mSensingManager.SetWallSensingRadius;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.FarSensingAngle].OnAttributeChanged += mSensingManager.SetFarSensingAngle;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.FarSensingRadius].OnAttributeChanged += mSensingManager.SetFarSensingRadius;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.CloseSensingAngle].OnAttributeChanged += mSensingManager.SetCloseSensingAngle;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.CloseSensingRadius].OnAttributeChanged += mSensingManager.SetCloseSensingRadius;
    }
    //Sets the speed of the predator NavMeshAgent
    void SetSpeed(float speed)
    {
        mNavMeshAgent.speed = speed;
    }
    //Sets the angular speed of the predator NavMeshAgent
    void SetAngularSpeed(float speed)
    {
        mNavMeshAgent.angularSpeed = speed;
    }
    //Returns a list of float with the first half being from one parent and the second half from the other parent
    //This is how genetic traits (points) are passed onto children agents
    List<float> GetParentGeneticPoints(PredatorController parent1Controller, GameObject parent2)
    {
        PredatorController parent2Controller = parent2.GetComponent<PredatorController>();
        Debug.Assert(parent1Controller && parent2Controller);

        List<float> parent1GeneticPoints = parent1Controller.mGeneticManager.GetFirstHalfGenetics();
        List<float> parent2GeneticPoints = parent2Controller.mGeneticManager.GetSecondHalfGenetics();

        foreach (float f in parent2GeneticPoints)
        {
            parent1GeneticPoints.Add(f);
        }

        return parent1GeneticPoints;
    }
    //Increases and decreases a point from a random genetic attribute

    void MutateGeneticAttributes()
    {
        //Want to randomly increase one attribute
        while(true)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.ImproveAttribute(randomAttribute))
            {
                break;
            }
        }
        //Want to randomly decrease one attribute as long as it's point count is greater than 1
        while(true)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.DeteriorateAttribute(randomAttribute))
            {
                break;
            }
        }


    }
    //When getting the attribute genes from both parents the total may add up to above or below what the max number of points should
    //This function will randomly select attributes to either add a point or take a point away from to ensure it is equal to mNumberGeneticPoints
    //Only one of these while loops will run depending on whether we need to add or subtract if it is equal to mNumberGeneticPoints neither will run
    public void UpdatePoints()
    {
        while(mGeneticManager.mNumberOfStartingPoints < mNumberGeneticPoints)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.ImproveAttribute(randomAttribute))
            {
                mGeneticManager.mNumberOfStartingPoints++;
            }
        }

        while (mGeneticManager.mNumberOfStartingPoints > mNumberGeneticPoints)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.DeteriorateAttribute(randomAttribute))
            {
                mGeneticManager.mNumberOfStartingPoints--;
            }
        }
    }
}
