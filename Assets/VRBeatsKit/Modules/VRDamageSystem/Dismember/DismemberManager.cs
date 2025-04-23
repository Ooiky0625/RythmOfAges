using UnityEngine;
using System.Collections.Generic;

namespace DamageSystem.Dismember
{
    public enum BodyPartType
    {
        Head,
        LeftArm,
        LeftForeArm,
        RightArm,
        RightForeArm,
        RightLeg,
        RightCalf,
        LeftLeg,
        LeftCalf
    }

    public class DismemberManager : MonoBehaviour
    {
        [SerializeField] private List<DismemberPart> dismembertPartList = null;

        private DamageableManager damageableManager = null;

        private void Awake()
        {
            damageableManager = GetComponent<DamageableManager>();

            for (int n = 0; n < dismembertPartList.Count; n++)
            {
                dismembertPartList[n].SetHP(damageableManager.HP);
                dismembertPartList[n].OnDismember.AddListener( OnDismember );
            }
        }

        private void OnDismember(DismemberPart part)
        {
            if(part.KillOnDismember)
            {
                damageableManager.Kill();
            }
        }
               
    }
}

