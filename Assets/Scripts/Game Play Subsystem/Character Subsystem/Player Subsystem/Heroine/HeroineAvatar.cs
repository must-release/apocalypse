using System.Collections;
using UnityEngine;

public class HeroineAvatar : MonoBehaviour, IPlayerAvatar
{
    /****** Public Members ******/

    public bool IsLoaded {get; set;}
    public bool IsAiming { set{}}

    public IEnumerator LoadWeaponsAndDots()
    {
        yield return null;
        IsLoaded = true;
    }

    public Transform GetTransform() { return transform; }

    // Show or hide character object
    public void ShowCharacter(bool value)
    {
        gameObject.SetActive(value);
    }

    public void RotateUpperBody(float rotateAngle)
    {
        
    }

    public void RotateUpperBody(Vector3 target)
    {
        
    }

    public void Aim(bool value)
    {

    }

    public float Attack()
    {
        return 0;
    }

    public void GetAnimators( out Animator lowerAnimator, out Animator upperAnimator)
    {
        lowerAnimator = _lowerBody.GetComponent<Animator>();
        upperAnimator = _upperBody.GetComponent<Animator>();
    }

    public void GetSpriteRenderers( out SpriteRenderer lowerSpriteRenderer, out SpriteRenderer upperSpriteRenderer )
    {
        lowerSpriteRenderer = _lowerBody.GetComponent<SpriteRenderer>();
        upperSpriteRenderer = _upperBody.GetComponent<SpriteRenderer>();
    }


    /****** Private Members ******/

    private const string _LowerBodyName = "Heroine Lower Body";
    private const string _UpperBodyName = "Heroine Upper Body";

    private Transform _lowerBody;
    private Transform _upperBody;
    

    private void Awake()
    {
        _lowerBody = transform.Find( _LowerBodyName );
        _upperBody = transform.Find( _UpperBodyName );
    }

    private void Start() 
    {
        StartCoroutine(LoadWeaponsAndDots()); 
    }
}
