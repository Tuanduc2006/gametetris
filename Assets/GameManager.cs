using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject gameOverPanel; // Kéo bảng chữ THUA vào đây ở Inspector

    void Start()
    {
        Time.timeScale = 1; // Đảm bảo game chạy bình thường khi khởi động
        SpawnNext();
    }

    public void SpawnNext()
    {
        int randomIndex = Random.Range(0, prefabs.Length);

        // Sinh gạch ở vị trí xuất phát
        GameObject nextTetromino = Instantiate(prefabs[randomIndex], new Vector3(4, 18, 0), Quaternion.identity);

        // KIỂM TRA THUA CUỘC NGAY KHI VỪA SINH RA
        if (!nextTetromino.GetComponent<Tetromino>().IsValidPosition())
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Hiện bảng chữ THUA
        }
        Time.timeScale = 0; // Dừng toàn bộ game
    }
}