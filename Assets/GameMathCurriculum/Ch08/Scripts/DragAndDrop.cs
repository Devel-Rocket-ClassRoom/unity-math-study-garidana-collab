using System.Collections;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    // 드롭이 가능한 영역 레이어
    [SerializeField] private LayerMask groundLayer;
    // 드래그 가능한 오브젝트 레이어
    [SerializeField] private LayerMask draggableLayer;
    // 드래그할 때 오브젝트가 지면에서 떠있는 높이
    [SerializeField] private float heightOffset = 0.5f;

    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private int pathSampleCount = 20;


    private GameObject selectedObject; // 선택된 오브젝트
    private Vector3 originPosition; // 원래 위치 저장

    private void Update()
    {
        // 마우스 좌클릭시 TryPickUp 메서드 호출
        if (Input.GetMouseButtonDown(0)) TryPickUp();
        if (Input.GetMouseButton(0)) Drag();
        // 마우스 좌클릭 뗄 시 Drop 메서드 호출
        if (Input.GetMouseButtonUp(0)) Drop();
    }

    private void TryPickUp()
    {
        // 마우스 위치에서 레이 발사
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 레이가 Terrain 레이어에 닿았는지 확인
        if (Physics.Raycast (ray, out RaycastHit hit, Mathf.Infinity, draggableLayer))
        {
            // 닿았다면 selectedObject에 해당 게임 오브젝트 할당
            selectedObject = hit.collider.gameObject;
            originPosition = selectedObject.transform.position;
            // 디버그 로그로 선택된 오브젝트 이름 출력
            Debug.Log($"클릭됨 : {selectedObject.name}");
        }
    }

    private void Drag()
    {
        if (selectedObject == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPos = hit.point + Vector3.up * heightOffset;
            selectedObject.transform.position = targetPos;
        }
    }

    private void Drop()
    {
        if (selectedObject == null) return;

        Collider[] overlaps = Physics.OverlapBox(
            selectedObject.transform.position,
            selectedObject.transform.localScale * 0.5f
        );

        DropZone dropZone = null;
        foreach(Collider col in overlaps)
        {
            dropZone = col.GetComponent<DropZone>();
            if (dropZone != null) break;
        }

        if (dropZone != null && !dropZone.isDropped)
        {
            selectedObject.transform.position = dropZone.transform.position + Vector3.up * heightOffset;
            dropZone.isDropped = true;
            Debug.Log($"배치 성공 : {selectedObject.name}");
        }
        else
        {
            StartCoroutine(ReturnToOrigin(selectedObject, originPosition));
            Debug.Log($"배치 실패 -> 원위치 복귀 : {selectedObject.name}");
        }
        selectedObject = null;
    }
    private IEnumerator ReturnToOrigin(GameObject obj, Vector3 destination)
    {
        Vector3 startPos = obj.transform.position;

        float elapsed = 0f;
        float duration = Vector3.Distance(startPos, destination) / returnSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 flatPos = Vector3.Lerp(startPos, destination, t);


            float terrainHeight = Terrain.activeTerrain.SampleHeight(flatPos);
            flatPos.y = terrainHeight + heightOffset;

            obj.transform.position = flatPos;
            yield return null;
        }
        obj.transform.position = destination;
    }
    
}
