using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weapons;

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Max health of player")]
		public int MaxHealth = 100;
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		private GunSystem _currentGun;
		private int _currentGunIndex;
		public GunSystem[] Guns;
		private Coroutine ReloadCoroutine;
		public Transform RecoilTransform;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;
		
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.001f;
		public HealthSystem HpSystem;

		private TextMeshProUGUI _hpText;
		

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			// pick first gun of the list
			_currentGun = Guns[0];

		}

		private void Start()
		{
			HpSystem = new HealthSystem(MaxHealth);
			HpSystem.OnDeath += Die;
			HpSystem.OnDamaged += UpdateHpText;
			_hpText = GameObject.Find("healthText").GetComponent<TextMeshProUGUI>();
			UpdateHpText();
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			
			AddWeaponCallbacks();
			Cursor.lockState = CursorLockMode.Locked;

		}

		private void AddWeaponCallbacks()
		{
			_currentGun.gameObject.SetActive(true);
			_currentGun.OnEmptyClick += StartReloadAnimation;
			_currentGun.recoilScript.RecoilTransform = RecoilTransform;

		}

		private void UpdateHpText()
		{
			_hpText.text = "HP: " + HpSystem.GetHealth();
		}

		private void StartReloadAnimation()
		{
			if (ReloadCoroutine == null)
			{
				ReloadCoroutine = StartCoroutine(nameof(HandleReloadCoroutine));
			}
			
		}

		private void Die()
		{
			SceneManager.LoadScene("GameOver");
		}

		private IEnumerator HandleReloadCoroutine()
		{
			_currentGun.StartReload();
			yield return new WaitForSeconds(_currentGun.reloadTime);
			_currentGun.FillAndCockWeapon();
			ReloadCoroutine = null;
		}
	

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
			ShootWeapon();
			ManualReload();
			SelectWeapon();
			_currentGun.recoilScript.ApplyRotationToTransform();
			
			
		}

		private void SelectWeapon()
		{
			if ( _input.selectWeapon > 0)
			{
				int index = (int)_input.selectWeapon;
				if (_currentGunIndex == index) return;
				if (index  > Guns.Length) return;
				_currentGun.gameObject.SetActive(false);
				_currentGunIndex = index;
				_currentGun = Guns[index - 1];
				AddWeaponCallbacks();
				_currentGun.InitAmmoTextRef();
				_currentGun.UpdateAmmoText();

			}
			
		}

		private void ShootWeapon()
		{
			_currentGun.PullTrigger(_input.shoot);
		}

		private void ManualReload()
		{
			if (_input.reload)
			{
				StartReloadAnimation();
			}
		}
		private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		bool SameSign(float num1, float num2)
		{
			if (num1 > 0 && num2 < 0)
				return false;
			if (num1 < 0 && num2 > 0)
				return false;
			return true;
		}
		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				bool IsCurrentDeviceMouse = true;
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				float deltaPitch = _input.look.y * RotationSpeed * deltaTimeMultiplier;
				float deltaYaw =   _input.look.x * RotationSpeed * deltaTimeMultiplier;
				Vector3 targetRecoil = _currentGun.recoilScript.targetRotation;

				
				if (targetRecoil.x > 180)
				{
					targetRecoil.x -= 360;
				}
				
				if (targetRecoil.y > 180)
				{
					targetRecoil.y -= 360;
				}
				if ( Mathf.Abs(targetRecoil.x) >= 0.001 && Mathf.Abs(deltaPitch) >= 0.001 && !SameSign(targetRecoil.x, deltaPitch))
				{
					
					float recoilRotationAmount = Mathf.Min(Mathf.Abs(deltaPitch), Mathf.Abs(targetRecoil.x));
					//RecoilTransform.Rotate(recoilRotationAmount * Mathf.Sign(deltaPitch) * Vector3.right, Space.Self);
					_currentGun.recoilScript.targetRotation += recoilRotationAmount * Mathf.Sign(deltaPitch) * Vector3.right;
					//deltaPitch -= recoilRotationAmount * Mathf.Sign(deltaPitch);
					
				}
				
				
				if ( Mathf.Abs(targetRecoil.y) >= 0.001 && Mathf.Abs(deltaYaw) >= 0.001 && !SameSign(targetRecoil.y, deltaYaw))
				{
					
					float recoilRotationAmount = Mathf.Min(Mathf.Abs(deltaYaw), Mathf.Abs(targetRecoil.y));
					//RecoilTransform.Rotate(recoilRotationAmount * Mathf.Sign(deltaYaw) * Vector3.up, Space.World );
					
					_currentGun.recoilScript.targetRotation += recoilRotationAmount * Mathf.Sign(deltaYaw) * Vector3.up;
					//deltaYaw -= recoilRotationAmount * Mathf.Sign(deltaYaw);
					
					
				}
				
				
				Vector3 currentRecoil = _currentGun.recoilScript.currentRotation;

				
				if (currentRecoil.x > 180)
				{
					currentRecoil.x -= 360;
				}
				
				if (currentRecoil.y > 180)
				{
					currentRecoil.y -= 360;
				}
				if ( Mathf.Abs(currentRecoil.x) >= 0.001 && Mathf.Abs(deltaPitch) >= 0.001 && !SameSign(currentRecoil.x, deltaPitch))
				{
					
					float recoilRotationAmount = Mathf.Min(Mathf.Abs(deltaPitch), Mathf.Abs(currentRecoil.x));
					//RecoilTransform.Rotate(recoilRotationAmount * Mathf.Sign(deltaPitch) * Vector3.right, Space.Self);
					_currentGun.recoilScript.currentRotation += recoilRotationAmount * Mathf.Sign(deltaPitch) * Vector3.right;
					deltaPitch -= recoilRotationAmount * Mathf.Sign(deltaPitch);
					
				}
				
				
				if ( Mathf.Abs(currentRecoil.y) >= 0.001 && Mathf.Abs(deltaYaw) >= 0.001 && !SameSign(currentRecoil.y, deltaYaw))
				{
					
					float recoilRotationAmount = Mathf.Min(Mathf.Abs(deltaYaw), Mathf.Abs(currentRecoil.y));
					//RecoilTransform.Rotate(recoilRotationAmount * Mathf.Sign(deltaYaw) * Vector3.up, Space.World );
					
					_currentGun.recoilScript.currentRotation += recoilRotationAmount * Mathf.Sign(deltaYaw) * Vector3.up;
					deltaYaw -= recoilRotationAmount * Mathf.Sign(deltaYaw);
					
					
				}
				
				_cinemachineTargetPitch += deltaPitch;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * deltaYaw);
				
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			if (_input.move.magnitude > 1f)
			{
				_input.move.Normalize();
			}
			float inputMagnitude = _input.move.magnitude;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}