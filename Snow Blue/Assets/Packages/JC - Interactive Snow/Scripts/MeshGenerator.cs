using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace JC.Snow
{
	[ExecuteInEditMode]
	public class MeshGenerator : MonoBehaviour
	{
		#region Mesh - Public
		public int Detail { get { return detail; } set { detail = value; } }
		public int Scale { get { return scale; } set { scale = value; } }
		public int Connections { get { return connections; } set { connections = value; } }
		public int MaxColumns { get { return maxColumns; } set { maxColumns = value; } }
		public Vector3[,] GroundPositions { get { return groundPositions; } set { groundPositions = value; } }
		public float Lod0 { get { return lod0; } set { lod0 = value; } }
		public float Lod1 { get { return lod1; } set { lod1 = value; } }
		public float Lod2 { get { return lod2; } set { lod2 = value; } }
		#endregion

		#region Renderer - Public
		public string LayerName { get { return layerName; } set { layerName = value; } }
		#endregion

		#region Shader - Public
		public Texture2D NoiseTexture { get { return noiseTexture; } set { noiseTexture = value; } }
		public float GroundScale { get { return groundScale; } set { groundScale = value; } }
		public float GroundHeight { get { return groundHeight; } set { groundHeight = value; } }
		public float SnowDetail { get { return snowDetail; } set { snowDetail = value; } }
		public float TrailIntensity { get { return trailIntensity; } set { trailIntensity = value; } }
		public Color SnowColor { get { return snowColor; } set { snowColor = value; } }
		#endregion

		#region Mesh - Private
		[SerializeField] private int detail = 255;
		[SerializeField] private int scale = 32;
		[SerializeField] private int connections = 1;
		[SerializeField] private float lod0 = 0.9f;
		[SerializeField] private float lod1 = 0.7f;
		[SerializeField] private float lod2 = 0.2f;
		#endregion

		#region Renderer - Private
		[SerializeField] private string layerName = "Interactive Trail";
		#endregion

		#region Shader - Private
		[SerializeField] private Texture2D noiseTexture;
		[SerializeField] private float groundScale = 0.5f;
		[SerializeField] private float groundHeight = 5f;
		[SerializeField] private float snowDetail = 0.5f;
		[SerializeField] private float trailIntensity = 0.5f;
		[SerializeField] private Color snowColor = new Color(1, 1, 1, 1);
		#endregion
		
		#region LOD
		private Mesh meshLod0;
		private Mesh meshLod1;
		private Mesh meshLod2;
		private Material globalMaterial;
		private RenderTexture globalRenderTexture;
		#endregion

		#region Generate Mesh
		private Vector3[] vertices;
		private Vector3[,] groundPositions;
		private int maxColumns;
		#endregion

		#region Unity Methods
		private void Awake()
		{
			#if UNITY_EDITOR
            noiseTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/JC - Interactive Snow/Textures/Snow.png", typeof(Texture2D));
			if(noiseTexture == null) Debug.LogError("Default noise texture not found. Please use a noise texture for it work!");
			#endif

			CreateGlobalRenderTexture();
			CreateGlobalMaterial();
		}
		#endregion
		
		#region Public Methods
		public void UpgradeShader(GameObject groundObject)
		{
			Material materialRef = groundObject.GetComponent<Renderer>().sharedMaterial;
			materialRef.SetTexture("Noise_Texture", noiseTexture);
			materialRef.SetFloat("Ground_Scale", groundScale);
			materialRef.SetFloat("Ground_Height", groundHeight);
			materialRef.SetFloat("Snow_Detail", snowDetail);
			materialRef.SetFloat("Trail_Intensity", trailIntensity);
			materialRef.SetVector("Ground_Color", (Vector4)snowColor);
			groundObject.GetComponent<Renderer>().sharedMaterial = materialRef;
		}

		public void UpgradeRenderer(GameObject renderObject)
		{
			renderObject.GetComponent<Camera>().cullingMask = LayerMask.GetMask(layerName);
		}

		public void GenerateMesh(Transform meshTransform, Vector3 meshPosition, Transform renderTransform)
		{
			GenerateLodGroup(meshTransform, meshPosition);
			GenerateRender(meshPosition, renderTransform.gameObject);
		}

		public void CreateGlobalMeshes()
		{
			GenerateMeshLod(ref meshLod0, 0);
			GenerateMeshLod(ref meshLod1, 1);
			GenerateMeshLod(ref meshLod2, 2);
		}
		#endregion

		#region Private Methods
		private void CreateGlobalMaterial()
		{
			Shader snowShader = Shader.Find("Shader Graphs/Interactive Snow");
			if(snowShader == null) Debug.LogError("Shader Graphs/Interactive Snow - Not found");
			else
			{
				globalMaterial = new Material(snowShader);
				globalMaterial.SetTexture("_MainTex", globalRenderTexture, RenderTextureSubElement.Color);
				globalMaterial.SetTexture("Noise_Texture", noiseTexture);
				globalMaterial.SetFloat("Ground_Scale", groundScale);
				globalMaterial.SetFloat("Ground_Height", groundHeight);
				globalMaterial.SetFloat("Snow_Detail", snowDetail);
				globalMaterial.SetFloat("Trail_Intensity", trailIntensity);
				globalMaterial.SetVector("Ground_Color", (Vector4)snowColor);
				globalMaterial.enableInstancing = true;
			}
		}

		private void CreateGlobalRenderTexture()
		{
			globalRenderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
		}

		private void SetMesh(Transform meshTransform, Vector3 groundPosition, int lod)
		{
			MeshFilter meshFilter = meshTransform.gameObject.GetComponent<MeshFilter>();
			if(meshFilter == null) meshFilter = meshTransform.gameObject.AddComponent<MeshFilter>();

			MeshRenderer meshRenderer = meshTransform.gameObject.GetComponent<MeshRenderer>();
			if(meshRenderer == null) meshRenderer = meshTransform.gameObject.AddComponent<MeshRenderer>();

			if(lod == 0) meshFilter.mesh = meshLod0;
			if(lod == 1) meshFilter.mesh = meshLod1;
			if(lod == 2) meshFilter.mesh = meshLod2;

			globalMaterial.SetVector("Ground_Color", (Vector4)snowColor);

			meshRenderer.sharedMaterial = globalMaterial;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

			meshTransform.localPosition = groundPosition;
		}

		private void GenerateRender(Vector3 groundPosition, GameObject renderObject)
		{
			renderObject.SetActive(false);

			float renderSizePosition = (scale) / 2f;

			renderObject.transform.rotation = Quaternion.Euler(90, 0, 0);
			renderObject.transform.localPosition = new Vector3(groundPosition.x, groundPosition.y + renderSizePosition / 2, groundPosition.z);

			Camera renderCamera = renderObject.GetComponent<Camera>();
			if(renderCamera == null) renderCamera = renderObject.AddComponent<Camera>();

			renderCamera.orthographic = true;
			renderCamera.nearClipPlane = 0;
			renderCamera.orthographicSize = renderSizePosition;
			renderCamera.farClipPlane = renderSizePosition;
			renderCamera.clearFlags  = CameraClearFlags.SolidColor;
			renderCamera.backgroundColor = Color.black;
			renderCamera.cullingMask = LayerMask.GetMask(layerName);
		}

		public void GenerateCollider()
		{
			BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
			if(boxCollider == null) boxCollider = gameObject.AddComponent<BoxCollider>();

			boxCollider.size = new Vector3(scale * maxColumns - (scale * 0.05f * (maxColumns - 1)), 0, scale * maxColumns - (scale * 0.05f * (maxColumns - 1)));
		}

		private void GenerateLodGroup(Transform meshTransform, Vector3 meshPosition)
		{
			LODGroup lodGroup = meshTransform.gameObject.AddComponent<LODGroup>();

			LOD[] meshLods = new LOD[3];
			for (int i = 0; i < meshLods.Length; i++)
			{
				Transform lodTransform = new GameObject("Ground LOD_" + i).transform;
				lodTransform.SetParent(meshTransform);

				SetMesh(lodTransform, meshPosition, i);

				Renderer[] lodRenderes = new Renderer[1];
				lodRenderes[0] = lodTransform.gameObject.GetComponent<Renderer>();

				if(i == 0) meshLods[i] = new LOD(lod0, lodRenderes);
				if(i == 1) meshLods[i] = new LOD(lod1, lodRenderes);
				if(i == 2) meshLods[i] = new LOD(lod2, lodRenderes);
			}

			lodGroup.SetLODs(meshLods);
			lodGroup.RecalculateBounds();
		}

		private void GenerateMeshLod(ref Mesh meshLod, int lod)
		{
			int lodDetail = detail / (lod * 6 + 1);

			meshLod = new Mesh();
			meshLod.name = "Interactive Ground";
			
			vertices = new Vector3[(lodDetail + 1) * (lodDetail + 1)];
			Vector2[] uv = new Vector2[vertices.Length];
			
			for (int i = 0, y = 0; y <= lodDetail; y++) {
				for (int x = 0; x <= lodDetail; x++, i++) {
					vertices[i] = new Vector3((float)x * (scale) / lodDetail - (scale) / 2f, 0, (float)y * (scale) / lodDetail - (scale) / 2f);
					uv[i] = new Vector2((float)x / lodDetail, (float)y / lodDetail);
				}
			}

			meshLod.vertices = vertices;
			meshLod.uv = uv;

			int[] triangles = new int[lodDetail * lodDetail * 6];
			for (int ti = 0, vi = 0, y = 0; y < lodDetail; y++, vi++) {
				for (int x = 0; x < lodDetail; x++, ti += 6, vi++) {
					triangles[ti] = vi;
					triangles[ti + 3] = triangles[ti + 2] = vi + 1;
					triangles[ti + 4] = triangles[ti + 1] = vi + lodDetail + 1;
					triangles[ti + 5] = vi + lodDetail + 2;
				}
			}

			meshLod.triangles = triangles;
			meshLod.RecalculateNormals();
		}
		#endregion
	}
}
