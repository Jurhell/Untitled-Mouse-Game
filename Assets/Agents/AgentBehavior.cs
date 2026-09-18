using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    [Space, Header("Search")]
    [SerializeField, Min(1f), Tooltip("How long the agent will search for the target.")]
    private float _searchTime = 5f;

    [SerializeField, Tooltip("The GameObject representing the search point.")]
    private GameObject _searchPoint;

    private float _searchTimer = 0f;
    private Vector3 _lastKnownPosition;

    private GameObject _searchPointInstance;

    private NavMeshAgent _agent;

    private EState _currentState = EState.WORKING;

    private bool _isHeadAgent = false;
    private bool _isSearching = false;
    private bool _SearchPointCreated = false;
    private bool _searchTriggered = false;
    private static bool _targetIsFound = false;

    private static List<NavMeshAgent> _eyesOnTarget = new List<NavMeshAgent>();

    enum EState
    {
        WORKING,
        ROAM,
        PURSUE,
        SEARCH,
        END
    }

    // Start is called before the first frame update
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_agent.enabled)
            return;

        //Line of sight
        //LineOfSightCheck();

        //Target alert radius

        //When player runs behind something or out of sight, agent stops pursuing player
        //Search last spotted area

        //When head agent is no longer in pursuit, all agents stop pursuing
        //Farther agents can end pursuit on their own if they lose sight of the target

        //Idle state should only be active at the beginning of the game
        if (_currentState == EState.WORKING)
        {
            //Agent performs its normal behavior
            return;
        }
        else if (_currentState == EState.PURSUE)
        {
            //When player is spotted alert all agents
            _targetIsFound = true;

            //The rest of PURSUE behavior is handled in FixedUpdate to avoid issues with NavMeshAgent movement

            return;
        }
        else if (_currentState == EState.SEARCH)
        {
            //Agent begins searching
            Search();

            return;
        }
    }

    private void FixedUpdate()
    {
        //Guard Clause
        if (!_agent.enabled || !_targetIsFound)
            return;
        Debug.Log(_eyesOnTarget.Count);

        Chase();

        //If no agents are in pursuit, the target is no longer found
        if (_eyesOnTarget.Count == 0)
            _targetIsFound = false;
    }

    private void TransitionTo(EState state)
    {
        //Guard Clause
        if (_currentState == EState.END || state == _currentState)
            return;

        _currentState = state;
    }

    private void Chase()
    {
        //Storing direction to the target
        Vector3 targetDirection = _target.transform.position - _agent.transform.position;

        //Storing variables needed for chase behavior
        float relativeHead = Vector3.Angle(_agent.transform.forward, _agent.transform.TransformVector(_target.transform.forward));
        float toTarget = Vector3.Angle(_agent.transform.forward, _agent.transform.TransformVector(targetDirection));

        //Storing force to have agent look ahead of the player
        float lookAhead = targetDirection.magnitude / (_agent.speed + _target.GetComponent<MouseController>().Speed);

        //Having agent chase
        _agent.destination = _target.transform.position + _target.transform.forward * lookAhead;
    }

    private void Search()
    {
        if (!_SearchPointCreated)
        {
            //Creating a search point at the last known position of the target
            _searchPointInstance = Instantiate(_searchPoint, _lastKnownPosition, Quaternion.identity);
            _SearchPointCreated = true;
        }

        //Having the agent move to the search point
        _agent.destination = _searchPointInstance.transform.position;

        //While agent is searching
        if (_isSearching)
        {
            _searchTimer += Time.deltaTime;

            if (!_searchTriggered)
            {
                //Having the agent rotate in a random direction every 3 seconds while searching,
                //and preventing the agent from rotating again until the 3 seconds have passed
                StartCoroutine(Wait(() => { _agent.transform.Rotate(0, UnityEngine.Random.Range(-180f, 180f), 0); _searchTriggered = false; }, 3f));
                _searchTriggered = true;
            }
            
            Debug.Log("Searching");
        }

        //If the agent hasn't found the target after searching for a set amount of time...
        if (_searchTimer == _searchTime)
        {
            //...Agent will end search and return to its normal behavior
            TransitionTo(EState.WORKING);
            Debug.Log("Test");
            EndSearch();
        }
    }

    private void EndSearch()
    {
        //Resetting search variables
        _searchTimer = 0f;
        _isSearching = false;
        _SearchPointCreated = false;

        if (_searchPointInstance != null)
            Destroy(_searchPointInstance);
    }

    //What functionality the agent should perform when it spots the target
    public void TargetIsSpotted(Transform target)
    {
        //If agent is not already in the list of agents that have eyes on the target
        if (!_eyesOnTarget.Contains(_agent))
        {
            //Add the agent to the list
            _eyesOnTarget.Add(_agent);

            EndSearch();

            //If the agent is not already in the PURSUE state, transition to it
            if (_currentState != EState.PURSUE)
                TransitionTo(EState.PURSUE);
        }

        //Saving the position of the target for when the agent loses sight of it
        _lastKnownPosition = target.position;
    }

    //What functionality the agent should perform when it loses sight of the target
    public void TargetIsLost()
    {
        //If the no longer has eyes on the target
        if (_eyesOnTarget.Contains(_agent))
        {
            //Remove them from the list
            _eyesOnTarget.Remove(_agent);

            //If the agent is not already in the SEARCH state, transition to it
            if (_currentState != EState.SEARCH)
                TransitionTo(EState.SEARCH);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Search Point"))
            _isSearching = true;
        Debug.Log("Searching");
    }

    //private void OnDrawGizmos()
    //{
    //    //Drawing a sphere to represent the agent's detection radius
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, _agentDetectionRadius * _agent.radius);
    //    //Drawing a line to represent the agent's line of sight
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, _hit.point);
    //    //
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(_hit.point, 0.1f);
    //}

    private IEnumerator Wait(Action callback, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        callback();
    }
}