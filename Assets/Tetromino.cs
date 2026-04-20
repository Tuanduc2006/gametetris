using UnityEngine;

public class Tetromino : MonoBehaviour
{
    // Tốc độ trượt tự động (đơn vị/giây)
    public float fallSpeed = 2f;

    void Update()
    {
        // 1. Phím di chuyển ngang và xoay vẫn giữ nguyên độ giật (snap) cho chuẩn xác
        if (Input.GetKeyDown(KeyCode.A)) { MoveLeft(); }
        else if (Input.GetKeyDown(KeyCode.S)) { MoveRight(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { Rotate(); }

        // 2. RƠI MƯỢT: Khối gạch sẽ trượt xuống liên tục theo mỗi khung hình
        float currentSpeed = Input.GetKey(KeyCode.X) ? fallSpeed * 10f : fallSpeed;
        transform.position += new Vector3(0, -currentSpeed * Time.deltaTime, 0);

        // 3. XỬ LÝ VA CHẠM: Nếu trong quá trình trượt mà lấn vào vùng cấm
        if (!IsValidPosition())
        {
            // Lập tức kéo ngược nó lên, làm tròn tọa độ để nó nằm ngay ngắn trên mặt lưới
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Ceil(transform.position.y), // Giật nhẹ lên mép trên của gạch dưới
                0
            );

            // CHỐNG LÚN TUYỆT ĐỐI: Nếu giật lên mà vẫn còn kẹt, đẩy thẳng lên từng ô cho đến khi an toàn
            while (!IsValidPosition())
            {
                transform.position += new Vector3(0, 1, 0);
            }

            LockAndSpawn(); // Chốt gạch
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

    // Đã đổi thành PUBLIC để GameManager có thể gọi kiểm tra Thua cuộc
    public bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            int roundX = Mathf.RoundToInt(child.position.x);

            // Dùng FloorToInt (lấy phần sàn) cho trục Y để bắt va chạm cực nhạy
            int floorY = Mathf.FloorToInt(child.position.y);

            // Kiểm tra lọt ra ngoài tường và đáy
            if (roundX < 0 || roundX >= GridMap.width || floorY < 0)
            {
                return false;
            }

            // Kiểm tra đè lên gạch cũ
            if (floorY < GridMap.height && GridMap.grid[roundX, floorY] != null)
            {
                return false;
            }
        }
        return true;
    }

    void LockAndSpawn()
    {
        // 1. Chép dữ liệu gạch vào mảng
        foreach (Transform child in transform)
        {
            int roundX = Mathf.RoundToInt(child.position.x);
            int roundY = Mathf.RoundToInt(child.position.y);

            // BẢO HIỂM LỖI ĐỎ: Chỉ lưu vào mảng nếu không bị tràn nóc
            if (roundX >= 0 && roundX < GridMap.width && roundY >= 0 && roundY < GridMap.height)
            {
                GridMap.grid[roundX, roundY] = child;
            }
        }

        // 2. Kích hoạt thuật toán xóa hàng
        GridMap.ClearFullRows();

        // 3. Tắt quyền điều khiển khối gạch này
        this.enabled = false;

        // 4. Gọi sinh khối mới bằng hàm chuẩn của Unity 6
        FindFirstObjectByType<GameManager>().SpawnNext();
    }
}