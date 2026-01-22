using UnityEngine;

public class ShipperControl : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public float speed = 6f;              // Tốc độ chạy
    public float mouseSensitivity = 200f; // Độ nhạy chuột

    [Header("Các thành phần (Tự động tìm)")]
    public Transform cam;                 // Cái Camera
    private CharacterController controller;
    private Animator anim;

    float xRotation = 0f;

    void Start()
    {
        // --- BƯỚC 1: TỰ KIỂM TRA CÁC BỘ PHẬN ---

        // 1. Tìm cái Lồng (Character Controller)
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("❌ LỖI NGHIÊM TRỌNG: Nhân vật chưa có 'Character Controller'! Hãy Add Component này vào.");
        }

        // 2. Tìm bộ Hoạt hình (Animator)
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("❌ LỖI NGHIÊM TRỌNG: Nhân vật thiếu 'Animator'!");
        }

        // 3. Tìm Camera (Nếu bạn quên kéo thả)
        if (cam == null)
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogError("❌ LỖI CAM: Không tìm thấy Main Camera nào trong Scene cả (hoặc nó bị tắt/đổi tag)!");
            }
        }

        // Khóa chuột để xoay cho dễ
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- CHỐT CHẶN AN TOÀN ---
        // Nếu thiếu 1 trong 3 thứ này thì dừng ngay, không chạy tiếp để tránh lỗi dòng 50
        if (controller == null || anim == null || cam == null) return;


        // --- PHẦN 1: XOAY CAMERA (Góc nhìn) ---

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay người sang trái/phải theo chuột
        transform.Rotate(Vector3.up * mouseX);

        // Gật đầu Camera lên/xuống
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f); // Giới hạn không cho gãy cổ
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        // --- PHẦN 2: DI CHUYỂN (WASD) ---

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Hướng đi theo hướng mặt của nhân vật
        Vector3 move = transform.right * x + transform.forward * z;

        // Lệnh di chuyển
        controller.Move(move * speed * Time.deltaTime);


        // --- PHẦN 3: HOẠT HÌNH (Chạy/Đứng) ---

        if (move.magnitude > 0.1f)
        {
            anim.SetFloat("Speed", 1); // Chạy
        }
        else
        {
            anim.SetFloat("Speed", 0); // Đứng
        }
    }
}