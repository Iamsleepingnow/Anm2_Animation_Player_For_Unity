using System.Collections;
using System.Collections.Generic;
using Iamsleepingnow.Anm2Player;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player.Demos
{
    public class Demo_SimplePlayerController : MonoBehaviour
    {
        [BoxGroup("Components 组件")]
        [SerializeField] public AnmSprite playerSprite; // 玩家动画组件
        [BoxGroup("Components 组件")]
        [SerializeField] public Camera playerCam; // 玩家摄像机
        [BoxGroup("Components 组件")]
        [SerializeField] public Rigidbody playerRigidbody; // 玩家刚体
        [BoxGroup("Components 组件")]
        [SerializeField] public CapsuleCollider playerCapsule; // 玩家胶囊体碰撞箱
        [BoxGroup("Components 组件")]
        [SerializeField] public bool useUnityAnimator = false;
        [BoxGroup("Components 组件"), ShowIf("useUnityAnimator")]
        [SerializeField] public Animator playerAnimator; // 玩家动画机

        [BoxGroup("Settings 设置"), Range(0f, 10f)]
        [SerializeField] public float moveSpeed = 2f; // 移动速度
        [BoxGroup("Settings 设置"), Range(0f, 10f)]
        [SerializeField] public float jumpMoveSpeed = 3f; // 跳跃时移动速度
        [BoxGroup("Settings 设置"), Range(0f, 10f)]
        [SerializeField] public float jumpForce = 3.5f; // 跳跃力度
        [BoxGroup("Settings 设置"), Range(-30f, 0f)]
        [SerializeField] public float gravityForce = -16f; // 重力
        [BoxGroup("Settings 设置"), Range(0.001f, 1f)]
        [SerializeField] public float cameraFollowSpeed = 0.1f; // 相机跟随速度
        [BoxGroup("Settings 设置"), Range(0.01f, 1f)]
        [SerializeField] public float inAirAnimSpeed = 0.45f; // 空中动画速度
        [BoxGroup("Settings 设置")]
        [SerializeField] public float cameraOffsetY = 2.5f; // 相机Y轴偏移
        [BoxGroup("Settings 设置")]
        [SerializeField] public float cameraOffsetZ = -3f; // 相机Z轴偏移
        [BoxGroup("Settings 设置")]
        [SerializeField] public float capsuleBaseHeight = 0.6f; // 胶囊体高度

        [Button("回到默认设置 Back to Default")]
        private void __DebugSetPropsToDefault() {
            moveSpeed = 2f;
            jumpMoveSpeed = 3f;
            jumpForce = 3.5f;
            gravityForce = -16f;
            cameraFollowSpeed = 0.1f;
            inAirAnimSpeed = 0.45f;
            cameraOffsetY = 2.5f;
            cameraOffsetZ = -3f;
            capsuleBaseHeight = 0.6f;
        }

        private Vector3 cameraTargetPosition; // 相机目标位置
        private Vector3 capsuleTargetCenter; // 胶囊体目标中心点
        private bool isFlipped = false; // 当前翻转状态
        private bool isInJumpingAnimation = false; // 是否正在跳跃动画中
        private bool isInJumpingEvent = false; // 是否正在跳跃事件中
        private bool isInMeleeAnimation = false; // 是否正在攻击动画中

        // 输入状态
        private bool inputW = false;
        private bool inputS = false;
        private bool inputA = false;
        private bool inputD = false;

        // 动画状态
        private enum PlayerState
        {
            Idle, // 待机
            Run, // 移动
            Jump, // 跳跃
            Melee, // 近战攻击
        }

        private PlayerState currentState = PlayerState.Idle; // 当前动画状态

        void Start() {
            playerCam = playerCam == null ? Camera.main : playerCam;
            // 初始化相机位置
            if (playerCam != null) {
                playerCam.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + cameraOffsetY,
                    transform.position.z + cameraOffsetZ
                );
            }
            if (playerCapsule != null) {
                playerCapsule.height = capsuleBaseHeight;
                playerCapsule.center = new Vector3(playerCapsule.center.x, capsuleBaseHeight / 2, playerCapsule.center.z);
                capsuleTargetCenter = playerCapsule.center;
            }
            if (playerSprite != null) {
                playerSprite.SetCurrentAnimation("Idle"); // 设置初始动画
                //
                playerSprite.SubscribeOnEventTriggered((anm, eventName) => { // 监听事件触发
                    if (eventName == "Jump") {
                        isInJumpingEvent = true;
                        JumpRigidbody(); // 跳跃刚体
                        playerSprite.SetPlayBackSpeed(inAirAnimSpeed); // 设置滞空时播放速度
                    }
                    else if (eventName == "JumpIdle") {
                        anm.Pause();
                    }
                    else if (eventName == "JumpToGround") {
                        isInJumpingEvent = false;
                    }
                }, false);
                //
                playerSprite.SubscribeOnAnimationCompleted((anm, animName, animIndex, isLoop) => { // 监听动画完成
                    if (animName == "Jump") { // 跳跃动画完成时
                        if (playerAnimator != null && useUnityAnimator) {
                            playerAnimator.SetBool("Jump", false); // 切换回待机动画
                        }
                        else {
                            IdleAnimation(); // 切换回待机动画
                        }
                        playerSprite.SetPlayBackSpeed(1f); // 恢复播放速度
                    }
                    else if (animName == "Melee") { // 攻击动画完成时
                        if (playerAnimator != null && useUnityAnimator) {
                            playerAnimator.SetBool("Melee", false); // 切换回待机动画
                        }
                        else {
                            IdleAnimation(); // 切换回待机动画
                        }
                    }
                });
                //
                playerSprite.SubscribeOnAnimationChanged((anm, animName, animIndex) => { // 监听动画切换
                    isInJumpingAnimation = animName == "Jump";
                    isInMeleeAnimation = animName == "Melee";
                });
                //
                playerSprite.SubscribeOnFrameUpdate((anm, progress) => { // 监听动画帧更新
                    if (anm.GetCurrentAnimationName() == "Jump") { // 跳跃动画更新时
                        capsuleTargetCenter = GetCapsuleCenterFromSpriteRoot(); // 获取胶囊体中心点
                    }
                });
            }
        }

        void Update() {
            if (playerSprite == null) return;
            // 获取输入
            inputW = Input.GetKey(KeyCode.W);
            inputS = Input.GetKey(KeyCode.S);
            inputA = Input.GetKey(KeyCode.A);
            inputD = Input.GetKey(KeyCode.D);
            // 处理跳跃
            if (Input.GetKeyDown(KeyCode.Space) && !isInJumpingEvent && !isInMeleeAnimation) {
                if (playerAnimator != null && useUnityAnimator) {
                    playerAnimator.SetBool("Jump", true); // 跳跃动画
                }
                else {
                    JumpAnimation(); // 跳跃动画
                }
            }
            // 处理攻击
            if (Input.GetMouseButtonDown(0) && !isInMeleeAnimation && !isInJumpingEvent) {
                if (playerAnimator != null && useUnityAnimator) {
                    playerAnimator.SetBool("Melee", true); // 攻击动画
                }
                else {
                    MeleeAnimation(); // 攻击动画
                }
            }
        }

        void FixedUpdate() {
            playerRigidbody.velocity += new Vector3(0, gravityForce / 100f, 0); // 加重力
            CameraFollow(); // 平滑跟随
            HandleMovementAndAnimation(); // 处理移动和动画切换
            if (playerCapsule != null) {
                playerCapsule.center = Vector3.Lerp(playerCapsule.center, capsuleTargetCenter, 0.5f); // 平滑移动
            }
        }
    
        void OnCollisionStay(Collision collision) { // 检测碰撞
            // 当玩家落地时
            if (playerSprite != null) {
                if (playerSprite.IsPaused) {
                    playerSprite.Play(); // 恢复动画
                    List<int> eventAtFrames =
                        playerSprite.CurrentAnmFile.GetEventTriggeredFrames(
                            playerSprite.GetCurrentAnimationIndex(),
                            "JumpToGround"); // 获取事件在当前动画中的所有触发帧
                    if (eventAtFrames == null) return;
                    if (eventAtFrames.Count > 0) {
                        playerSprite.SetCurrentFrameIndex(eventAtFrames[0]); // 跳转到跳跃动画着地帧
                        isInJumpingEvent = false;
                    }
                    capsuleTargetCenter = GetCapsuleCenterFromSpriteRoot(); // 获取胶囊体中心点
                    if (playerCapsule != null) {
                        playerCapsule.center = capsuleTargetCenter; // 直接设置，不适用插值
                    }
                }
            }
        }

        void CameraFollow() { // 相机平滑跟随
            if (playerCam != null) {
                cameraTargetPosition = new Vector3(
                    transform.position.x,
                    transform.position.y + cameraOffsetY,
                    transform.position.z + cameraOffsetZ
                );
                playerCam.transform.position = Vector3.Lerp(
                    playerCam.transform.position,
                    cameraTargetPosition,
                    cameraFollowSpeed
                );
            }
        }

        void HandleMovementAndAnimation() { // 处理移动和动画切换
            if (playerSprite == null || isInMeleeAnimation || (isInJumpingEvent && currentState != PlayerState.Jump)) { return; }
            float moveX = 0f; // 移动X轴方向
            float moveZ = 0f; // 移动Z轴方向
            bool isRunning = false;
            // 处理前后移动
            if (inputW || inputS) {
                moveZ = inputW ? 1 : -1;
                isRunning = true;
            }
            // 处理左右移动
            if (inputA || inputD) {
                moveX = inputD ? 1 : -1;
                isRunning = true;
                isFlipped = inputA;
                SpriteHorizontalFlip(isFlipped); // 设置翻转
            }
            // 地面移动控制
            if (!isInJumpingAnimation && !isInMeleeAnimation) {
                // 更新动画状态
                if (isRunning && currentState != PlayerState.Run) {
                    if (playerAnimator != null && useUnityAnimator) {
                        playerAnimator.SetBool("Run", true); // 移动动画
                    }
                    else {
                        RunAnimation(); // 移动动画
                    }
                }
                else if (!isRunning && currentState != PlayerState.Idle) {
                    if (playerAnimator != null && useUnityAnimator) {
                        playerAnimator.SetBool("Run", false); // 待机动画
                    }
                    else {
                        IdleAnimation(); // 待机动画
                    }
                }
            }
            // 执行移动
            if (!isInJumpingAnimation) {
                if (isRunning) {
                    Vector3 move = moveSpeed * Time.deltaTime * new Vector3(moveX, 0, moveZ).normalized;
                    playerRigidbody.MovePosition(transform.position + move);
                }
            }
            else if (isInJumpingAnimation && isInJumpingEvent) {
                if (isRunning) {
                    Vector3 move = jumpMoveSpeed * Time.deltaTime * new Vector3(moveX, 0, moveZ).normalized;
                    playerRigidbody.MovePosition(transform.position + move);
                }
            }
        }

        public void IdleAnimation() { // 待机动画
            currentState = PlayerState.Idle;
            playerSprite.SetCurrentAnimation("Idle"); // 切换回待机动画
        }

        public void RunAnimation() { // 移动动画
            currentState = PlayerState.Run;
            playerSprite.SetCurrentAnimation("Run"); // 切换到移动动画
        }

        public void JumpAnimation() { // 跳跃动画
            currentState = PlayerState.Jump;
            playerSprite.SetCurrentAnimation("Jump");
        }

        void JumpRigidbody() { // 跳跃刚体
            if (playerRigidbody != null) {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.y, jumpForce); // 应用向上的力
            }
        }

        public void MeleeAnimation() { // 攻击动画
            currentState = PlayerState.Melee;
            playerSprite.SetCurrentAnimation("Melee");
        }

        Vector3 GetCapsuleCenterFromSpriteRoot() { // 获取胶囊体中心点
            if (playerCapsule != null && playerSprite != null) {
                float layerAltitude = playerSprite.GetRootLayer().transform.localPosition.y; // 获取动画的根层高度
                return new Vector3(playerCapsule.center.x, capsuleBaseHeight / 2 + layerAltitude, playerCapsule.center.z);
            }
            return Vector3.zero;
        }

        void SpriteHorizontalFlip(bool isFlip) { // 设置动画横向翻转
            if (playerSprite != null) {
                Vector3 origionalScale = playerSprite.transform.localScale;
                playerSprite.transform.localScale = new Vector3((isFlip ? -1 : 1) * Mathf.Abs(origionalScale.x), origionalScale.y, origionalScale.z);
            }
        }
    }
}

