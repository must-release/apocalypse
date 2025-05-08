using UnityEngine;
using LayerEnums;
using Unity.Mathematics;

public class TerrainChecker : MonoBehaviour 
{
    private Transform checker;
    private float groundCheckingDistance;
    private Vector3 groundCheckingVector;
    private float obstacleCheckingDistance;
    private LayerMask groundLayer, obstacleLayer;
    private ContactFilter2D groundFilter, obstacleFilter;
    RaycastHit2D[] groundHits, obstaclesHits;


    private void Start() {
        checker = transform.parent;

        groundLayer = LayerMask.GetMask(LAYER.GROUND);
        obstacleLayer = LayerMask.GetMask("Default", LAYER.GROUND);

        // Set up filters
        groundFilter = new ContactFilter2D();
        groundFilter.SetLayerMask(groundLayer);
        groundFilter.useTriggers = false;

        obstacleFilter = new ContactFilter2D();
        obstacleFilter.SetLayerMask(obstacleLayer);
        obstacleFilter.useTriggers = false;

        // Hit buffers
        groundHits = new RaycastHit2D[1];
        obstaclesHits = new RaycastHit2D[1];
    }

    public void SetTerrainChecker(float gndChkdist, Vector3 gndChkAngle, float obsChkDist)
    {
        groundCheckingDistance = gndChkdist;
        groundCheckingVector = gndChkAngle.normalized;
        obstacleCheckingDistance = obsChkDist;
    }

    // Check if there is ground ahead, and no obstacles
    public bool CanMoveAhead()
    {
        return IsGroundAhead() && !IsObstacleAhead();
    }

    private bool IsGroundAhead()
    {
        groundCheckingVector.x = checker.localScale.x > 0 ? math.abs(groundCheckingVector.x) : -math.abs(groundCheckingVector.x);

        int hitCount = Physics2D.Raycast(checker.position, groundCheckingVector, groundFilter, 
            groundHits, groundCheckingDistance);

        return hitCount > 0;
    }

    private bool IsObstacleAhead()
    {
        int direction = checker.localScale.x > 0 ? 1 : -1;

        int hitCount = Physics2D.Raycast(checker.position, Vector3.right * direction, obstacleFilter, 
            obstaclesHits, obstacleCheckingDistance);

        return hitCount > 0;
    }


    private void OnDrawGizmosSelected()
    {
        if(checker == null) return;

        groundCheckingVector.x = checker.localScale.x > 0 ? math.abs(groundCheckingVector.x) : -math.abs(groundCheckingVector.x);
        int direction = checker.localScale.x > 0 ? 1 : -1;

        Gizmos.color = Color.red;
        Vector3 start = checker.position;
        Vector3 end = start + groundCheckingVector * groundCheckingDistance;
        Gizmos.DrawLine(start, end);

        end = start + Vector3.right * direction * obstacleCheckingDistance;
        Gizmos.DrawLine(start, end);
    }
}