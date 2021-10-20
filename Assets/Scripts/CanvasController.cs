using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing
{
	public class CanvasController : MonoBehaviour
	{
		[SerializeField]
		private float fadeOutTime = 0.75f;

		public void StartGame()
		{
			FadeOutAll();
			var buttons = GetComponentsInChildren<Button>();

			foreach (var button in buttons)
			{
				button.interactable = false;
			}
		}

		public void RestartGame()
		{
			Debug.Log("Restart game pressed");
		}

		private void FadeOutAll()
		{
			var images = GetComponentsInChildren<Image>();

			foreach (var image in images)
			{
				StartCoroutine(FadeOut(image, fadeOutTime));
			}

			Invoke(nameof(EnableGame), fadeOutTime);
		}

		private IEnumerator FadeOut(Image image, float time)
		{
			float t = 0;

			while (t < time)
			{
				var color = Color.white;
				color.a = Mathf.Lerp(1, 0, t / time);
				image.color = color;
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}

		private void EnableGame()
		{
			Game.Instance.ReadyToStart = true;
			gameObject.SetActive(false);
		}
	}
}
