using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace TimeRuins
{
	public class DialogController : MonoBehaviour
	{
		public TextMeshProUGUI Dialog;
		public float TextSpeed; // How fast the text appears on screen
		public float TextEraseSpeed; // How fast text disappears
		public float MessageTime; // How long text remains on screen before disappearing

		// Start is called before the first frame update
		void Awake()
		{
			// Reset the Dialog box
			Dialog?.SetText("");
		}

		public void DisplayMessage(string message)
		{
			StartCoroutine(WriteMessage(message));
		}

		private IEnumerator WriteMessage(string message)
		{
			// Write out the text
			for (int i = 0; i < message.Length; i++)
			{
				// Set the message in the dialog object
				Dialog?.SetText(message.Substring(0, i));
				// Pause before typing the next letter
				yield return new WaitForSecondsRealtime(1 / TextSpeed);
			}

			// Give the player time to read
			yield return new WaitForSecondsRealtime(MessageTime);

			// Erase the text
			for (int i = message.Length; i >= 0; i--)
			{
				// Set the message in the dialog object
				Dialog?.SetText(message.Substring(0, i));
				// Pause before typing the next letter
				yield return new WaitForSecondsRealtime(1 / TextEraseSpeed);
			}
		}
	}
}
