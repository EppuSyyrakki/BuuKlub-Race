using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.GUI
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField]
		private RectTransform damageTransform;

		private readonly Vector3 _startingScale = new Vector3(0, 1, 1);
		private readonly Vector3 _endingScale = new Vector3(1, 1, 1);

		private void Awake()
		{
			damageTransform.localScale = _startingScale;
		}

		public void SetHealth(int currentHealth, int max)
		{
			int currentDamage = max - currentHealth;
			var newScale = Vector3.Lerp(_startingScale, _endingScale, (float) currentDamage / (float) max);
			// var newScale = new Vector3((float) current / (float) max, 1, 1);
			damageTransform.localScale = newScale;
		}
	}
}
