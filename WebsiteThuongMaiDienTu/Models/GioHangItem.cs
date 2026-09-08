namespace WebsiteThuongMaiDienTu.Models
{
    // Model tạm dùng để lưu 1 dòng sản phẩm trong giỏ hàng (Session["GioHang"]).
    // Đây KHÔNG phải Entity Framework model, không map vào bảng CSDL nào.
    // Toàn bộ nghiệp vụ thêm/sửa/xóa giỏ hàng sẽ được cài đặt đầy đủ ở module Giỏ hàng (không nằm trong phạm vi này).
    // Ở đây, class này chỉ tồn tại để _Layout.cshtml tính được số lượng hiển thị trên badge giỏ hàng.
    public class GioHangItem
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string Hinh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien
        {
            get { return DonGia * SoLuong; }
        }
    }
}