using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Assertions;

public class TerrainChecker : MonoBehaviour 
{
    /****** Public Members ******/

    public void SetTerrainChecker(float gndChkdist, Vector3 gndChkAngle, float obsChkDist)
    {
        _groundCheckingDistance = gndChkdist;
        _groundCheckingVector = gndChkAngle.normalized;
        _obstacleCheckingDistance = obsChkDist;
    }

    public bool CanMoveAhead()
    {
        return CheckIsGroundAhead() && !CheckIsObstacleAhead();
    }


    /****** Private Members ******/

    private Transform _checker;
    private float _groundCheckingDistance;
    private Vector3 _groundCheckingVector;
    private float _obstacleCheckingDistance;


    private void Start()
    {
        _checker = transform.parent;
        Debug.Assert(null != _checker, "Parent transform is not assigned.");
    }

    private bool CheckIsGroundAhead()
    {
        if (_checker == null) return false;

        _groundCheckingVector.x = _checker.localScale.x < 0 ? math.abs(_groundCheckingVector.x) : -math.abs(_groundCheckingVector.x);

        var groundHit = Physics2D.Raycast(_checker.position, _groundCheckingVector, _groundCheckingDistance, LayerMask.GetMask(Layer.Ground));

        return groundHit.collider != null;
    }

    private bool CheckIsObstacleAhead()
    {
        int direction = _checker.localScale.x < 0 ? 1 : -1;
         _groundCheckingVector.x = _checker.localScale.x < 0 ? math.abs(_groundCheckingVector.x) : -math.abs(_groundCheckingVector.x);


        var frontHit = Physics2D.Raycast(_checker.position, Vector3.right * direction, _obstacleCheckingDistance, LayerMask.GetMask(Layer.Obstacle, Layer.Ground));
        var bottomHit = Physics2D.Raycast(_checker.position, _groundCheckingVector, _groundCheckingDistance, LayerMask.GetMask(Layer.Obstacle));

        return frontHit.collider != null || bottomHit.collider != null;
    }


    private void OnDrawGizmosSelected()
    {
        if (_checker == null) return;

        _groundCheckingVector.x = _checker.localScale.x < 0 ? math.abs(_groundCheckingVector.x) : -math.abs(_groundCheckingVector.x);
        int direction = _checker.localScale.x < 0 ? 1 : -1;

        Gizmos.color = Color.red;
        Vector3 start = _checker.position;
        Vector3 end = start + _groundCheckingVector * _groundCheckingDistance;
        Gizmos.DrawLine(start, end);

        end = start + Vector3.right * direction * _obstacleCheckingDistance;
        Gizmos.DrawLine(start, end);
    }
}