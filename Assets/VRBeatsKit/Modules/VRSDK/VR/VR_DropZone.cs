using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRSDK.Events;

namespace VRSDK
{

    public enum DropZoneMode
    {
        Collider,
        Distance
    }

    //this script handles the dropzones
    public class VR_DropZone : MonoBehaviour
    {        
        [SerializeField] private DropZoneMode dropZoneMode = DropZoneMode.Distance;
        [SerializeField] private Transform dropPoint = null;
        [SerializeField] private Collider[] dropZoneColliderArray = new Collider[2];
        [SerializeField] private VR_Grabbable startingDrop = null;
        [SerializeField] private bool shouldFly = false;
        [SerializeField] private float flyTime = 0.1f;       
        [SerializeField] private bool syncronizePosition = true;
        [SerializeField] private bool syncronizeRot = true;
        [SerializeField] private bool disableCollidersOnDrop = false;
        [SerializeField] private float dropRadius = 0.0f;
        [SerializeField] private bool usePreview = false;
        [SerializeField] private bool canStack = true;
        [SerializeField] private OnDropStateChangeEvent onDrop = null;
        [SerializeField] private OnDropStateChangeEvent onUndrop = null;


        [SerializeField] private List<VR_Grabbable> insideCollider = new List<VR_Grabbable>();
        [SerializeField] private List<VR_Grabbable> trackedGrabbable = new List<VR_Grabbable>();
        private List<VR_Grabbable> grabbableList = new List<VR_Grabbable>();
        private List<VR_Grabbable> droppedGrabbableList = new List<VR_Grabbable>();
        private VR_TagFilter tagFilter = null;    
       
        private GameObject preview = null;
        private GameObject previewOwner = null;
        private Material transparentMat = null;
        private Dictionary<VR_Grabbable , UnityAction<GrabState>> onGrabStateChangeConnections = new Dictionary<VR_Grabbable, UnityAction<GrabState>>();
        private Dictionary<VR_Grabbable, UnityAction<GrabState>> onGrabStateChangeTrackConnections = new Dictionary<VR_Grabbable, UnityAction<GrabState>>();
        private VR_Controller rightController = null;
        private VR_Controller leftController = null;

       
        public VR_Grabbable StartingDrop { get { return startingDrop; } }

        public Transform DropPoint { get { return dropPoint; } }
        public OnDropStateChangeEvent OnDrop { get { return onDrop; } }
        public OnDropStateChangeEvent OnUnDrop { get { return onUndrop; } }
        public float FlyTime { get { return flyTime; } }
        public bool ShouldFly { get { return shouldFly; } }

        public List<VR_Grabbable> DroppedGrabbableList { get { return droppedGrabbableList; } }
        public bool IsEmpty { get { return droppedGrabbableList.Count == 0; } }

        public bool debugMode = false;

        private void Awake()
        {
            //get the tag filter if we have one, this help us know what objects can be dropped in this dropzone
            tagFilter = GetComponent<VR_TagFilter>();

            //load the transparent material, just in case we want to use preview mode
            transparentMat = Resources.Load( "TransparentMaterial" ) as Material;

            //set initial drop grabbable
            if (startingDrop != null)
            {
                OnGrabStateChange( GrabState.Drop, startingDrop );
            }

            onDrop.AddListener(OnThisDropStateChange);
        }
        
        private void Start()
        {
            grabbableList = VR_Manager.instance.GrabbableList;
            rightController = VR_Manager.instance.Player.RightController;
            leftController = VR_Manager.instance.Player.LeftController;
        }

        private void OnColliderGrabbableEnter(VR_Grabbable grabbable)
        {
            if (grabbable != null && !insideCollider.Contains( grabbable ))
            {
                insideCollider.Add(grabbable);
            }
        }

        private void OnColliderGrabbableExit(VR_Grabbable grabbable)
        {
            if (grabbable != null && insideCollider.Contains( grabbable ))
            {
                insideCollider.Remove( grabbable );
            }
        }

        private void OnThisDropStateChange(VR_Grabbable grabbable)
        {
            if (grabbable == null || !disableCollidersOnDrop)
                return;

            Collider[] collideraArray = grabbable.GetComponentsInChildren<Collider>();

            for (int n = 0; n < collideraArray.Length; n++)
            {
                collideraArray[n].enabled = false;
            }
        }

        private void Update()
        {
            RemoveEmptyFromDroppedList();
            ProcessGrabbableList();

            if (dropZoneMode == DropZoneMode.Collider)
            {
                UpdateColliderDropzone();
            }
        }

        private void UpdateColliderDropzone()
        {
            VR_Grabbable rightGrabbable = VR_Manager.instance.Player.RightController.CurrentGrab;
            VR_Grabbable leftGrabbable = VR_Manager.instance.Player.LeftController.CurrentGrab;
            
            foreach (var collider in dropZoneColliderArray)
            {    
                UpdateGrabbableDropState(collider, rightGrabbable, rightController.Position);
                UpdateGrabbableDropState(collider, leftGrabbable, leftController.Position);
            }
        }

        private void UpdateGrabbableDropState(Collider collider, VR_Grabbable grabbable, Vector3 handPosition)
        {
            if(collider == null || grabbable == null) return;
            
            Vector3 closestPoint = collider.ClosestPoint(handPosition);
            
            if ((handPosition - closestPoint).magnitude < Mathf.Epsilon)
            {
                OnColliderGrabbableEnter(grabbable);
            }
            else
            {
                OnColliderGrabbableExit(grabbable);
            }
        }

        private void RemoveEmptyFromDroppedList()
        {
            for (int n = 0; n < droppedGrabbableList.Count; n++)
            {
                if (droppedGrabbableList[n] == null)
                {
                    droppedGrabbableList.RemoveAt( n );
                    n--;
                }
            }
        }

        private void ProcessGrabbableList()
        {
            for (int n = 0; n < grabbableList.Count; n++)
            {
                if (grabbableList[n].CanUseDropZone)
                {
                    ProccessGrabbable( grabbableList[n] );
                }
                
            }
        }
       

        private void ProccessGrabbable(VR_Grabbable grabbable)
        {
            if (!IsGrabbableTagValid( grabbable ))
            {
               
                return;
            }
           
            bool insideRange = IsInsideDropRange(grabbable);

            //if the we dont have any object dropped and we are in range
            if (insideRange && CanDropGrabbable( grabbable ))
            {
                HandleGrabbableEnterDropzone(grabbable);                
            }
            //if the object is outside range and we was tracking it
            else if (!insideRange && trackedGrabbable.Contains( grabbable ))
            {
                OnGrabbableExit( grabbable );
            }
        }

        /// <summary>
        /// can this grabbable be dropped in the dropzone?
        /// </summary>       
        private bool IsGrabbableTagValid(VR_Grabbable grabbable)
        {
            if (droppedGrabbableList.Count > 0)
            {
                bool canStack = CanStackThisGrabbable( grabbable );

                
                return canStack;
            }

            //if we have a tagfilter and the grabbable dont has a VR_Tag component
            if (tagFilter != null && grabbable.GrabbableTag == null)
                return false;

            //if we have a tagfilter and the tag of the grabbable dont match in the tag filter ignore it
            if (grabbable.GrabbableTag != null && tagFilter != null && !tagFilter.Check( grabbable.GrabbableTag.TagEnum ))
                return false;

            return true;
        }

        private bool CanStackThisGrabbable(VR_Grabbable grabbable)
        {
            if (droppedGrabbableList.Count == 0)
                return true;

            if (!canStack)
            {
                return false;
            }

            VR_Grabbable droppedGrabbble = droppedGrabbableList[0];

            if (droppedGrabbble.GrabbableTag == null || grabbable.GrabbableTag == null)
                return false;


            //you can just stack objects that are of the same type
            return droppedGrabbble.GrabbableTag.TagEnum == grabbable.GrabbableTag.TagEnum;
        }

        private bool IsInsideDropRange(VR_Grabbable grabbable)
        {
            bool insideRange = false;            

            if (dropZoneMode == DropZoneMode.Distance)
            {
                float distance = ( grabbable.transform.position - dropPoint.position ).magnitude;
                insideRange = distance < dropRadius;
            }
            else
            {
                insideRange = insideCollider.Contains( grabbable );
            }

            return insideRange;
        }

        private bool CanDropGrabbable(VR_Grabbable grabbable)
        {
            if (grabbable == null || grabbable.GetComponent<BlockDrop>() != null)
                return false;

            return droppedGrabbableList.Count == 0 || CanStackThisGrabbable(grabbable);
        }    

        private void HandleGrabbableEnterDropzone(VR_Grabbable grabbable)
        {
            
            //if the current grabbable is no being affected by any other dropzones juts call to grabbable enter
            if (grabbable.AffectedDropZone == null)
            {
                OnGrabbableEnter( grabbable );
            }

            //if the grabbable is being affected by any other dropzone
            else if (grabbable.AffectedDropZone != this && ShouldUpdateAffectedDropZone(grabbable) )
            {            
                grabbable.AffectedDropZone.OnGrabbableExit( grabbable );
                OnGrabbableEnter( grabbable );
            }
        }

        public void OnGrabbableExit(VR_Grabbable grabbable)
        {

            if (grabbable != null && droppedGrabbableList.Contains(grabbable) )
            {                
                Transform parent = grabbable.transform.parent;
                grabbable.transform.parent = null;
                grabbable.transform.localScale = CalculateOriginalScale( grabbable );
                grabbable.transform.parent = parent;
            }

            if (grabbable.AffectedDropZone == this)
            {
                grabbable.AffectedDropZone = null;
            }           
           
            StopTrackingGrabbable( grabbable );

            if (previewOwner != null && previewOwner.gameObject == grabbable.gameObject)
            {
                DestroyPreview( grabbable );
                VR_Grabbable closerGrabbable = GetCloserGrabbable();

                if (closerGrabbable != null)
                {
                    CreatePreviewFor( closerGrabbable );
                }               
            }
        }

        private VR_Grabbable GetCloserGrabbable()
        {
            float minDistance = float.MaxValue;
            VR_Grabbable closerGrabbable = null;

            for (int n = 0; n < trackedGrabbable.Count; n++)
            {
                float d = Vector3.Distance( trackedGrabbable[n].transform.position , dropPoint.position );

                if (d < minDistance)
                {
                    closerGrabbable = trackedGrabbable[n];
                    minDistance = d;
                }

            }

            return closerGrabbable;
        }

        private Vector3 CalculateOriginalScale(VR_Grabbable grabbable)
        {
            DropZoneInfo info = grabbable.GetComponent<DropZoneInfo>();

            return info == null ? grabbable.transform.localScale : info.OriginalScale ;
        }
       

        private void StopTrackingGrabbable(VR_Grabbable grabbable)
        {
            if (grabbable != null && trackedGrabbable.Contains( grabbable ))
            {
               
                trackedGrabbable.Remove( grabbable );
                grabbable.OnGrabStateChange.RemoveListener( onGrabStateChangeTrackConnections[grabbable] );
                onGrabStateChangeTrackConnections.Remove(grabbable);
            }
        }

        private void DestroyPreview(VR_Grabbable grabbable)
        {
            if (previewOwner == grabbable.gameObject)
                Destroy( preview );
        }

        private bool ShouldUpdateAffectedDropZone(VR_Grabbable grabbable)
        {
            float distanceToThisDropZone = ( grabbable.transform.position - dropPoint.position ).magnitude;
            float distanceToOtherDropzone = ( grabbable.AffectedDropZone.DropPoint.position - grabbable.transform.position ).magnitude;
           
            return distanceToThisDropZone < distanceToOtherDropzone - 0.001f;            
        }

        public void OnGrabbableEnter(VR_Grabbable grabbable, bool force = false)
        {

            if (force)
            {
                ProcessDrop(grabbable);
            }


            if (( grabbable != null && grabbable.CurrentGrabState == GrabState.Grab ) || force)
            {

               
                grabbable.AffectedDropZone = this;
                                
                StartTracking( grabbable );


                if (!CanStackThisGrabbable( grabbable ))
                    return;

                if (preview != null)
                    Destroy( preview );

                if (usePreview && droppedGrabbableList.Count == 0)
                {
                    CreatePreviewFor(grabbable);
                }
            }
        }

        private void CreatePreviewFor(VR_Grabbable grabbable)
        {
            previewOwner = grabbable.gameObject;
            DropZoneInfo dropZoneInfo = grabbable.GetComponent<DropZoneInfo>();

            preview = new GameObject(grabbable.gameObject.name + "_Preview");

           
            MeshFilter[] meshFilters = grabbable.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = grabbable.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;

                i++;
            }

            MeshFilter filter = preview.AddComponent<MeshFilter>();
            filter.mesh = new Mesh();
            filter.mesh.CombineMeshes( combine );

            MeshRenderer renderer = preview.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material = transparentMat;
            renderer.material.color = new Color( 1.0f, 1.0f, 1.0f, 0.5f );


            preview.transform.position = CalculateDropEndPosition(grabbable);
            preview.transform.rotation = CalculateDropEndRotation(grabbable);

            if (dropZoneInfo != null)
            {
                Vector3 scale = dropZoneInfo == null ? grabbable.transform.localScale : dropZoneInfo.OriginalScale;
                scale *= dropZoneInfo.ScaleModifier;
                preview.transform.localScale = scale;
            }
            else
            {
                preview.transform.localScale = grabbable.transform.lossyScale;
            }

            preview.transform.parent = dropPoint;

        }

        private Vector3 CalculateDropEndPosition(VR_Grabbable grabbable)
        {
            DropZoneInfo info = grabbable.GetComponent<DropZoneInfo>();

            return info == null ? dropPoint.position : dropPoint.position + info.PositionOffset;
        }

        private Quaternion CalculateDropEndRotation(VR_Grabbable grabbable)
        {
            DropZoneInfo info = grabbable.GetComponent<DropZoneInfo>();

            return info == null ? dropPoint.rotation : dropPoint.rotation * Quaternion.Euler( info.RotationOffset );
        }


        public void OnGrabStateChange(GrabState state, VR_Grabbable grabbable, bool force = false)
        {

            if (force || (state == GrabState.Drop && CanDropGrabbable(grabbable)) )
            {               
                Rigidbody rb = grabbable.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.isKinematic = true;

                ProcessDrop(grabbable);
                
                DropZoneInfo dropZoneInfo = grabbable.GetComponent<DropZoneInfo>();

               
                Vector3 dropScale = grabbable.transform.localScale;

                if (dropZoneInfo != null)
                {
                    dropScale = grabbable.transform.localScale * dropZoneInfo.ScaleModifier;
                }

                if (shouldFly)
                {

                    grabbable.enabled = false;

                    if (dropZoneInfo != null && dropZoneInfo.ScaleModifier != 1.0f)
                    {
                        StartCoroutine( ModifyScaleRoutine( grabbable.transform, dropScale, flyTime ) );
                    }

                    StartCoroutine( FlyRoutine( grabbable ) );
                }
                else
                {
                    onDrop.Invoke( grabbable );

                    if (syncronizePosition)
                        grabbable.transform.position = CalculateDropEndPosition(grabbable);
                    if (syncronizeRot)
                        grabbable.transform.rotation = CalculateDropEndRotation(grabbable);

                    if (preview != null)
                        preview.gameObject.SetActive( false );

                    if(dropZoneInfo != null)
                        grabbable.transform.localScale = dropScale;
                }


            }
            else if (droppedGrabbableList.Contains(grabbable) && state == GrabState.Grab)
            {

                if (preview != null)
                    preview.SetActive( true );

                OnGrabbableExit( grabbable );
            }

            else if (droppedGrabbableList.Contains(grabbable) && state == GrabState.Flying)
            {


                if ( ShouldModifyScale(grabbable) )
                {
                    DropZoneInfo dropzoneInfo = grabbable.GetComponent<DropZoneInfo>();

                    StartCoroutine( ModifyScaleRoutine( grabbable.transform, dropzoneInfo.OriginalScale, grabbable.GrabFlyTime ) );
                }                
            }
        }

        private void ProcessDrop(VR_Grabbable grabbable)
        {                      
            if (grabbable != null && !droppedGrabbableList.Contains(grabbable))
            {
                droppedGrabbableList.Add( grabbable );
                //grabbable.OnGrabStateChange.AddListener( delegate (GrabState state){ OnDroppedGrabbableGrabStateChange( grabbable, state ); } );

                UnityAction<GrabState> listener = delegate (GrabState state) { OnDroppedGrabbableGrabStateChange( grabbable, state ); };
                onGrabStateChangeConnections[grabbable] = listener;
                grabbable.OnGrabStateChange.AddListener(listener);


            }
        }

        private bool ShouldModifyScale(VR_Grabbable grabbable)
        {
            DropZoneInfo info = grabbable.GetComponent<DropZoneInfo>();

            return info != null && info.ScaleModifier != 1.0f;
        }

      

        private IEnumerator FlyRoutine(VR_Grabbable grabbable)
        {
            float elapseTime = 0.0f;

            Vector3 startPosition = grabbable.transform.position;
            Quaternion startRotation = grabbable.transform.rotation;

            while (elapseTime < FlyTime)
            {
                elapseTime += Time.deltaTime;
                float lerp = elapseTime / flyTime;


                if (syncronizePosition)
                    grabbable.transform.position = Vector3.Lerp( startPosition , CalculateDropEndPosition(grabbable), lerp );
                if (syncronizeRot)
                    grabbable.transform.rotation = Quaternion.Lerp( startRotation , CalculateDropEndRotation(grabbable), lerp );

                yield return new WaitForSeconds(Time.deltaTime);
            }

            onDrop.Invoke( grabbable );
            grabbable.enabled = true;

            if (syncronizePosition)
                grabbable.transform.position = CalculateDropEndPosition(grabbable);
            if (syncronizeRot)
                grabbable.transform.rotation = CalculateDropEndRotation(grabbable);

            if (preview != null)
                preview.gameObject.SetActive( false );

        }

        private IEnumerator ModifyScaleRoutine(Transform transform, Vector3 desireScale , float t)
        {
            float elapseTime = 0.0f;

            while (elapseTime < t)
            {
                elapseTime += Time.deltaTime;
                float lerp = elapseTime / t;
                transform.localScale = Vector3.Lerp( transform.localScale  , desireScale , lerp);

                yield return new WaitForEndOfFrame();
            }
        }


        

        private void StartTracking(VR_Grabbable grabbable)
        {
            if (!trackedGrabbable.Contains( grabbable ))
            {
               
                trackedGrabbable.Add( grabbable );

                UnityAction<GrabState> unityAction = delegate (GrabState state)
                {
                    if (trackedGrabbable.Contains( grabbable ))
                        OnGrabStateChange( state, grabbable );
                };

                grabbable.OnGrabStateChange.AddListener( unityAction );

                onGrabStateChangeTrackConnections[grabbable] = unityAction;


            }
        }

        //remove all the unnecessary code from the visual copies
        private void RemoveComponents(GameObject go)
        {
            Component[] componentArray = go.GetComponentsInChildren<Component>();

            for (int n = 0; n < componentArray.Length; n++)
            {
                if (componentArray[n] != null)
                {
                    if (componentArray[n] is Canvas)
                        Destroy( componentArray[n].gameObject );

                    else if ( CanDestroyComponent( componentArray[n] ) )
                        Destroy( componentArray[n] );
                }


            }
        }

        private bool CanDestroyComponent(Component c)
        {
            return !( c is Transform ) && !( c is MeshRenderer ) && !( c is MeshFilter ) && !(c is VR_Outline);
        }

       

        /// <summary>
        /// Called when the current dropped grabbable change his grab state
        /// </summary>
        /// <param name="state"></param>
        private void OnDroppedGrabbableGrabStateChange(VR_Grabbable grabbable , GrabState state)
        {
            if (state == GrabState.Grab)
            {

                onUndrop.Invoke( grabbable );

                //remove the listener               
                grabbable.OnGrabStateChange.RemoveListener( onGrabStateChangeConnections[grabbable] );
                OnGrabbableExit( grabbable );

                droppedGrabbableList.Remove(grabbable);
                onGrabStateChangeConnections.Remove(grabbable);

                if (dropZoneMode == DropZoneMode.Collider)
                {
                    RemoveFromInsideCollider( grabbable );
                }
                
            }


        }

        private void RemoveFromInsideCollider(VR_Grabbable grabbable)
        {
            insideCollider.Remove(grabbable);
            /*
            List<Collider> allColliders = insideCollider.Keys.ToList();

            for (int n = 0; n < allColliders.Count; n++)
            {
                if (insideCollider[allColliders[n]] == grabbable)
                {
                    insideCollider.Remove( allColliders[n] );
                }
            }*/
        }

        public void SetDropRadiusViaInspector(float radius)
        {
            dropRadius = radius;
        }



    }

}

