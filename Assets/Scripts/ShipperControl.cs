using UnityEngine;

public class ShipperControl : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float speed = 5f;              // Tốc độ chạy
    public float turnSpeed = 10f;         // Tốc độ xoay người theo Camera

    [Header("Cài đặt Nhảy & Trọng lực")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Các thành phần")]
    public Transform cam;                 // Kéo Main Camera vào đây
    private CharacterController controller;
    private Animator anim;

    // Biến nội bộ
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked; // Giấu chuột
    }

    void Update()
    {
        // --- 1. XỬ LÝ TRỌNG LỰC ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            anim.SetBool("IsJumping", false);
        }

        // --- 2. XOAY NHÂN VẬT THEO HƯỚNG CAMERA ---
        // Lấy góc Y của Camera
        float targetAngle = cam.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        // Xoay nhân vật từ từ về hướng Camera đang nhìn
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // --- 3. DI CHUYỂN (STRAFE) ---
        float x = Input.GetAxis("Horizontal"); // A, D (-1 đến 1)
        float z = Input.GetAxis("Vertical");   // W, S (-1 đến 1)

        // Tính hướng đi dựa trên hướng TRƯỚC MẶT và BÊN PHẢI của nhân vật
        // Vì nhân vật đã xoay theo camera ở bước 2, nên:
        // - transform.forward = Hướng Camera nhìn
        // - transform.right = Hướng phải của Camera
        Vector3 move = transform.right * x + transform.forward * z;

        // Thực hiện di chuyển
        controller.Move(move * speed * Time.deltaTime);

        // --- 4. CẬP NHẬT ANIMATOR (BLEND TREE 2D) ---
        // Truyền trực tiếp X và Z vào Animator để nó tự trộn:
        // Z > 0: Đi tới | Z < 0: Đi lùi
        // X > 0: Đi phải | X < 0: Đi trái
        anim.SetFloat("InputX", x, 0.1f, Time.deltaTime);
        anim.SetFloat("InputZ", z, 0.1f, Time.deltaTime);

        // --- 5. XỬ LÝ NHẢY ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetBool("IsJumping", true);
        }

        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}