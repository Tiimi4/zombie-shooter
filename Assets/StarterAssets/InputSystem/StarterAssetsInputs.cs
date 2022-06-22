using System;
using UnityEngine;
using UnityEngine.InputSystem;


	public class StarterAssetsInputs : MonoBehaviour
	{
		public GameInputActions InputActions;
		
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool shoot;
		public bool reload;
		public float selectWeapon;


		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;

		public void Awake()
		{
			InputActions = new GameInputActions();
			InputActions.Enable();
			InputActions.Gameplay.Movement.performed += OnMove;
			InputActions.Gameplay.Movement.canceled += OnMove;
			InputActions.Gameplay.Jump.performed += (context =>  JumpInput(context.ReadValueAsButton()));
			InputActions.Gameplay.Jump.canceled += (context =>  JumpInput(context.ReadValueAsButton()));
			InputActions.Gameplay.Look.performed += (context =>  LookInput(context.ReadValue<Vector2>()));
			InputActions.Gameplay.Look.canceled += (context =>  LookInput(context.ReadValue<Vector2>()));
			InputActions.Gameplay.Sprint.performed += (context =>  SprintInput(context.ReadValueAsButton()));
			InputActions.Gameplay.Sprint.canceled += (context =>  SprintInput(context.ReadValueAsButton()));
			InputActions.Gameplay.Shoot.performed += context => shoot = context.ReadValueAsButton();
			InputActions.Gameplay.Shoot.canceled += context => shoot = context.ReadValueAsButton();
			InputActions.Gameplay.Reload.performed += context => reload = context.ReadValueAsButton();
			InputActions.Gameplay.Reload.canceled += context => reload = context.ReadValueAsButton();
			InputActions.Gameplay.SelectWeapon.performed += context => selectWeapon = context.ReadValue<float>();
			InputActions.Gameplay.SelectWeapon.canceled += context => selectWeapon = context.ReadValue<float>();
			
		}
		

		
		
		public void OnMove(InputAction.CallbackContext callbackContext)
		{
			MoveInput(callbackContext.ReadValue<Vector2>());
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
