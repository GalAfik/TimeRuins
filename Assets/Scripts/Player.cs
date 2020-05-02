using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class Player : MonoBehaviour
	{
		public SpriteMask TimelineMask;
		public SpriteRenderer SpriteRenderer { get; set; }
		public Animator Animator { get; set; }

		private void Awake()
		{
			// Get references to the reference objects
			Animator = GetComponent<Animator>();
			SpriteRenderer = GetComponent<SpriteRenderer>();
		}

		public void SetTimelineMask(bool status)
		{
			// Grow/Shrink the mask
			IEnumerator growAction = TimelineMask?.GetComponent<TimelineMask>().Grow();
			IEnumerator shrinkAction = TimelineMask?.GetComponent<TimelineMask>().Shrink();
			if (status == true) StartCoroutine(growAction);
			else StartCoroutine(shrinkAction);
		}

		public void Die()
		{
			// Set the death animation
			Animator?.SetTrigger("Death");
		}
	}
}
