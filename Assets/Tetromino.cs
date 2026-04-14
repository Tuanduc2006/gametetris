using UnityEngine;

public class Tetromino : MonoBehaviour
{
    // Tốc độ trượt tự động (đơn vị/giây)
    public float fallSpeed = 2f;

    void Update()
    {
        // 1. Điều khiển ngang và xoay
        if (Input.GetKeyDown(KeyCode.A)) { MoveLeft(); }
        else if (Input.GetKeyDown(KeyCode.S)) { MoveRight(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { Rotate(); }

        // 2. RƠI MƯỢT: Trượt xuống liên tục
        float currentSpeed = Input.GetKey(KeyCode.X) ? fallSpeed * 10f : fallSpeed;
        transform.position += new Vector3(0, -currentSpeed * Time.deltaTime, 0);

        // 3. Xử lý va chạm và tiếp đất
        if (!IsValidPosition())
        {
            // Ép tọa độ về số nguyên để nằm ngay ngắn trên lưới
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Ceil(transform.position.y),
                0
            );

            // Bảo hiểm nhích lên nếu vẫn bị cấn
            if (!IsValidPosition())
            {
                transform.position += new Vector3(0, 1, 0);
            }

            LockAndSpawn();
        }
    }

    public void MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);
        if (!IsValidPosition()) { transform.position += new Vector3(1, 0, 0); }
    }

    public void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);
        if (!IsValidPosition()) { transform.position += new Vector3(-1, 0, 0); }
    }

    public void Rotate()
    {
        transform.Rotate(0, 0, -90);
        if (!IsValidPosition()) { transform.Rotate(0, 0, 90); }
    }

    // Đã đổi thành PUBLIC để GameManager có thể gọi được
    public bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            int roundX = Mathf.RoundToInt(child.position.x);
            int floorY = Mathf.FloorToInt(child.position.y);

            if (roundX < 0 || roundX >= GridMap.width || floorY < 0) return false;

            // Kiểm tra đè lên gạch cũ trong phạm vi chiều cao bàn cờ
            if (floorY < GridMap.height && GridMap.grid[roundX, floorY] != null) return false;
        }
        return true;
    }

    void LockAndSpawn()
    {
        foreach (Transform child in transform)
        {
            int roundX = Mathf.RoundToInt(child.position.x);
            int roundY = Mathf.RoundToInt(child.position.y);

            // BẢO HIỂM: Tránh lỗi IndexOutOfRangeException khi gạch quá cao
            if (roundX >= 0 && roundX < GridMap.width && roundY >= 0 && roundY < GridMap.height)
            {
                GridMap.grid[roundX, roundY] = child;
            }
        }

        GridMap.ClearFullRows();
        this.enabled = false;

        // Gọi khối mới
        FindFirstObjectByType<GameManager>().SpawnNext();
    }
}