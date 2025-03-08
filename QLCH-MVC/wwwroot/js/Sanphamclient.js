
document.addEventListener("DOMContentLoaded", function () {
    const cartItemsElement = document.getElementById("cart-items");
    const cartTotalElement = document.getElementById("cart-total");
    const thanhtoanButton = document.getElementById("thanhtoan");


    const storeId = document.getElementById("storeData").dataset.storeId;
    const banId = document.getElementById("BanidData").dataset.banid;


    // Giỏ hàng lưu trữ sản phẩm
    let cart = [];

    // Thêm sản phẩm vào giỏ hàng khi click vào icon
    document.querySelectorAll(".cart-shopping").forEach(icon => {
        icon.addEventListener("click", function () {
            const productElement = this.closest(".product-item");
            const productId = productElement.dataset.productId;
            const productName = productElement.dataset.productName;
            const productPrice = parseFloat(productElement.dataset.productPrice);

            // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
            const existingProduct = cart.find(item => item.id === productId);

            if (existingProduct) {
                existingProduct.quantity += 1;
            } else {
                cart.push({
                    id: productId,
                    name: productName,
                    price: productPrice,
                    quantity: 1
                });
            }

            updateCartUI();
        });
    });
    // Cập nhật giao diện giỏ hàng
    function updateCartUI() {
        cartItemsElement.innerHTML = "";
        let total = 0;

        cart.forEach((item, index) => {
            const li = document.createElement("li");

            // Hiển thị sản phẩm và nút xóa (✖)
            li.innerHTML = `
                ${item.name} x ${item.quantity} - ${(item.price * item.quantity).toLocaleString()} VNĐ
                <span class="remove-item" data-index="${index}" style="color: red; cursor: pointer; margin-left: 10px;">✖</span>
            `;

            cartItemsElement.appendChild(li);
            total += item.price * item.quantity;
        });

        cartTotalElement.textContent = total.toLocaleString();

        // Thêm sự kiện click cho nút xóa sản phẩm
        const removeButtons = document.querySelectorAll(".remove-item");
        removeButtons.forEach(button => {
            button.addEventListener("click", function () {
                const index = parseInt(this.getAttribute("data-index"));
                cart.splice(index, 1); // Xóa sản phẩm khỏi mảng cart
                updateCartUI(); // Cập nhật lại giao diện
            });
        });
    }


    // Xử lý sự kiện "Thanh toán"
    thanhtoanButton.addEventListener("click", function () {
        if (cart.length === 0) {
            alert("Giỏ hàng trống! Vui lòng thêm sản phẩm trước khi thanh toán.");
            return;
        }



        function updateTableStatusAPI(banId, storeId) {
            if (!banId || !storeId || isNaN(banId) || isNaN(storeId)) {
                console.error("❌ Lỗi: `banId` hoặc `storeId` không hợp lệ!", banId, storeId);
                alert("Không thể cập nhật trạng thái bàn vì thiếu thông tin.");
                return;
            }
            fetch("/Ban/UpdateTableStatus", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    BanId: parseInt(banId),  // Chắc chắn gửi số nguyên
                    StoreId: parseInt(storeId)
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        console.log("✅ Trạng thái bàn đã được cập nhật.");
                    } else {
                        console.error("⚠️ Lỗi khi cập nhật trạng thái bàn:", data.message);
                    }
                })
                .catch(error => {
                    console.error("❌ Lỗi khi gọi API:", error);
                });
        }
        // Lấy tổng tiền từ giỏ hàng
        const totalAmount = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);

        // Tạo object chứa dữ liệu thanh toán bao gồm storeId
        const transactionData = {
            Amount: totalAmount,
            Note: "Thanh toán đơn hàng",
            BanId: parseInt(banId),
            StoreId: parseInt(storeId),
            QRCodeUrl: ''
        };

        // Gửi dữ liệu đến action Create
        fetch("/Ban/Create", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(transactionData)
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    var transactionId = data.transactionId;
                    console.log("Giao dịch được tạo với banid ID:", banId);
                    console.log("Giao dịch được tạo với ID:", transactionId);
                    console.log("Giao dịch được tạo với cuahang ID:", storeId);
                    fetch(`/Ban/GenerateQrView?transactionId=${transactionId}&storeId=${storeId}`, {
                        method: "GET"
                    })
                        .then(response => response.text())
                        .then(html => {

                            if (html.includes("Đã có lỗi xảy ra") || html.toLowerCase().includes("error")) {
                                throw new Error("Nội dung trả về có lỗi, không hiển thị QR.");
                            }

                            let qrContainer = document.getElementById("qrViewContainer");

                            // Xóa nội dung cũ trước khi thêm mới
                            qrContainer.innerHTML = "";
                            let qrWrapper = document.createElement("div");
                            qrWrapper.classList.add("qr-container");
                            savedQRCode = html;
                            // Thêm QR vào `qrWrapper`
                            let qrCodeDiv = document.createElement("div");
                            qrCodeDiv.innerHTML = html;
                            qrWrapper.appendChild(qrCodeDiv);

                            const downloadButton = document.createElement("button");
                            downloadButton.innerHTML = "⬇️ Tải mã QR";
                            downloadButton.classList.add("qr-download-btn");
                            downloadButton.onclick = function () {
                                const qrImg = qrCodeDiv.querySelector("img");
                                if (qrImg) {
                                    const link = document.createElement("a");
                                    link.href = qrImg.src;
                                    link.download = "QRCode.png";
                                    link.click();
                                    updateTableStatusAPI(banId, storeId);
                                } else {
                                    alert("Không tìm thấy hình ảnh mã QR để tải xuống.");
                                }
                            };
                            qrWrapper.appendChild(downloadButton);


                            // Thêm nút đóng (✖)
                            let closeButton = document.createElement("button");
                            closeButton.innerHTML = "✖";
                            closeButton.classList.add("qr-close-btn");
                            closeButton.onclick = function () {
                                qrContainer.style.display = "none";
                                qrContainer.innerHTML = ""; // Xóa nội dung QR khi đóng
                                cart = [];

                                updateCartUI();
                                updateTableStatusAPI(banId, storeId);
                            };
                            qrContainer.appendChild(closeButton);
                            // Hiển thị QR
                            qrContainer.style.display = "flex";
                            qrContainer.style.justifyContent = "center"; // Căn giữa toàn bộ QR
                            qrContainer.style.alignItems = "center";
                            qrContainer.appendChild(qrWrapper);



                        })
                        .catch(error => {
                            console.error("Error loading QR view:", error);
                            alert("Bạn Chưa thêm tài khoản ngân hàng!");
                        });

                } else {
                    alert("Thanh toán thất bại: " + data.message);
                }
            })
            .catch(error => {
                console.error("Lỗi khi gọi API:", error);
                alert("Đã xảy ra lỗi khi thanh toán!");
            });
    });
});