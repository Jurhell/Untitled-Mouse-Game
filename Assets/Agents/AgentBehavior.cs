using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentBehavior : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private GameObject _target;

    [SerializeField, Min(0.5f), Tooltip("How big the agent's detection radius is.")]
    private float _agentDetectionRadius = 10f;

    [SerializeField, Min(0.5f), Tooltip("How far the agent's line of sight is.")]
    private float _losDistance = 20f;

    [SerializeField, Min(0.5f), Tooltip("How wide the agent's line of sight is.")]
    private float _losAngle = 45f;

    [Space, Header("Search")]
    [SerializeField, Min(1f), Tooltip("How long the agent will search for the target.")]
    private float _searchTime = 5f;

    private float _searchTimer = 0f;
    private Vector3 _lastKnownPosition;

    private NavMeshAgent _agent;

    private EState _currentState = EState.WORKING;

    private bool _isHeadAgent = false;
    private bool _isSearching = false;
    private static bool _targetIsFound = false;

    private static List<NavMeshAgent> _eyesOnTarget = new List<NavMeshAgent>();

    private UnityEvent _playerDetectedEvent = new UnityEvent();

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
        LineOfSightCheck();

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

            return;
        }
        else if (_currentState == EState.SEARCH)
        {
            //When player is lost, agent stops pursuing
            _eyesOnTarget.Remove(_agent);
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

    private void LineOfSightCheck()
    {
        bool withinDistance = false;


        //Checking if player is within line of sight distance
        if (Vector3.Distance(_agent.transform.position, _target.transform.position) <= _losDistance)
        {
            withinDistance= true;
            //Debug.Log("Player is close");
        }
        else
        {
            withinDistance= false;
            //Debug.Log("Player is far");
        }

        RaycastHit hit;
        
        //Casting a ray that serves as the agent's line of sight
        if (Physics.Raycast(_agent.transform.position, _agent.transform.forward, out hit, _losDistance))
        {
            //If raycast has hit the player
            if (hit.collider.gameObject == _target)
            {
                //Adding the agent to the list of agents that have eyes on the target
                if (!_eyesOnTarget.Contains(_agent))
                    _eyesOnTarget.Add(_agent);

                //Storing player's position for when the player is lost
                _lastKnownPosition = hit.collider.transform.position;

                Debug.Log("Ray has hit player");
            }
            //If raycast has hit something other than the player after target was previously found...
            else if (hit.collider.gameObject != _target && _targetIsFound)
            {
                //Remove the agent from the list of agents that have eyes on the target
                if (_eyesOnTarget.Contains(_agent))
                    _eyesOnTarget.Remove(_agent);

                Debug.Log("Player is lost");
            }
            //If raycast has not hit the player
            else
            {
                Debug.Log("Ray has not hit player");
            }
        }
        else
        {
            //Debug.Log("Out of Sight2");
        }

        Vector3 directionToTarget = _target.transform.position - _agent.transform.position;
        Vector3 agentForward = _agent.transform.forward;

        //Calculating the angle between the agent's forward direction and the direction to the target
        float angle = Vector3.SignedAngle(directionToTarget, agentForward, Vector3.up);

        //Checking if the angle is within the line of sight angle and agent is close enough to the target
        if (angle < _losAngle && angle > - 1 * _losAngle && withinDistance)
        {
            //If so, begin chasing the target
            TransitionTo(EState.PURSUE);

            _isSearching = false;

            Debug.Log("Chasing");
        }
        else
        {
            Debug.Log("Not Chasing");
        }
    }

    private bool RadiusCheck()
    {
        float seekMagnitude = (_target.transform.position - _agent.transform.position).magnitude;

        //Checking if seek magnitude is less than the product of the agent's avoidance radius and detection radius
        if (seekMagnitude <= _agent.radius * _agentDetectionRadius)
        {
            _targetIsFound = true;
            return true;
        }
        else
            return false;
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
        //Having agent search for the target at the last known position
        _agent.destination = _lastKnownPosition;


        //When agent enters the radius of the last known position, begin searching
        //Agent rotates to search for the target

        if (_agent.transform.position == _lastKnownPosition)
        {
            _searchTimer += Time.deltaTime;
            _isSearching = true;
        }

        if (_isSearching)
        {
            //StartCoroutine(Wait(() => { _agent.transform.Rotate(0, UnityEngine.Random.Range(-180f, 180f), 0); }, 1f));
            Debug.Log("Searching");
        }

        //If the agent hasn't found the target after searching for a set amount of time...
        if (_searchTimer == _searchTime)
        {
            //...Agent will end search and return to its normal behavior
            TransitionTo(EState.WORKING);
            _searchTimer = 0f;
            _isSearching = false;
        }
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