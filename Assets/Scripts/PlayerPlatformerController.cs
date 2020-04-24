using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class PlayerPlatformerController : PhysicsObject
	{
		public float MaxSpeed = 10; // The max speed the player can accelerate to
		public float JumpForce = 24; // The vertical force applied to the player when they jump
		public float MaxJumpBuffer = 0.2f; // The time after being grounded that the player has to jump
		public int MaxAirJumps = 1; // The number of jumps the player is allowed to perform in the air

		private SpriteRenderer SpriteRenderer;
		private Animator Animator;

		private float JumpBuffer;
		private int AirJumpCount = 0; // how many times the player has jumped before touching the ground

		// Use this for initialization
		void Awake()
		{
			// Grab references to components
			SpriteRenderer = GetComponent<SpriteRenderer>();
			Animator = GetComponent<Animator>();
		}

		protected override void ComputeVelocity()
		{
			Vector2 move = Vector2.zero;

			// Zero out jumps if on the ground
			if (Grounded)
			{
				JumpBuffer = 0;
				AirJumpCount = 0;
			}
			else
			{
				JumpBuffer += Time.deltaTime;
			}

			// Handle horizontal input from the player
			move.x = Input.GetAxis("Horizontal");

			// Handle jumping
			if (Input.GetButtonDown("Jump"))
			{
				// Allow the player to jump if they are, or were recently, grounded
				if (Grounded || JumpBuffer < MaxJumpBuffer)
				{
					// Jump off the ground
					Velocity.y = JumpForce;
				}
				// Also allow the player to jump if they have not exceeded their air jump count
				else if (AirJumpCount < MaxAirJumps)
				{
					// Jump in the air
					AirJumpCount++;
					Velocity.y = JumpForce;
				}
			}

			// Handle jump cancelling
			if (!Grounded && Input.GetButtonUp("Jump"))
			{
				if (Velocity.y > 0)
				{
					Velocity.y = Velocity.y * 0.5f;
				}
			}

			// Handle flipping the sprite when the player moves left
			SpriteRenderer.flipX = !SpriteRenderer.flipX ? (move.x < 0f) : (move.x <= 0f);

			// Set animator properties
			Animator.SetBool("Grounded", Grounded);
			Animator.SetFloat("VelocityX", Mathf.Abs(Velocity.x) / MaxSpeed);

			// Apply the new Velocity to the targetVelocity for the physics object
			base.TargetVelocity = move * MaxSpeed;
		}
	}
}