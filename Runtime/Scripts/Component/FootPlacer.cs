using UnityEngine;

namespace HRYooba.Kinect
{
    [RequireComponent(typeof(Animator))]
    public class FootPlacer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        [Tooltip("地面のY座標からのオフセット。足を少し浮かせたり、めり込ませたりする場合に調整します。")]
        private float _offsetY = 0.05f;

        [SerializeField]
        [Tooltip("地面とみなすY座標。")]
        private float _groundY = 0.0f;

        private Animator _animator;
        private Transform _leftFoot;
        private Transform _rightFoot;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator component not found on this GameObject.", this);
                return;
            }

            // 各ボーンのTransformを取得
            _leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            _rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

            if (_leftFoot == null || _rightFoot == null)
            {
                Debug.LogError("Could not find foot bones. Make sure the model is a configured Humanoid.", this);
            }
        }

        // LateUpdateは、すべてのUpdate関数の呼び出し後にフレームごとに呼び出されます。
        // アニメーションの更新はこの間に行われるため、アニメーション適用後の最終的なボーン位置を取得するのに適しています。
        private void LateUpdate()
        {
            // 足のボーンが取得できていなければ何もしない
            if (_leftFoot == null || _rightFoot == null)
            {
                return;
            }

            // 1. 左右の足のワールド座標におけるY座標を取得
            float leftFootY = _leftFoot.position.y;
            float rightFootY = _rightFoot.position.y;

            // 2. より低い（地面に近い）方の足のY座標を特定
            float lowestFootY = Mathf.Min(leftFootY, rightFootY);

            // 3. 目標とするY座標を計算（地面のY座標 + オフセット）
            float targetY = _groundY + _offsetY;

            // 4. 移動が必要な差分を計算
            //    (目標のY座標) - (現在の最も低い足のY座標)
            float deltaY = targetY - lowestFootY;

            // 5. キャラクターのルートtransformのY座標に差分を加算して移動させる
            //    これにより、キャラクター全体が上下に移動し、足が地面に接地します。
            Vector3 newPosition = transform.position;
            newPosition.y += deltaY;
            transform.position = newPosition;
        }
    }
}