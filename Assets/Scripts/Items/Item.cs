using System.Collections;
using UnityEngine;

namespace BK.Items
{
	[RequireComponent(typeof(Billboard), 
		typeof(SphereCollider), 
		typeof(SpriteRenderer))]
	public abstract class Item : MonoBehaviour
	{
		protected SphereCollider col;

		private void Awake()
		{
			col = GetComponent<SphereCollider>();
			col.center = Vector3.up;
			col.radius = 1;
		}

		// Update is called once per frame
		void Update()
		{
			var self = transform.position;
			var newPos = new Vector3(self.x, self.y , self.z - Game.Instance.forwardSpeed * Time.deltaTime * 0.12f);
			transform.position = newPos;
		}
	}
}