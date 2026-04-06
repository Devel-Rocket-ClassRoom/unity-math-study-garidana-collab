// =============================================================================
// Assignment_PlanetOrbit.cs
// -----------------------------------------------------------------------------
// Matrix4x4.TRS와 MultiplyPoint3x4로 행성-위성 궤도를 구현하는 시스템
// =============================================================================

using UnityEngine;
using TMPro;
using UnityEngine.AI;
public class Assignment_PlanetOrbit : MonoBehaviour
{

    [Header("=== 행성 설정 ===")]
    [Tooltip("행성이 공전할 중심점")]
    [SerializeField] private Vector3 orbitCenter = Vector3.zero;

    [Tooltip("중심점을 기준으로 한 행성의 공전 반경")]
    [Range(3f, 15f)]
    [SerializeField] private float planetOrbitRadius = 5f;

    [Tooltip("행성의 공전 속도 (도/초)")]
    [Range(10f, 180f)]
    [SerializeField] private float planetOrbitSpeed = 30f;

    [Header("=== 위성 설정 ===")]
    [Tooltip("행성을 공전하는 위성 오브젝트")]
    [SerializeField] private Transform satellite;

    [Tooltip("행성을 기준으로 한 위성의 공전 반경")]
    [Range(1f, 5f)]
    [SerializeField] private float satelliteOrbitRadius = 2f;

    [Tooltip("위성의 공전 속도 (도/초)")]
    [Range(30f, 360f)]
    [SerializeField] private float satelliteOrbitSpeed = 90f;

    [Header("=== UI 연결 ===")]
    [Tooltip("궤도 정보를 표시할 TMP_Text")]
    [SerializeField] private TMP_Text uiText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [Tooltip("행성의 월드 좌표")]
    [SerializeField] private Vector3 planetWorldPos = Vector3.zero;

    [Tooltip("위성의 로컬 오프셋 (행성 기준, 행렬 계산 결과)")]
    [SerializeField] private Vector3 satelliteLocalPos = Vector3.zero;

    [Tooltip("위성의 월드 좌표")]
    [SerializeField] private Vector3 satelliteWorldPos = Vector3.zero;

    private void Update()
    {
        // TODO

        // 해답
        float planetAngle = Time.time * planetOrbitSpeed;

        Matrix4x4 planetOrbitMatrix = Matrix4x4.TRS(orbitCenter, Quaternion.Euler(0f, planetAngle, 0f),Vector3.one);

        transform.position = planetOrbitMatrix.MultiplyPoint(new Vector3(planetOrbitRadius, 0f, 0f));



        float satelliteAngle = Time.time * satelliteOrbitSpeed;

        Matrix4x4 satelliteOrbitMatrix = Matrix4x4.TRS (transform.position, Quaternion.Euler(0f, satelliteAngle, 0f), Vector3.one);

        satellite.position = satelliteOrbitMatrix.MultiplyPoint(new Vector3(satelliteOrbitRadius, 0f, 0f));


        planetWorldPos  = transform.position;
        satelliteLocalPos = satellite.localPosition;
        satelliteWorldPos = satellite.position;

        // // 행성의 공전 계산
        // float planetOrbitAngle = Time.time * planetOrbitSpeed;
        // // 행성의 TRS 행렬 생성
        // Matrix4x4 planetTRS = Matrix4x4.TRS(orbitCenter, Quaternion.Euler(0f, planetOrbitAngle, 0f), Vector3.one);
        // // 행성의 월드 좌표 계산
        // planetWorldPos = planetTRS.MultiplyPoint3x4(new Vector3(planetOrbitRadius, 5f, 0f));
        // transform.position = planetWorldPos;


        // // 위성 공전 각도 계산
        // float satelliteOrbitAngle = Time.time * satelliteOrbitSpeed;
        // // 위성 TRS 행렬 생성
        // Matrix4x4 satelliteTRS = Matrix4x4.TRS(planetWorldPos, Quaternion.Euler(0f, satelliteOrbitAngle, 0f), Vector3.one);
        // // 위성 월드 좌표 계산
        // satelliteWorldPos = satelliteTRS.MultiplyPoint3x4(new Vector3(satelliteOrbitRadius, 0f, 0f));
        // satellite.position = satelliteWorldPos;
        

        

        UpdateUI();
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        VectorGizmoHelper.DrawCircleXZ(orbitCenter, planetOrbitRadius, new Color(1f, 1f, 0f, 0.3f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(orbitCenter, transform.position);

        if (satellite != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, satellite.position);

            VectorGizmoHelper.DrawCircleXZ(transform.position, satelliteOrbitRadius,
                new Color(0f, 1f, 1f, 0.3f));
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(orbitCenter, 0.3f);
    }

    private void UpdateUI()
    {
        if (uiText == null) return;

        uiText.text =
            $"[과제] 행성-위성 궤도\n" +
            $"\n<color=yellow>행성 월드 좌표:</color>\n" +
            $"  ({planetWorldPos.x:F2}, {planetWorldPos.y:F2}, {planetWorldPos.z:F2})\n" +
            $"\n<color=cyan>위성 로컬 오프셋 (행성 기준):</color>\n" +
            $"  ({satelliteLocalPos.x:F2}, {satelliteLocalPos.y:F2}, {satelliteLocalPos.z:F2})\n" +
            $"\n<color=cyan>위성 월드 좌표:</color>\n" +
            $"  ({satelliteWorldPos.x:F2}, {satelliteWorldPos.y:F2}, {satelliteWorldPos.z:F2})";
    }
}
