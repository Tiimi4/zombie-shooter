using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField, Range(0, 0.999f)] private float snappinessFactor;
    [SerializeField, Range(0, 0.999f)] private float returnSpeedFactor;
    [SerializeField, Range(0, 100)] private float snappinessMultiplier;
    [SerializeField, Range(0, 100)] private float returnSpeedMultiplier;


    public Transform RecoilTransform { get; set; }

    

    
    // Update is called once per frame
    void Update()
    {
        _targetRotation = SlerpDamp(_targetRotation, Vector3.zero, 1f - returnSpeedFactor , Time.deltaTime * returnSpeedMultiplier);
        _currentRotation = SlerpDamp(_currentRotation, _targetRotation, 1f - snappinessFactor , Time.deltaTime * snappinessMultiplier);
        RecoilTransform.localRotation = Quaternion.Euler(_currentRotation);
    }

    public void RecoilFire()
    {
        _targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
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
