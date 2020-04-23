using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class Player : MonoBehaviour
	{
		[Serializable]
		public class ConfigurationData
		{
			[Header("Reference Objects")]
			public TimeController TimeController;
		}
		public ConfigurationData Conf = new ConfigurationData();
		public SpriteRenderer SpriteRenderer;
		public Animator Animator;

		private void Awake()
		{
			// Get references to the reference objects
			Animator = GetComponent<Animator>();
			SpriteRenderer = GetComponent<SpriteRenderer>();
		}
	}
}
