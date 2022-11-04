using UnityEngine;
using UnityEngine.Rendering;

namespace JC.Snow
{
    [ExecuteInEditMode]
    public class UpdateConnection : MonoBehaviour
    {
        public ScriptableConnection ScriptableConnection { get { return scriptableConnection; } set { scriptableConnection = value; } }
        [SerializeField] private ScriptableConnection scriptableConnection;

        private bool toRelease;

        private void Start()
        {
            int renderResolution = 1;
            RenderTexture renderTexture = new RenderTexture(renderResolution, renderResolution, 0, RenderTextureFormat.ARGB32);

            transform.GetChild(1).GetComponent<Camera>().targetTexture = renderTexture;
            Material materialInstance = Instantiate(transform.GetChild(0).GetChild(0).GetComponent<Renderer>().sharedMaterial) as Material;

            materialInstance.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);

            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().sharedMaterial = materialInstance;
            transform.GetChild(0).GetChild(1).GetComponent<Renderer>().sharedMaterial = materialInstance;
            transform.GetChild(0).GetChild(2).GetComponent<Renderer>().sharedMaterial = materialInstance;
        }

        private void LateUpdate()
        {
            ToggleRenderMaterial();
        }

        private void ToggleRenderMaterial()
        {
            Vector2 xzMainCamera = new Vector2(scriptableConnection.mainCamera.position.x, scriptableConnection.mainCamera.position.z);

            Vector3 groundPosition = transform.GetChild(0).GetChild(0).position;
            Vector2 xzPosition = new Vector2(groundPosition.x, groundPosition.z);

            if(Vector2.Distance(xzMainCamera, xzPosition) < scriptableConnection.renderDistance && !toRelease)
            {
                int renderResolution = 256;
                RenderTexture renderTexture = new RenderTexture(renderResolution, renderResolution, 0, RenderTextureFormat.ARGB32);

                transform.GetChild(1).gameObject.GetComponent<Camera>().targetTexture = renderTexture;

                transform.GetChild(0).GetChild(0).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);
                transform.GetChild(0).GetChild(1).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);
                transform.GetChild(0).GetChild(2).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);

                transform.GetChild(1).gameObject.SetActive(true);

                toRelease = true;
            }

            else if(Vector2.Distance(xzMainCamera, xzPosition) < scriptableConnection.renderDistance && toRelease) return;
            else if(!toRelease) return;
            else
            {
                transform.GetChild(1).gameObject.SetActive(false);
                
                int renderResolution = 1;
                RenderTexture renderTexture = new RenderTexture(renderResolution, renderResolution, 0, RenderTextureFormat.ARGB32);

                transform.GetChild(1).gameObject.GetComponent<Camera>().targetTexture = renderTexture;

                transform.GetChild(0).GetChild(0).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);
                transform.GetChild(0).GetChild(1).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);
                transform.GetChild(0).GetChild(2).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", renderTexture, RenderTextureSubElement.Color);

                toRelease = false;
            }
        }
    }
}
