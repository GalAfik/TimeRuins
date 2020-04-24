using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	[RequireComponent(typeof(Rigidbody2D))]

	public class PhysicsObject : MonoBehaviour
	{
		public float MinGroundNormalY = .65f; // The minimum angle of the ground that the object can "stand" on - used for slopes
		public float GravityModifier = 1f; // The gravity applied to the object

		protected Vector2 TargetVelocity;
		protected bool Grounded; // Is the object currently grounded?
		protected Vector2 GroundNormal;
		protected Rigidbody2D RigidBody;
		protected Vector2 Velocity;
		protected ContactFilter2D ContactFilter;
		protected RaycastHit2D[] HitBuffer = new RaycastHit2D[16];
		protected List<RaycastHit2D> HitBufferList = new List<RaycastHit2D>(16);

		protected const float minMoveDistance = 0.001f;
		protected const float shellRadius = 0.01f;

		void OnEnable()
		{
			// Get a reference to the rigid body component
			RigidBody = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			// Set up the contact filter for collisions
			ContactFilter.useTriggers = false;
			ContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
			ContactFilter.useLayerMask = true;
		}

		void Update()
		{
			// Zero out the velocity
			TargetVelocity = Vector2.zero;

			// Compute this object's velocity
			ComputeVelocity();
		}

		protected virtual void ComputeVelocity() { } // To be implemented in child classes

		void FixedUpdate()
		{
			Velocity += GravityModifier * Physics2D.gravity * Time.deltaTime;
			Velocity.x = TargetVelocity.x;

			// Reset whether the object is grounded
			Grounded = false;

			// The proposed change in position 
			Vector2 deltaPosition = Velocity * Time.deltaTime;
			Vector2 moveAlongGround = new Vector2(GroundNormal.y, -GroundNormal.x);
			Vector2 move = moveAlongGround * deltaPosition.x;

			// Apply x movement
			Movement(move, false);

			// Calculate the y movement
			move = Vector2.up * deltaPosition.y;

			// Apply y movement
			Movement(move, true);
		}

		// Apply the movement vector to the object's rigidbody
		void Movement(Vector2 move, bool yMovement)
		{
			float distance = move.magnitude;

			// Make sure that the object is actually moving
			if (distance > minMoveDistance)
			{
				// Check for collisions in the proposed new position
				int count = RigidBody.Cast(move, ContactFilter, HitBuffer, distance + shellRadius);
				HitBufferList.Clear();
				for (int i = 0; i < count; i++)
				{
					HitBufferList.Add(HitBuffer[i]);
				}

				for (int i = 0; i < HitBufferList.Count; i++)
				{
					Vector2 currentNormal = HitBufferList[i].normal;
					if (currentNormal.y > MinGroundNormalY)
					{
						Grounded = true;
						if (yMovement)
						{
							GroundNormal = currentNormal;
							currentNormal.x = 0;
						}
					}

					float projection = Vector2.Dot(Velocity, currentNormal);
					if (projection < 0)
					{
						Velocity = Velocity - projection * currentNormal;
					}

					// Modify the new position based on any collisions
					float modifiedDistance = HitBufferList[i].distance - shellRadius;
					distance = modifiedDistance < distance ? modifiedDistance : distance;
				}
			}

			// Finally, apply the move vector and velocity to the object
			RigidBody.position = RigidBody.position + move.normalized * distance;
		}
	}
}