using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace TimeRuins
{
	public class Timer : MonoBehaviour
	{
		public Slider Slider;

		public void SetMaxTimer(float time)
		{
			Slider.maxValue = time;
			Slider.value = time;
		}

		public void SetTimer(float time)
		{
			Slider.value = time;
		}
	}
}
