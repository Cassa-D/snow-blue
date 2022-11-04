using UnityEngine;

namespace JC.Snow
{
    [RequireComponent(typeof (ParticleSystem))]
    public class ControlledTrail : MonoBehaviour
    {
        private ParticleSystem ps;
        private Transform relativeParent;
        private ParticleSystem.MainModule mainModule;

        private void Awake()
        {
            ps = gameObject.GetComponent<ParticleSystem>();
            relativeParent = transform.parent;
            mainModule = ps.main;
        }

        private void LateUpdate()
        {
            mainModule.startRotation = relativeParent.localRotation.eulerAngles.y * Mathf.Deg2Rad;
        }
    }
}

