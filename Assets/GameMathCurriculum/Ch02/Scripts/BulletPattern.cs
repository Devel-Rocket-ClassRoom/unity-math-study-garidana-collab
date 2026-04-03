// =============================================================================
// BulletPattern.cs
// -----------------------------------------------------------------------------
// 원형 탄막 패턴 발사
// =============================================================================

using UnityEngine;
using TMPro;
using UnityEditor.Callbacks;

public class BulletPattern : MonoBehaviour
{
    [Header("=== 탄막 설정 ===")]
    [Range(3, 36)]
    [SerializeField] private int bulletCount = 12; 
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireInterval = 1f;
    [SerializeField] private GameObject bulletPrefab;

    [Header("=== 회전 설정 ===")]
    [SerializeField] private float autoRotationSpeed = 0f;
    [SerializeField] private float bulletLifetime = 5f;

    [Header("=== UI 텍스트 ===")]
    [SerializeField] private TextMeshProUGUI uiText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [SerializeField] private float angleSpacing;
    [SerializeField] private float nextFireTime;
    [SerializeField] private float currentRotationOffset;

    private void Start()
    {
        // TODO
        angleSpacing = 360f / bulletCount;
        nextFireTime = Time.time + fireInterval;
    }

    private void Update()
    {
        currentRotationOffset = (Time.time * autoRotationSpeed) % 360f;

        if (Time.time >= nextFireTime)
        {
            FireBulletPattern();
            nextFireTime = Time.time + fireInterval;
        }

        UpdateUI();
    }

    /// <summary>
    /// 원형(폴라좌표) 배치를 이용해 일정 각도 간격으로 bulletCount만큼 탄환(총알)을 360도 방향으로 동시에 발사하는 함수.
    /// 주요 원리와 수학적/물리적 개념:
    /// - (1) 각 탄환은 중심(이 오브젝트) 기준으로, angleSpacing(=360도/bulletCount)씩 회전시켜 배치됩니다.
    /// - (2) 각 탄환의 발사 방향을 구할 때, 각도를 라디안(radian)으로 변환하여 x, z 성분에 삼각함수(cos/sin)를 적용합니다.
    ///   └ 원점 기준 반지름 r=1, 각도 θ에서 x=cosθ, z=sinθ의 점이 원 위에 위치합니다 ("단위원 parametric equation").
    ///   └ 회전 오프셋(currentRotationOffset)으로 전체 패턴 회전이 가능합니다.
    /// - (3) direction은 발사 방향(단위벡터), bulletSpeed를 곱해 Rigidbody의 선형속도(velocity)로 입힙니다.
    /// - (4) 각 탄환은 bulletLifetime 후 Destroy되어 자동 제거됩니다.
    /// </summary>
    private void FireBulletPattern()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // 각 탄환의 발사 각도 계산: (i번째) * 각도간격 + 전체 패턴 회전 Offset
            float angleDegree = (i * angleSpacing + currentRotationOffset) % 360;
            // 삼각함수 사용을 위해 도(degree) -> 라디안 변환
            float angleRadian = angleDegree * Mathf.Deg2Rad;

            // 단위원 Parametric 방정식: (cosθ, sinθ) -> 3D 방향 (XZ 평면)
            Vector3 direction = new Vector3(Mathf.Cos(angleRadian), 0f, Mathf.Sin(angleRadian)).normalized;

            // 탄환 생성 (위치: 중심, 회전은 없음)
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Rigidbody를 이용해 방향 * 속도로 '선형 속도' 적용 (사이클릭/등속 운동)
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = direction * bulletSpeed;

            // 일정 시간 후 탄환 파괴 (메모리 누수 방지)
            Destroy(bullet, bulletLifetime);
        }
    }

    private void UpdateUI()
    {
        if (uiText == null) return;

        uiText.text = $"<b>[원형 탄막 패턴]</b>\n" +
                     $"발사 개수: {bulletCount}\n" +
                     $"각도 간격: {angleSpacing:F1}°\n" +
                     $"총알 속도: {bulletSpeed}u/s\n" +
                     $"회전 오프셋: {currentRotationOffset:F1}°\n" +
                     $"다음 발사: {(nextFireTime - Time.time):F2}초";
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        if (!Application.isPlaying) return;

        float spacing = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angleDegrees = (i * spacing + currentRotationOffset) % 360f;
            float angleRadians = angleDegrees * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(angleRadians),
                0f,
                Mathf.Sin(angleRadians)
            ).normalized;

            VectorGizmoHelper.DrawArrow(
                transform.position,
                transform.position + direction * 2f,
                new Color(1f, i / (float)bulletCount, 0f, 1f)
            );
        }

        VectorGizmoHelper.DrawCircleXZ(transform.position, 2f, Color.gray, 32);
    }
}
