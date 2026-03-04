using UnityEngine;

public class CameraTPS : MonoBehaviour
{
    public Transform target; // Kéo nhân vật vào đây
    public float mouseSensitivity = 3f;
    public float distanceFromTarget = 5f; // Khoảng cách camera

    float rotationY;
    float rotationX;

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -40, 40); // Giới hạn nhìn lên xuống

        Vector3 direction = new Vector3(0, 0, -distanceFromTarget);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        // Vị trí camera = Vị trí nhân vật + Góc xoay * Khoảng lùi
        transform.position = target.position + Vector3.up * 1.5f + rotation * direction;
        transform.LookAt(target.position + Vector3.up * 1.5f); // Luôn nhìn vào vai/đầu nhân vật
    }
}