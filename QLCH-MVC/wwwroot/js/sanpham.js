document.addEventListener("DOMContentLoaded", function () {
    initAddProduct();
    initDeleteProduct();
    initEditProduct();
});

// ======================== THÊM SẢN PHẨM ========================
function initAddProduct() {
    const addButton = document.querySelector('.add-button');
    const addProductForm = document.getElementById('addProductForm');
    const overlay = document.querySelector('.overlay');

    if (!addButton || !addProductForm) return;

    addButton.addEventListener("click", function () {
        console.log("✅ Nút thêm sản phẩm đã được bấm!");
        showAddForm();
    });

    function showAddForm() {
        addProductForm.style.display = "block";
        overlay.style.display = "block";
    }

    function closeAddForm() {
        addProductForm.style.display = "none";
        overlay.style.display = "none";
        addProductForm.reset();
        document.getElementById("previewImg").src = '';
    }
    window.closeAddForm = closeAddForm;

    document.getElementById("productImage").addEventListener("change", function (event) {
        const previewImg = document.getElementById("previewImg");
        if (!previewImg) {
            console.error("❌ Không tìm thấy phần tử previewImg!");
            return;
        }

        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                previewImg.src = e.target.result;
                previewImg.style.display = "block";
            };
            reader.readAsDataURL(file);
        } else {
            previewImg.src = "";
        }
    });

    document.getElementById("addProductForm").addEventListener("submit", async function (e) {
        e.preventDefault();
        let formData = new FormData();
        formData.append("Ten", document.getElementById("productName").value);
        formData.append("Gia", document.getElementById("productPrice").value);
        formData.append("MoTa", document.getElementById("productDescription").value);
        formData.append("DanhMucId", document.getElementById("productCategory").value);

        let imageFile = document.getElementById("productImage").files[0];
        if (imageFile) {
            formData.append("ImageURL", imageFile);
        }

        // Kiểm tra log dữ liệu trước khi gửi request
        for (let [key, value] of formData.entries()) {
            console.log(key, value);
        }

        let response = await fetch("/QuanLySanPham/PostSanPham", {
            method: "POST",
            body: formData
        });

        let result = await response.json();
        alert(result.message);
        if (result.success) location.reload();
    });

}

// ======================== XÓA SẢN PHẨM ========================
function initDeleteProduct() {
    const confirmDialog = document.querySelector(".confirm-dialog");
    const overlay = document.querySelector(".overlay");
    const cancelButton = document.querySelector(".cancel-btn");

    if (!confirmDialog || !cancelButton) {
        console.error("⚠️ Không tìm thấy .confirm-dialog hoặc .cancel-btn trong DOM!");
        return;
    }

    window.deleteProduct = async function (productId) {
        console.log("🗑️ Chuẩn bị xóa sản phẩm có ID:", productId);

        confirmDialog.style.display = "block";
        overlay.style.display = "block";
        // Hàm đóng hộp thoại khi bấm nút Hủy

        // Thêm sự kiện cho nút "Xóa"
        document.querySelector(".confirm-btn").onclick = async function () {
            try {
                let response = await fetch(`/QuanLySanPham/DeleteSanPham/${productId}`, { method: 'DELETE' });
                if (response.ok) {
                    alert('✅ Xóa sản phẩm thành công!');
                    window.location.reload();
                } else {
                    alert('❌ Không thể xóa sản phẩm! Đã có lỗi xảy ra.');
                }
            } catch (error) {
                console.error("Lỗi khi xóa sản phẩm:", error);
            }
            closeDeleteDialog(); // Đảm bảo đóng hộp thoại sau khi xóa
        };
        function closeEditForm() {
            document.getElementById('editProductForm').style.display = 'none';
            document.querySelector('.overlay').style.display = 'none';
            document.querySelector('.confirm-dialog').style.display = 'none';
        }
        window.closeEditForm = closeEditForm;


    }
}


// ======================== SỬA SẢN PHẨM ========================
function initEditProduct() {

    window.editProduct = async function (productId) {
        console.log("✏️ Đang chỉnh sửa sản phẩm có ID:", productId);

        let response = await fetch(`/QuanLySanPham/GetSanPham/${productId}`, {
            method: 'GET',
        });

        console.log("Response status:", response.status);
        console.log("Response URL:", response.url);

        let data = await response.json();
        console.log("✅ Dữ liệu từ API:", data);

        if (!data.success) {
            throw new Error("Dữ liệu sản phẩm rỗng hoặc không tồn tại!");
        }

        document.getElementById("editProductForm").style.display = "block";
        document.querySelector(".overlay").style.display = "block";

        let product = data.data || data; // Lấy dữ liệu sản phẩm từ API
        console.log("✅ Dữ liệu sản phẩm sau khi parse:", product);

        document.getElementById("editProductName").value = product.Ten || 'Không có dữ liệu';
        document.getElementById("editProductPrice").value = product.Gia || '0';
        document.getElementById("editProductDescription").value = product.MoTa || 'Không có mô tả';
        document.getElementById("editProductType").value = product.DanhMucId || '0';
        document.getElementById("editIndex").value = product.SanPhamId || '0';
        // Khi tải dữ liệu sản phẩm
        if (product.ImageBase64) {
            let imgElement = document.getElementById("editPreviewImg");
            imgElement.src = `data:image/png;base64,${product.ImageBase64}`;

            // Lưu chuỗi ảnh cũ vào input ẩn
            document.getElementById("oldProductImage").value = product.ImageBase64;
        } else {
            document.getElementById("editPreviewImg").src = "/default-image.png";
        }


        // Cập nhật ảnh preview khi chọn file mới
        document.getElementById("editProductImage").addEventListener("change", function (e) {
            let file = e.target.files[0];
            if (file) {
                let reader = new FileReader();
                reader.onload = function (event) {
                    document.getElementById("editPreviewImg").src = event.target.result; // Hiển thị ảnh mới
                };
                reader.readAsDataURL(file);
            }
        });
    };

    document.getElementById("editProductForm").addEventListener("submit", async function (e) {
        e.preventDefault();
        let productId = document.getElementById("editIndex").value;
        let formData = new FormData();

        formData.append("Ten", document.getElementById("editProductName").value);
        formData.append("Gia", document.getElementById("editProductPrice").value);
        formData.append("MoTa", document.getElementById("editProductDescription").value);
        formData.append("DanhMucId", document.getElementById("editProductType").value);

        let imageFile = document.getElementById("editProductImage").files[0];

        if (imageFile) {
            // Nếu có ảnh mới, thêm ảnh vào FormData
            formData.append("ImageURL", imageFile);
        } else {
            // Nếu không có ảnh mới, lấy ảnh cũ từ input hidden
            let oldImageBase64 = document.getElementById("oldProductImage").value;

            if (oldImageBase64) {
                // Chuyển base64 thành Blob
                let byteCharacters = atob(oldImageBase64);
                let byteArrays = [];
                for (let offset = 0; offset < byteCharacters.length; offset += 1024) {
                    let slice = byteCharacters.slice(offset, offset + 1024);
                    let byteNumbers = new Array(slice.length);
                    for (let i = 0; i < slice.length; i++) {
                        byteNumbers[i] = slice.charCodeAt(i);
                    }
                    byteArrays.push(new Uint8Array(byteNumbers));
                }
                let blob = new Blob(byteArrays, { type: 'image/jpeg' }); // hoặc 'image/png' tùy vào loại hình ảnh của bạn
                let file = new File([blob], "image.jpg", { type: 'image/jpeg' });
                formData.append("ImageURL", file);
            } else {
                alert("❌ Lỗi: Hình ảnh không được để trống!");
                return;
            }
        }

        try {
            let response = await fetch(`/QuanLySanPham/PutSanPham/${productId}`, {
                method: "PUT",
                body: formData
            });
            let result = await response.json();
            alert(result.message);
            if (result.success) {
                location.reload();
            }
        } catch (error) {
            console.error("Lỗi khi cập nhật sản phẩm:", error);
        }
    });


    function closeEditForm() {
        document.getElementById("editProductForm").style.display = "none";
        document.querySelector(".overlay").style.display = "none";
        document.getElementById("editProductForm").reset();
        document.getElementById("editPreviewImg").src = '';
    }
    window.closeEditForm = closeEditForm;
}
