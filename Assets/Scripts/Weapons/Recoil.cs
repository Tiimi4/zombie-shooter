using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    public Vector3 currentRotation;
    public Vector3 targetRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    
    [SerializeField, Range(0, 20)] private float snappinessMultiplier;
    [SerializeField, Range(0, 0.999f)] private float snappinessSmoothing;

    [SerializeField, Range(0, 1500)] private float returnSpeed;
    
    public Transform RecoilTransform { get; set; }


    public AnimationCurve xRecoilMultiplier;
    
    
    // Update is called once per frame
    void Update()   
    {
        
        if (targetRotation.sqrMagnitude < 0.001)
        {
            currentRotation = Vector3.MoveTowards(currentRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        }
        else
        {
            targetRotation = Vector3.MoveTowards(targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        }
        currentRotation = LerpDamp(currentRotation, targetRotation, snappinessSmoothing , Time.deltaTime * snappinessMultiplier);
               

        // _targetRotation = LerpDamp(_targetRotation, Vector3.zero, 1f - returnSpeedFactor , Time.deltaTime * returnSpeedMultiplier);
        //currentRotation = SlerpDamp(currentRotation, _targetRotation, 1f - snappinessFactor , Time.deltaTime * snappinessMultiplier);
        //RecoilTransform.localRotation = Quaternion.Euler(_currentRotation);
    }

    public void ApplyRotationToTransform()
    {
        Quaternion target = Quaternion.AngleAxis(currentRotation.y, Vector3.up) * RecoilTransform.parent.rotation;
        target *= Quaternion.Euler(Vector3.right * currentRotation.x);
        RecoilTransform.rotation = target;
           
     /*
        RecoilTransform.rotation =
            Quaternion.AngleAxis(currentRotation.y, Vector3.up) * RecoilTransform.parent.rotation ;
        RecoilTransform.Rotate(Vector3.right * currentRotation.x);
        */
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
    public static Vector3 SlerpDamp(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        return Vector3.Slerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }
    public static Vector3 LerpDamp(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        return Vector3.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }
}
