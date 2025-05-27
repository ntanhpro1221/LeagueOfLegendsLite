from PIL import Image
import os
import math

# Đường dẫn tới thư mục gốc chứa ảnh và các folder con
root_dir = "./"  # ← Thay bằng đường dẫn thật

# Thu thập toàn bộ ảnh PNG trong thư mục và folder con
image_paths = []
for root, _, files in os.walk(root_dir):
    for file in files:
        if file.lower().endswith(".png"):
            full_path = os.path.join(root, file)
            try:
                with Image.open(full_path) as img:
                    if img.size == (32, 32):
                        image_paths.append(full_path)
            except Exception as e:
                print(f"Lỗi khi đọc ảnh {full_path}: {e}")

# Kiểm tra nếu không có ảnh nào phù hợp
if not image_paths:
    print("Không có ảnh PNG nào kích thước 32x32 được tìm thấy.")
    exit()

# Sắp xếp tên ảnh cho ổn định
image_paths.sort()

# Load tất cả ảnh hợp lệ
images = [Image.open(path) for path in image_paths]

# Cấu hình lưới
tile_width, tile_height = 32, 32
num_images = len(images)
columns = 10  # Bạn có thể chỉnh lại nếu muốn
rows = math.ceil(num_images / columns)

# Tạo ảnh kết quả
sheet_width = columns * tile_width
sheet_height = rows * tile_height
sprite_sheet = Image.new("RGBA", (sheet_width, sheet_height))

# Ghép ảnh vào sprite sheet
for idx, img in enumerate(images):
    x = (idx % columns) * tile_width
    y = (idx // columns) * tile_height
    sprite_sheet.paste(img, (x, y))

# Lưu ảnh
sprite_sheet.save("sprite_sheet.png")
print(f"Đã ghép {num_images} ảnh 32x32 thành sprite_sheet.png ({columns} cột, {rows} hàng)")
