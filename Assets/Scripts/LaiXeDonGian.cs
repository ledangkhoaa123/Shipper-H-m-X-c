using UnityEngine;

public class LaiXeDonGian : MonoBehaviour
{
    [Header("--- 1. CÀI ĐẶT XE ---")]
    public float tocDoToiDa = 15f;
    public float giaToc = 8f;
    public float lucPhanh = 15f;
    public float doRe = 150f;
    public float giamTocTuNhien = 5f;

    [Header("--- 2. HIỆU ỨNG NGHIÊNG ---")]
    public Transform thanXe; 
    public float gocNghiengMax = 30f;
    public float tocDoNghieng = 5f;

    [Header("--- 3. GTA SETUP ---")]
    public GameObject nhanVatDiBo;
    public GameObject nhanVatLaiXe;
    public GameObject camDiBo;
    public GameObject camLaiXe;
    public Transform viTriXuongXe;
    public float khoangCachF = 3f;

    private Rigidbody rb;
    private float tocDoHienTai = 0f;
    private bool dangLaiXe = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // --- CODE FIX TỰ ĐỘNG KHÓA TRỤC ---
        // Tự động bỏ khóa Y (để quẹo được) và khóa chặt X, Z (để không lật/chúi đầu)
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // (Lưu ý: Không khóa Y ở đây để xe còn quay đầu được)

        XuongXe();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (dangLaiXe) XuongXe();
            else if (Vector3.Distance(transform.position, nhanVatDiBo.transform.position) < khoangCachF) LenXe();
        }

        if (!dangLaiXe) return;

        // Xử lý tốc độ
        float inputW = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Space))
            tocDoHienTai = Mathf.MoveTowards(tocDoHienTai, 0f, lucPhanh * Time.deltaTime);
        else if (inputW != 0)
            tocDoHienTai = Mathf.MoveTowards(tocDoHienTai, inputW * tocDoToiDa, giaToc * Time.deltaTime);
        else
            tocDoHienTai = Mathf.MoveTowards(tocDoHienTai, 0f, giamTocTuNhien * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!dangLaiXe) return;

        float inputA = Input.GetAxis("Horizontal");

        // 1. QUAY XE (Vật lý)
        if (Mathf.Abs(tocDoHienTai) > 0.1f)
        {
            float chieu = (tocDoHienTai > 0) ? 1f : -1f;
            float rotAmount = inputA * doRe * chieu * Time.fixedDeltaTime;

            // Xoay trục Y của TOÀN BỘ XE
            Quaternion turn = Quaternion.Euler(0f, rotAmount, 0f);
            rb.MoveRotation(rb.rotation * turn);
        }

        // 2. DI CHUYỂN (Ép hướng)
        Vector3 vanTocMoi = transform.forward * tocDoHienTai;
        rb.linearVelocity = new Vector3(vanTocMoi.x, rb.linearVelocity.y, vanTocMoi.z);

        // 3. NGHIÊNG XE (Visual Only)
        XuLyNghieng(inputA);
    }

    void XuLyNghieng(float inputA)
    {
        if (thanXe == null) return;

       

        float targetZ = 0;
        if (Mathf.Abs(tocDoHienTai) > 2f)
        {
            float chieu = (tocDoHienTai > 0) ? -1 : 1;
            targetZ = inputA * gocNghiengMax * chieu;
        }

        // FIX KHÓA TRỤC Y CHO VISUAL:
        // Chỉ thay đổi Z, còn X và Y luôn giữ bằng 0 so với xe mẹ
        // Điều này giúp cái vỏ xe luôn dính chặt vào xe mẹ, không bị xoay lệch
        float currentZ = thanXe.localEulerAngles.z;
        float newZ = Mathf.LerpAngle(currentZ, targetZ, Time.fixedDeltaTime * tocDoNghieng);

        thanXe.localEulerAngles = new Vector3(0, 0, newZ);
    }

    // --- LOGIC GTA ---
    void LenXe()
    {
        dangLaiXe = true;
        nhanVatDiBo.SetActive(false); nhanVatLaiXe.SetActive(true);
        camDiBo.SetActive(false); camLaiXe.SetActive(true);

        // Bật vật lý bình thường để chạy
        rb.isKinematic = false;
        rb.linearDamping = 0.05f; // Nhả phanh tay (Về số nhỏ để xe lướt đi)
    }

    void XuongXe()
    {
        dangLaiXe = false;
        if (viTriXuongXe)
        {
            nhanVatDiBo.transform.position = viTriXuongXe.position;
            nhanVatDiBo.transform.rotation = viTriXuongXe.rotation;
        }
        nhanVatDiBo.SetActive(true); nhanVatLaiXe.SetActive(false);
        camLaiXe.SetActive(false); camDiBo.SetActive(true);

        tocDoHienTai = 0;

        rb.isKinematic = false;    // Vẫn cho trọng lực hoạt động
        rb.linearDamping = 1000f;  // Kéo phanh tay cực mạnh (Xe đứng im không trôi)
    }
}