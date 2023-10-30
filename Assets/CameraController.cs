using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 600.0f; // Скорость перемещения камеры

    private float zoomSpeed = 1000.0f; // Скорость приближения/удаления
    private float minZoomSize = 1620; // Минимальный размер камеры (зум out)
    private float maxZoomSize = 540; // Максимальный размер камеры (зум in)
    private float zoomSensitivity = 100;
    private float targetZoom;

    [SerializeField] private Camera cam;

    private void Start()
    {
        targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        // Получаем ввод от клавиатуры
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Создаем вектор для перемещения
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0.0f);

        // Перемещаем камеру на основе ввода
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Изменяем размер ортографической камеры в зависимости от ввода
        targetZoom -= Input.mouseScrollDelta.y * zoomSensitivity;
        targetZoom = Mathf.Clamp(targetZoom, maxZoomSize, minZoomSize);
        float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
        cam.orthographicSize = newSize;
    }
}