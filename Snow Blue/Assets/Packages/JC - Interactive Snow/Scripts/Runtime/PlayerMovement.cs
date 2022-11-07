using UnityEngine;

namespace JC.Snow
{
    [RequireComponent(typeof (Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Trail Code
        [SerializeField] private ParticleSystem trail;
        private bool isGround;

        // Show Trail only on ground
        private void Update()
        {
            if(isGround) ShowTrail();
            else HideTrail();
        }

        private void OnCollisionStay() { isGround = true; }
        private void OnCollisionExit() { isGround = false; }

        private void ShowTrail()
        {
            if(!trail.isEmitting) trail.Play();
        }

        private void HideTrail()
        {
            if(trail.isPlaying) trail.Stop();
        }
        #endregion

        #region Player Movement Code
        private Rigidbody rig;
        private Vector3 velMovement;
        private Vector3 initPosition;

        private void Awake()
        {
            rig = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            initPosition = transform.position;    
        }

        private void FixedUpdate()
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            rig.MovePosition(Vector3.SmoothDamp(rig.position, rig.position + direction, ref velMovement, Time.deltaTime * 5));

            if(direction != Vector3.zero)
            {
                rig.MoveRotation(Quaternion.Slerp(rig.rotation, Quaternion.LookRotation(direction), velMovement.magnitude * 4 * Time.deltaTime));
            }

            if(rig.position.y <= -5f)
            {
                rig.velocity = Vector3.zero;
                rig.MovePosition(initPosition);
            }
        }
        #endregion
    }
}
