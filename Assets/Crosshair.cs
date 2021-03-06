using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    private RectTransform crosshair;
    [SerializeField] private float restingSize;
    [SerializeField] private float maxMoveSize;
    [SerializeField] private float maxShootSize;
    [SerializeField] private float maxJumpSize;
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeedMultiplier;
    [SerializeField] private float shootSpeedMultiplier;
    [SerializeField] private float shotResetTimeout;

    


    
    private float _currentSize;
    private StarterAssetsInputs _inputs;
    private FirstPersonController _controller;

    public bool bulletWasShot = false;
    private float _timeSinceLastShot;


    // Start is called before the first frame update
    void Start()
    {
        crosshair = GetComponent<RectTransform>();
        _inputs = GameObject.Find("PlayerCapsule").GetComponent<StarterAssetsInputs>();
        _controller = GameObject.Find("PlayerCapsule").GetComponent<FirstPersonController>();

    }

    // Update is called once per frame
    void Update()
    {
        

        if (IsShooting())
        {
            _currentSize = _currentSize = Mathf.Lerp(_currentSize, maxShootSize, Time.deltaTime * shootSpeedMultiplier);
        }

        if (IsInAir())
        {
            _currentSize = _currentSize = Mathf.Lerp(_currentSize, maxJumpSize, Time.deltaTime * jumpSpeedMultiplier);
        } else if (IsMoving())
        {
            _currentSize = Mathf.Lerp(_currentSize,maxMoveSize, Time.deltaTime * speed);
        }
        else
        {
            _currentSize = Mathf.Lerp(_currentSize,restingSize, Time.deltaTime * speed);
        }

        crosshair.sizeDelta = new Vector2(_currentSize, _currentSize);
    }

    private bool IsMoving()
    {
        return _inputs.move.SqrMagnitude() > 0;
        
    }

    private bool IsJumping()
    {
        return _inputs.jump;
    }

    private bool IsInAir()
    {
        return !_controller.Grounded;
    }

    private bool IsShooting()
    {
        _timeSinceLastShot += Time.deltaTime;
        if (bulletWasShot)
        {
            _timeSinceLastShot = 0f;
            bulletWasShot = false;
            return true;
        }

        if (_timeSinceLastShot < shotResetTimeout)
        {
            return true;
        }

        return false;
        
    }
}
