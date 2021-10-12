using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing
{
	public class FrameTimeCounter : MonoBehaviour
	{
		private Text _text;
		private string s = "Frame time: ";

		private void Awake()
		{
			_text = GetComponent<Text>();
			StartCoroutine(SetText());
		}

		private IEnumerator SetText()
		{
			while (true)
			{
				float time = 0;
				yield return new WaitForSeconds(0.2f);
				time += Time.deltaTime;
				yield return new WaitForSeconds(0.2f);
				time += Time.deltaTime;
				yield return new WaitForSeconds(0.2f);
				time += Time.deltaTime;
				yield return new WaitForSeconds(0.2f);
				time += Time.deltaTime;
				yield return new WaitForSeconds(0.2f);
				time += Time.deltaTime;
				_text.text = s + (time / 5 * 100).ToString("0.00") + "ms";
			}
		}
	}
}
