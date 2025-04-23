using DamageSystem;
using VRBeats.Events;
using UnityEngine;
using EzySlice;

namespace VRBeats
{
    public class Cuttable : Damageable
    {
        [SerializeField] private OnDamageEvent onCut = null;

        private Material insideMaterial = null;

        private const float minCutVelocity = 0.25f;

        private void Awake()
        {
            insideMaterial = GetComponent<MeshRenderer>().material;
        }

        public override void DoDamage(DamageInfo info)
        {
            onCut.Invoke(info);

            var beatDamageInfo = info as BeatDamageInfo;
            if (beatDamageInfo == null) return;
            
            Vector3 cutDir = Vector3.right;

            if (beatDamageInfo.velocity > minCutVelocity)
            {
                cutDir = CalculateCutDirection(info.hitDir, beatDamageInfo.hitObject.transform.up);
            }            

            if (Cut(info.hitPoint, cutDir, insideMaterial))
            {
                Destroy(gameObject);
            }
        }

        private Vector3 CalculateCutDirection(Vector3 hitDir, Vector3 saberUp)
        {

            Vector3 cutDir;

            if (Mathf.Abs(hitDir.y) > Mathf.Abs(hitDir.x))
            {
                cutDir = Vector3.right * Mathf.Sign(hitDir.y);

                Vector2 saberDir = new Vector2(saberUp.x, saberUp.z);
                Vector2 planeDir = new Vector2(0.0f, 1.0f);
                saberDir.Normalize();
                planeDir.Normalize();

                float saberAngle = Vector2.SignedAngle(saberDir, planeDir);
                float hitAngle = Vector3.SignedAngle(hitDir, cutDir, Vector3.forward);
                cutDir = Quaternion.Euler(0.0f, saberAngle, 90.0f - hitAngle) * cutDir;

            }
            else
            {

                Vector2 saberDir = new Vector2(saberUp.x, saberUp.y) * -1;
                Vector2 planeDir = new Vector2(0.0f, 1.0f);
                saberDir.Normalize();
                planeDir.Normalize();

                float saberAngle = Vector3.SignedAngle(saberUp, Vector3.forward, Vector3.right); ;

                Vector2 hit = new Vector2(hitDir.x, hitDir.y);
                Vector2 planeRight = new Vector2(1.0f, 0.0f);

                float hitAngle = Vector2.SignedAngle(hit, planeRight);

                cutDir = Quaternion.Euler(-saberAngle, 0.0f, -hitAngle) * Vector3.up;
            }

            return cutDir;
        }


        private bool Cut(Vector3 point, Vector3 up, Material mat)
        {
            GameObject[] parts = gameObject.SliceInstantiate(point, up, mat);

            if (parts == null)
                return false;

            for (int n = 0; n < parts.Length; n++)
            {
                parts[n].AddComponent<DestroyOnBecameInvisible>();
            }

            Rigidbody rb = parts[0].AddComponent<Rigidbody>();
            rb.AddForce(100.0f * up);
            rb.AddForce(200.0f * transform.forward * -1);



            rb = parts[1].AddComponent<Rigidbody>();
            rb.AddForce(100.0f * up * -1.0f);
            rb.AddForce(200.0f * transform.forward * -1);

            return true;
        }

    }

}
