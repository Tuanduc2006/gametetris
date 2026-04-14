using UnityEngine;

public class GridMap : MonoBehaviour
{
    public static int width = 10;
    public static int height = 20;
    public static Transform[,] grid = new Transform[width, height];

    // --- HÀM MỚI: QUÉT VÀ XÓA HÀNG ---
    public static void ClearFullRows()
    {
        for (int y = 0; y < height; y++) // Quét từ đáy (y=0) lên đỉnh (y=19)
        {
            if (IsRowFull(y)) // Nếu thấy 1 hàng ngang đã đầy
            {
                DeleteRow(y);       // 1. Phá hủy hàng đó
                MoveRowsDown(y + 1); // 2. Kéo toàn bộ các hàng phía trên tụt xuống 1 ô
                y--;                 // 3. Lùi bước quét lại để kiểm tra cái hàng vừa bị rớt xuống
            }
        }
    }

    static bool IsRowFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null) return false; // Chỉ cần 1 ô trống là hàng chưa đầy
        }
        return true; // Nếu duyệt hết 10 cột mà không có ô trống -> Đầy!
    }

    static void DeleteRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject); // Xóa cục gạch khỏi màn hình
            grid[x, y] = null;              // Xóa dữ liệu trong mảng lưới
        }
    }

    static void MoveRowsDown(int startY)
    {
        for (int y = startY; y < height; y++) // Từ hàng nứt đổ lên...
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y]; // Chép dữ liệu xuống hàng dưới
                    grid[x, y] = null;           // Xóa dữ liệu hàng cũ
                    grid[x, y - 1].position += new Vector3(0, -1, 0); // Kéo hình ảnh gạch rớt xuống màn hình
                }
            }
        }
    }
}