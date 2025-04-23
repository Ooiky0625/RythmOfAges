using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VRBeats
{
    public class VR_BeatCubeSpawneable : Spawneable
    {
        [SerializeField] private GameObject arrow = null;
        [SerializeField] private GameObject dot = null;
        [SerializeField] private Spark sparkPrefab = null;        
        [SerializeField] private ColorSide colorSide = ColorSide.Left;
        [SerializeField] private bool useSpark = false;
        
        public ColorSide ColorSide { get { return colorSide; } }
        public Direction HitDirection { get { return hitDirection;  } }

        private Direction hitDirection = Direction.Center;

        public override void OnSpawn()
        {
            base.OnSpawn();

            if (useSpark)
            {
                if (sparkPrefab == null) return;
                
                Color desireColor = VR_BeatManager.instance.GetColorFromColorSide(colorSide);
                Spark spark = Instantiate(sparkPrefab, transform.position, Quaternion.identity);
                spark.transform.parent = transform;
                spark.Construct(desireColor);
            }
        }


        public override void Construct(SpawnEventInfo info)
        {
            base.Construct(info);
            
            transform.rotation = CalculateRotationFromDirection(info.hitDirection);
            colorSide = info.colorSide;
            useSpark = info.useSpark;
            hitDirection = info.hitDirection;

            //use the arrow of the center
            arrow.SetActive( info.hitDirection != Direction.Center );
            dot.SetActive( info.hitDirection == Direction.Center );

        }

        
        private Quaternion CalculateRotationFromDirection(Direction dir)
        {
            Vector3 rot = Vector3.zero;

            if (dir == Direction.Up)
            {
                rot = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else if (dir == Direction.UpperRight)
            {
                rot = new Vector3(0.0f, 0.0f, -45.0f);
            }
            else if (dir == Direction.Right)
            {
                rot = new Vector3(0.0f, 0.0f, -90.0f);
            }
            else if (dir == Direction.LowerRight)
            {
                rot = new Vector3(0.0f, 0.0f, -135.0f);
            }
            else if (dir == Direction.Down)
            {
                rot = new Vector3(0.0f, 0.0f, -180.0f);
            }

            else if (dir == Direction.LowerLeft)
            {
                rot = new Vector3(0.0f, 0.0f, -225.0f);
            }
            else if (dir == Direction.Left)
            {
                rot = new Vector3(0.0f, 0.0f, -270.0f);
            }
            else if (dir == Direction.UpperLeft)
            {
                rot = new Vector3(0.0f, 0.0f, -315.0f);
            }


            return Quaternion.Euler(rot);
        }

#if UNITY_EDITOR

        string[] textureNameArray = { "upperLeft", "up", "upperRight", "left", "center", "right", "lowerLeft", "down", "lowerRight" };
        Texture[] texArray = null;

        private bool updateHitDir = false;
        private bool updateUseSpark = false;
        private bool updateColorSide = false;


        private void LoadArrowTextures()
        {
           
            texArray = new Texture[textureNameArray.Length];
            for (int n = 0; n < texArray.Length; n++)
            {
                texArray[n] = Resources.Load("Editor/Arrow/" + textureNameArray[n]) as Texture;
            }
        }

        public override void CustomInspector(SpawnEventInfo info , Object[] targets)
        {
            EditorGUI.BeginChangeCheck();

            LoadArrowTextures();

            EditorGUI.BeginChangeCheck();
            info.hitDirection = DrawArrowGridInspector("" , info.hitDirection);
            if (EditorGUI.EndChangeCheck())
            {
                updateHitDir = true;
            }

            base.CustomInspector(info , targets);

            EditorGUI.BeginChangeCheck();
            info.useSpark = EditorGUILayout.Toggle("Use Spark" , info.useSpark);
            if (EditorGUI.EndChangeCheck())
            {
                updateUseSpark = true;
            }

            EditorGUI.BeginChangeCheck();
            info.colorSide = (ColorSide) EditorGUILayout.EnumPopup("Color Side" , info.colorSide);
            if (EditorGUI.EndChangeCheck())
            {
                updateColorSide = true;
            }

            /*manual assignment here, remember to check that the selected objects
                  are in fact of the appropriate type.*/
            foreach (Object obj in targets)
            {
                if (obj is VR_BeatSpawnMarker spawnMarker)
                {
                    if (updateHitDir)
                        spawnMarker.spawInfo.hitDirection = info.hitDirection;

                    else if (updateUseSpark)
                        spawnMarker.spawInfo.useSpark = info.useSpark;

                    else if (updateColorSide)
                        spawnMarker.spawInfo.colorSide = info.colorSide;
                }
            }

            updateHitDir = false;
            updateColorSide = false;
            updateUseSpark = false;

        }

        private Direction DrawArrowGridInspector(string label, Direction dir)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            GUILayout.BeginVertical("Box");

            dir = (Direction)GUILayout.SelectionGrid((int)dir, texArray, 3, GUILayout.ExpandWidth(false), GUILayout.MaxHeight(150f), GUILayout.MaxWidth(150f));
            GUILayout.EndVertical();

            return dir;
        }
#endif



    }

}

