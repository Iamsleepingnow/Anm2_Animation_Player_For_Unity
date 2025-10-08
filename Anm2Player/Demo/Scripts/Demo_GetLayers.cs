using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player.Demos
{
    public class Demo_GetLayers : MonoBehaviour
    {
        [SerializeField] public AnmSprite anmSprite = null;
        [SerializeField] public GameObject tipPrefab = null;

        void Awake() {
            if (anmSprite != null) {
                anmSprite.SubscribeOnFileLoaded(anm =>
                {
                    List<GameObject> spawnedObjects = new();
                    GameObject go = null;
                    // 通过AnmSprite.GetRootLayer()获取Root层
                    // 通过AnmSprite.GetNullLayer()获取Null层
                    // 通过AnmSprite.GetSpriteLayer()获取Sprite层
                    go = Instantiate(tipPrefab, anm.GetNullLayer("LeftHand").transform); // anm.GetNullLayer(0)
                    spawnedObjects.Add(go);
                    go = Instantiate(tipPrefab, anm.GetNullLayer("RightHand").transform); // anm.GetNullLayer(1)
                    spawnedObjects.Add(go);
                    go = Instantiate(tipPrefab, anm.GetNullLayer("LeftFeet").transform); // anm.GetNullLayer(2)
                    spawnedObjects.Add(go);
                    go = Instantiate(tipPrefab, anm.GetNullLayer("RightFeet").transform); // anm.GetNullLayer(3)
                    spawnedObjects.Add(go);
                    //
                    foreach (var g in spawnedObjects) {
                        g.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                        g.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    }
                    //
                    anm.GetSpriteLayer("CharacterJump").SelfMeshRenderer.sharedMaterial.SetColor("_EmissionColor", new Color(0.25f, 0f, 1f) * 1.25f);
                });
            }
        }
    }
}