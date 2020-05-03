using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class Player : MonoBehaviour
	{
		public SpriteRenderer SpriteRenderer { get; set; }
		public Animator Animator { get; set; }

		private void Awake()
		{
			// Get references to the reference objects
			Animator = GetComponent<Animator>();
			SpriteRenderer = GetComponent<SpriteRenderer>();
		}

		public void Die()
		{
			// Set the death animation
			Animator?.SetTrigger("Death");
		}
	}
}
