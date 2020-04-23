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
		public Animator Animator;

		private void Awake()
		{
			// Get a reference to the Animator object
			Animator = GetComponent<Animator>();
		}
	}
}
