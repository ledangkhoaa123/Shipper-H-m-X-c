using UnityEngine;

public class ShipperControl : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float speed = 5f;              // Tốc độ chạy
    public float mouseSensitivity = 200f; // Độ nhạy chuột để xoay camera

    [Header("Cài đặt Nhảy & Trọng lực")]
    public float jumpHeight = 1.5f;       // Độ cao nhảy (mét)
    public float gravity = -9.81f;        // Trọng lực (hút xuống đất)

    [Header("Các thành phần (Tự động tìm)")]
    public Transform cam;                 // Camera chính
    private CharacterController controller;
    private Animator anim;

    // Biến nội bộ để tính toán
    float xRotation = 0f;
    Vector3 velocity;      // Lực rơi tự do
    bool isGrounded;       // Kiểm tra chân chạm đất

    void Start()
    {
        // Tự động tìm các bộ phận trên người nhân vật
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Nếu quên kéo Camera vào thì tự tìm Main Camera
        if (cam == null)
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogError("❌ LỖI: Không tìm thấy Camera nào! Hãy gắn Main Camera vào ô Cam.");
            }
        }

        // Giấu con trỏ chuột đi để xoay cho dễ
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- 1. XỬ LÝ TRỌNG LỰC & CHẠM ĐẤT ---
        // Kiểm tra xem nhân vật có đang đứng trên đất không
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ép nhẹ xuống đất để không bị lơ lửng
            anim.SetBool("IsJumping", false); // Tắt trạng thái nhảy trong Animator
        }

        // --- 2. XOAY CAMERA (Góc nhìn) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay người sang trái/phải
        transform.Rotate(Vector3.up * mouseX);

        // Ngước camera lên/xuống (Giới hạn 45 độ để không gãy cổ)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- 3. DI CHUYỂN (WASD) ---
        float x = Input.GetAxis("Horizontal"); // A, D
        float z = Input.GetAxis("Vertical");   // W, S

        // Hướng đi theo hướng mặt của nhân vật
        Vector3 move = transform.right * x + transform.forward * z;

        // Thực hiện di chuyển
        controller.Move(move * speed * Time.deltaTime);

        // --- 4. CẬP NHẬT ANIMATOR (BLEND TREE) ---
        // Gửi thông số InputX, InputZ sang Animator để nó tự trộn hành động
        // 0.1f là độ trễ giúp chuyển động mượt mà hơn
        anim.SetFloat("InputX", x, 0.1f, Time.deltaTime);
        anim.SetFloat("InputZ", z, 0.1f, Time.deltaTime);

        // --- 5. XỬ LÝ NHẢY (JUMP) ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Công thức vật lý để tính lực bật nhảy: v = căn(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // Báo cho Animator biết là đang nhảy
            anim.SetBool("IsJumping", true);
        }

        // Áp dụng trọng lực rơi xuống
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}