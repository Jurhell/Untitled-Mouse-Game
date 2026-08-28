using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject _target;

    [SerializeField, Min(0.5f), Tooltip("How big the agent's detection radius is.")]
    private float _agentDetectionRadius = 10f;

    [SerializeField, Min(0.5f), Tooltip("How far the agent's line of sight is.")]
    private float _losDistance = 20f;

    [SerializeField]
    private float _losAngle = 45f;

    private NavMeshAgent _agent;

    private EState _currentState = EState.IDLE;

    private bool _isHeadAgent = false;
    private bool _isChasing = false;

    private UnityEvent _playerDetectedEvent = new UnityEvent();

    enum EState
    {
        IDLE,
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

        TransitionTo(EState.PURSUE);

        //Line of sight
        LineOfSightCheck();

        //When player is spotted alert nearby agents
        //If player is spotted by head agent alert all agents

        //Target alert radius
        //Search last spotted area

        //Idle state should only be active at the beginning of the game
        if (_currentState == EState.IDLE)
        {
            //play idle animation
            return;
        }
        else if (_currentState == EState.PURSUE)
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!_agent.enabled || !_isChasing)
            return;

        Chase();
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
            Debug.Log("Player is close");
        }

        RaycastHit hit;
        
        //Casting a ray that serves as the agent's line of sight
        if (Physics.Raycast(_agent.transform.position, _agent.transform.forward, out hit, _losDistance))
        {
            //If raycast has hit the player
            if (hit.collider.gameObject == _target)
            {
                Debug.Log("Ray has hit player");
            }
            //If raycast has not hit the player
            else
            {
                Debug.Log("Ray has not hit player");
            }
        }
        else
        {
            Debug.Log("Out of Sight2");
        }

        Vector3 directionToTarget = _target.transform.position - _agent.transform.position;
        Vector3 agentForward = _agent.transform.forward;

        //Calculating the angle between the agent's forward direction and the direction to the target
        float angle = Vector3.SignedAngle(directionToTarget, agentForward, Vector3.up);

        //Checking if the angle is within the line of sight angle and agent is close enough to the target
        if (angle < _losAngle && angle > - 1 * _losAngle && withinDistance)
        {
            Debug.Log("In Sight");
            _isChasing = true;
        }
        else
        {
            Debug.Log("Not in Sight");
            _isChasing = false;
        }
    }

    private bool RadiusCheck()
    {
        float seekMagnitude = (_target.transform.position - _agent.transform.position).magnitude;

        //Checking if seek magnitude is less than the product of the agent's avoidance radius and detection radius
        if (seekMagnitude <= _agent.radius * _agentDetectionRadius)
        {
            _isChasing = true;
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
}