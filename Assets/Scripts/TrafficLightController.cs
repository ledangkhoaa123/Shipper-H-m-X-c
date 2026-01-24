using UnityEngine;
using UnityEngine.SceneManagement;

public class TrafficLightController : MonoBehaviour
{
    [Header("⏱️ Thời gian đèn (giây)")]
    public float redTime = 5f;
    public float yellowTime = 2f;
    public float greenTime = 5f;

    [Header("💡 3 Bóng đèn (kéo object vào đây)")]
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    [Header("⚠️ Vùng nguy hiểm")]
    public BoxCollider dangerZone;

    private LightState currentState = LightState.Red;
    private float timer = 0f;

    enum LightState { Red, Yellow, Green }

    void Start()
    {
        if (redLight == null || yellowLight == null || greenLight == null)
        {
            Debug.LogError("❌ CHƯA GẮN 3 BÓNG ĐÈN!");
            return;
        }

        if (dangerZone != null)
        {
            dangerZone.isTrigger = true;
        }

        UpdateLights();
        Debug.Log("🔴 ĐÈN ĐỎ - GAME BẮT ĐẦU!");
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case LightState.Red:
                if (timer >= redTime)
                {
                    currentState = LightState.Green;
                    timer = 0f;
                    UpdateLights();
                    Debug.Log("🟢 ĐÈN XANH!");
                }
                break;

            case LightState.Green:
                if (timer >= greenTime)
                {
                    currentState = LightState.Yellow;
                    timer = 0f;
                    UpdateLights();
                    Debug.Log("🟡 ĐÈN VÀNG!");
                }
                break;

            case LightState.Yellow:
                if (timer >= yellowTime)
                {
                    currentState = LightState.Red;
                    timer = 0f;
                    UpdateLights();
                    Debug.Log("🔴 ĐÈN ĐỎ!");
                }
                break;
        }
    }

    void UpdateLights()
    {
        redLight.SetActive(false);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);

        switch (currentState)
        {
            case LightState.Red:
                redLight.SetActive(true);
                break;
            case LightState.Yellow:
                yellowLight.SetActive(true);
                break;
            case LightState.Green:
                greenLight.SetActive(true);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentState == LightState.Red)
        {
            Debug.LogError("GAME OVER - VUOT DEN DO!");
            Time.timeScale = 0f;
        }
    }

}
