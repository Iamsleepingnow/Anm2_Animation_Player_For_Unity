using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    [AddComponentMenu("Anm2Player/Anm Comment")]
    public class AnmComment : MonoBehaviour
    {
#if UNITY_EDITOR
        [Multiline(50)]
        [HideInInspector] public string comment = "...";
        
        [HideInInspector] public bool inEdit = false;
#endif
    }
}