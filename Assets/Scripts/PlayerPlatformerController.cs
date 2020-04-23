using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class PlayerPlatformerController : PhysicsObject
	{
		public float MaxSpeed = 7; // The max speed the player can accelerate to
		public float JumpForce = 7;

		private SpriteRenderer SpriteRenderer;
		private Animator Animator;

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

			// Handle horizontal input from the player
			move.x = Input.GetAxis("Horizontal");

			// Handle jumping
			if (Input.GetButtonDown("Jump") && Grounded)
			{
				Velocity.y = JumpForce;
			}
			else if (Input.GetButtonUp("Jump"))
			{
				if (Velocity.y > 0)
				{
					Velocity.y = Velocity.y * 0.5f;
				}
			}

			// Handle flipping the sprite when the player moves left
			bool flipSprite = (SpriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
			if (flipSprite) SpriteRenderer.flipX = !SpriteRenderer.flipX;

			// Set animator properties
			Animator.SetBool("Grounded", Grounded);
			Animator.SetFloat("VelocityX", Mathf.Abs(Velocity.x) / MaxSpeed);

			// Apply the new Velocity to the targetVelocity for the physics object
			base.TargetVelocity = move * MaxSpeed;
		}
	}
}