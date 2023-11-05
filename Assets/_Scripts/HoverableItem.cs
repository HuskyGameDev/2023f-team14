using System.Collections;
using System.Collections.Generic;
using System.Numerics;
//using UnityEditor.UIElements;
using UnityEngine;

public class HoverableItem : MonoBehaviour
{
    private UnityEngine.Vector3 startPosition;
    private UnityEngine.Vector3 endPosition;

    private UnityEngine.Vector3 startRotation;
    private UnityEngine.Vector3 endRotation;
    
    [SerializeField]
    private float upDownSpeed = 3f;
    private float elapsedTime;

    private bool goingUp = true;
    [SerializeField]
    private AnimationCurve positionCurve;

    [SerializeField]
    private float height = 4;

    [SerializeField]
    private float rotationSpeed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        endPosition.x = startPosition.x;
        endPosition.z = startPosition.z;
        endPosition.y = startPosition.y + height;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        
        elapsedTime += Time.deltaTime;
        float completion = elapsedTime / upDownSpeed;
        float curveValue = positionCurve.Evaluate(completion);
        transform.position = UnityEngine.Vector3.Lerp(startPosition, endPosition, curveValue);
        transform.Rotate(UnityEngine.Vector3.up, rotationSpeed);
        
        if(curveValue >= 1){
            if(goingUp){
                startPosition.y = endPosition.y;
                endPosition.y = startPosition.y - height;
                goingUp = false;
            }else{
                startPosition.y = endPosition.y;
                endPosition.y = startPosition.y + height;
                goingUp = true;
            }
            elapsedTime = 0;
            rotationSpeed *= -1;
        }
    }


}
