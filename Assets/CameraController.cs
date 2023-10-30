using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 600.0f; // �������� ����������� ������

    private float zoomSpeed = 1000.0f; // �������� �����������/��������
    private float minZoomSize = 1620; // ����������� ������ ������ (��� out)
    private float maxZoomSize = 540; // ������������ ������ ������ (��� in)
    private float zoomSensitivity = 100;
    private float targetZoom;

    [SerializeField] private Camera cam;

    private void Start()
    {
        targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        // �������� ���� �� ����������
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // ������� ������ ��� �����������
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0.0f);

        // ���������� ������ �� ������ �����
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // �������� ������ ��������������� ������ � ����������� �� �����
        targetZoom -= Input.mouseScrollDelta.y * zoomSensitivity;
        targetZoom = Mathf.Clamp(targetZoom, maxZoomSize, minZoomSize);
        float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
        cam.orthographicSize = newSize;
    }
}