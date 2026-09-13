using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float _viewRadius;

    [SerializeField, Range(0f, 360f)]
    private float _viewAngle;

    public LayerMask _targetMask ;
    public LayerMask _obstacleMask;

    private Transform _visibleTarget;

    public float ViewRadius => _viewRadius;
    public float ViewAngle => _viewAngle;

    public Transform VisibleTarget => _visibleTarget;

    private void Start()
    {
        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    private void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

        //Iterating through all the targets in the view radius
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (transform.position - this.transform.position).normalized;

            //Checking if the target is within the field of view angle
            if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(this.transform.position, target.position);

                //Checking if there are any obstacles between the agent and the target
                if (!Physics.Raycast(this.transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    // Target is visible
                    _visibleTarget = target;
                }
            }
        }
    }

    //Calculates the direction vector from an angle in degrees
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            //Adding the agent's current rotation to the angle, to rotate the angle when agent is rotated
            angleInDegrees += transform.eulerAngles.y;  
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private IEnumerator FindTargetWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindVisibleTargets();
    }
}
