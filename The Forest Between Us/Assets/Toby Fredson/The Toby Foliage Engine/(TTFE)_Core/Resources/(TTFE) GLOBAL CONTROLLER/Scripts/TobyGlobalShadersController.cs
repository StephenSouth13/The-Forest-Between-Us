using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TobyFredson
{
	[ExecuteInEditMode]
	public class TobyGlobalShadersController : MonoBehaviour
	{
		#region Inspector Fields
		[SerializeField] private TobyWindType windType;
		[SerializeField] [Range(0f, 1f)] private float windStrength;
		[SerializeField] [Range(1f, 3f)] private float windSpeed;
		[SerializeField] [Range(-2f, 2f)] private float season;
		#endregion

		#region Private Fields
		private TobyShaderValuesModel cachedAppliedValue;
		private readonly List<Material> matsGrassFoliage = new List<Material>();
		private readonly List<Material> matsTreeBark = new List<Material>();
		private readonly List<Material> matsTreeFoliage = new List<Material>();
		private readonly List<Material> matsTreeBillboard = new List<Material>();
		private readonly List<Material> matsGlobalController = new List<Material>();
		private readonly HashSet<Renderer> registeredRenderers = new HashSet<Renderer>();
		private bool isRefreshing;
		#endregion

		#region Unity Callbacks
		protected void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		protected void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		protected void Start()
		{
			if (!isRefreshing)
			{
				Refresh();
			}
		}

		protected void Update()
		{
			var newValues = GetNewValues();
			if (!newValues.Equals(cachedAppliedValue))
			{
				ApplyValues(newValues);
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (!isRefreshing)
			{
				Refresh();
			}
		}
		#endregion

		#region Public API
		[ContextMenu("TobyFredson - Refresh")]
		public virtual void Refresh()
		{
			if (!isRefreshing)
			{
				StartCoroutine(RefreshListCoroutine(true));
			}
		}

		public void RegisterRenderer(Renderer ren)
		{
			registeredRenderers.Add(ren);
			var mats = ren.sharedMaterials;
			foreach (var mat in mats)
			{
				RegisterMaterial(mat);
			}
		}

		public void UnregisterRenderer(Renderer ren)
		{
			registeredRenderers.Remove(ren);
		}

		public int GetMaterialCount(string type)
		{
			switch (type)
			{
			case "Grass": return matsGrassFoliage.Count;
			case "Bark": return matsTreeBark.Count;
			case "Foliage": return matsTreeFoliage.Count;
			case "Billboard": return matsTreeBillboard.Count;
			case "Controller": return matsGlobalController.Count;
			default: return 0;
			}
		}
		#endregion

		#region Client Impl
		protected virtual IEnumerator RefreshListCoroutine(bool forceRefresh = false)
		{
			if (isRefreshing) yield break;
			isRefreshing = true;

			if (forceRefresh || registeredRenderers.Count == 0)
			{
				matsGrassFoliage.Clear();
				matsTreeBark.Clear();
				matsTreeFoliage.Clear();
				matsTreeBillboard.Clear();
				matsGlobalController.Clear();
				registeredRenderers.Clear();

				var renderers = FindObjectsOfType<Renderer>();
				int batchSize = 100;
				for (int i = 0; i < renderers.Length; i += batchSize)
				{
					int end = Mathf.Min(i + batchSize, renderers.Length);
					for (int j = i; j < end; j++)
					{
						RegisterRenderer(renderers[j]);
					}
					yield return null;
				}
			}
			else
			{
				foreach (var ren in registeredRenderers)
				{
					var mats = ren.sharedMaterials;
					foreach (var mat in mats)
					{
						RegisterMaterial(mat);
					}
				}
			}

			Debug.Log($"Grass: {matsGrassFoliage.Count}, Bark: {matsTreeBark.Count}, Foliage: {matsTreeFoliage.Count}, Billboard: {matsTreeBillboard.Count}");
			isRefreshing = false;

			ApplyValues(GetNewValues(), true);
		}

		protected virtual void RegisterMaterial(Material mat)
		{
			if (mat == null || mat.shader == null) return;

			if (mat.shader.name.Equals(TobyConstants.SHADER_NAME_GRASS_FOLIAGE))
			{
				matsGrassFoliage.Add(mat);
			}
			else if (mat.shader.name.Equals(TobyConstants.SHADER_NAME_TREE_BARK))
			{
				matsTreeBark.Add(mat);
			}
			else if (mat.shader.name.Equals(TobyConstants.SHADER_NAME_TREE_FOLIAGE))
			{
				matsTreeFoliage.Add(mat);
			}
			else if (mat.shader.name.Equals(TobyConstants.SHADER_NAME_TREE_BILLBOARD))
			{
				matsTreeBillboard.Add(mat);
			}
			else if (mat.shader.name.Equals(TobyConstants.SHADER_NAME_GLOBAL_CONTROLLER))
			{
				matsGlobalController.Add(mat);
			}
		}

		protected virtual void ApplyValues(TobyShaderValuesModel model, bool isForced = false)
		{
			if (matsGrassFoliage.Count == 0 && matsTreeBark.Count == 0 && matsTreeFoliage.Count == 0) return;
			if (!isForced && cachedAppliedValue.Equals(model)) return;

			var mats = new List<Material>();
			mats.AddRange(matsTreeFoliage);
			mats.AddRange(matsTreeBark);
			mats.AddRange(matsGrassFoliage);
			mats.AddRange(matsTreeBillboard);
			mats.AddRange(matsGlobalController);

			foreach (var mat in mats)
			{
				mat.SetFloat(TobyConstants.SHADER_VAR_FLOAT_SEASON, model.season);
				mat.SetFloat(TobyConstants.SHADER_VAR_FLOAT_WIND_STRENGTH, model.windStrength);
				mat.SetFloat(TobyConstants.SHADER_VAR_FLOAT_WIND_SPEED, model.windSpeed);

				switch (model.windType)
				{
				case TobyWindType.GentleBreeze:
					mat.EnableKeyword(TobyConstants.SHADER_WIND_TYPE_VALUE_GENTLEBREEZE);
					mat.DisableKeyword(TobyConstants.SHADER_WIND_TYPE_VALUE_WINDOFF);
					break;
				case TobyWindType.WindOff:
					mat.EnableKeyword(TobyConstants.SHADER_WIND_TYPE_VALUE_WINDOFF);
					mat.DisableKeyword(TobyConstants.SHADER_WIND_TYPE_VALUE_GENTLEBREEZE);
					break;
				}
			}

			cachedAppliedValue = model;
		}

		protected TobyShaderValuesModel GetNewValues()
		{
			return new TobyShaderValuesModel
			{
				season = season,
				windType = windType,
				windStrength = windStrength,
				windSpeed = windSpeed
			};
		}
		#endregion
	}
}