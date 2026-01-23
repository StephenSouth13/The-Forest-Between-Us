using UnityEngine;

namespace MapAndRadarSystem
{
	public class Minimap : MonoBehaviour
	{
		public Camera topDownCamera;

		void LateUpdate()
		{
			if (MapAndRadarManager.Instance != null && MapAndRadarManager.Instance.Actor != null)
            {
				Vector3 newPosition = MapAndRadarManager.Instance.Actor.position;
				newPosition.y = topDownCamera.transform.position.y;
				topDownCamera.transform.position = newPosition;
				topDownCamera.transform.rotation = Quaternion.Euler(90f, MapAndRadarManager.Instance.Actor.eulerAngles.y, 0f);
			}
		}

		public void Click_ZoomIn()
		{
			if (topDownCamera.orthographicSize > 30)
			{
				topDownCamera.orthographicSize = topDownCamera.orthographicSize - 5;
			}
		}

		public void Click_ZoomOut()
		{
			if (topDownCamera.orthographicSize < 70)
			{
				topDownCamera.orthographicSize = topDownCamera.orthographicSize + 5;
			}
		}
	}
}