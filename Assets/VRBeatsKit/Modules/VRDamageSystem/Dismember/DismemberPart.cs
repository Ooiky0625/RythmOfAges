using UnityEngine;
using System.Collections.Generic;
using DamageSystem.Events;

namespace DamageSystem.Dismember
{
    public class DismemberPart : Damageable
    {
        [SerializeField] private Rigidbody bodyPartPrefab = null;
        [SerializeField] private GameObject realBodyPart = null;
        [SerializeField] private float resistance = 0.25f;
        [SerializeField] private bool killOnDismember = false;
        [SerializeField] private GameObject[] connectedBodyParts = null;
        [SerializeField] private OnDismemberEvent onDismember = null;

        private float hp = 0.0f;
        private DamageablePart thisDamageablePart = null;        
        private Dictionary<Mesh, GameObject> bodyPartStateConnections = new Dictionary<Mesh, GameObject>();
        private Rigidbody thisRB = null;

        public bool KillOnDismember { get { return killOnDismember; } }
        public float Resistance { get { return resistance; } }
        public OnDismemberEvent OnDismember { get { return onDismember; } }

        private void Awake()
        {
            thisDamageablePart = GetComponent<DamageablePart>();
            thisRB = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            CreateBodyPartStateConnections();
        }

        private void CreateBodyPartStateConnections()
        {
            for (int n = 0; n < connectedBodyParts.Length; n++)
            {
                SkinnedMeshRenderer skinnedMeshRender = connectedBodyParts[n].GetComponent<SkinnedMeshRenderer>();

                if (skinnedMeshRender != null)
                {
                    bodyPartStateConnections[skinnedMeshRender.sharedMesh] = skinnedMeshRender.gameObject;                   
                }
            }
        }

        public void SetHP(float hp)
        {
            this.hp = hp * resistance;
        }

        public override void DoDamage(DamageInfo info)
        {
            hp -= info.dmg * thisDamageablePart.DamageMultiplier;


            if( CanDismember(info) )
            {
                Rigidbody bodyPart = CreateBodyPart();
                info.ApplyImpact( bodyPart );
                onDismember.Invoke(this);
            }
        }

        private bool CanDismember(DamageInfo info)
        {
            return info.canDismember && hp <= 0.0f && realBodyPart.gameObject.activeInHierarchy;
        }

        private Rigidbody CreateBodyPart()
        {
            Rigidbody bodyPart = Instantiate( bodyPartPrefab, transform.position, transform.rotation );
            DestroyUnnecesaryBodyParts(bodyPart.gameObject);

            realBodyPart.gameObject.SetActive( false );
            DisableConnectedBodyParts();
            return bodyPart;
        }

        private void DisableConnectedBodyParts()
        {
            for (int n = 0; n < connectedBodyParts.Length; n++)
            {
                connectedBodyParts[n].SetActive(false);
            }
        }

        //disable body parts that has been already dismember
        private void DestroyUnnecesaryBodyParts(GameObject bodyPart)
        {
           
            foreach ( Transform child in bodyPart.transform)
            {
               
                MeshFilter meshRender = child.GetComponent<MeshFilter>();

                if (meshRender != null)
                {
                    GameObject bodyPartGO = null;

                    if (bodyPartStateConnections.TryGetValue( meshRender.sharedMesh, out bodyPartGO ))
                    {                       
                        meshRender.gameObject.SetActive( bodyPartGO.activeInHierarchy );
                    }
                    
                }

            }
        }
    }
}

