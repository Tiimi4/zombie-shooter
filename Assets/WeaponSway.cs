using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
	[Header("Sway settings")]
	[SerializeField] private float smooth;
	[SerializeField] private float swayMultiplier;

	
	private StarterAssetsInputs _input;
    // Start is called before the first frame update
    void Start()
    {
	    _input = GameObject.Find("PlayerCapsule").GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
	    float mouseX = _input.look.x * swayMultiplier;
	    float mouseY = _input.look.y * swayMultiplier;
	    
	    Quaternion rotationX = Quaternion.AngleAxis(mouseX, Vector3.up);
	    Quaternion rotationY = Quaternion.AngleAxis(-mouseY, Vector3.right);

	    Quaternion targetRotation = rotationX * rotationY;
	    
	    // rotate
	    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);

    }
}
