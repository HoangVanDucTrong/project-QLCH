document.addEventListener("DOMContentLoaded", function () {
    const notificationBell = document.querySelector(".fa-bell"); // Biểu tượng chuông
    if (!notificationBell) {
        console.error("❌ Không tìm thấy biểu tượng thông báo `.fa-bell`");
        return;
    }

    const notificationList = document.createElement("div"); // Danh sách thông báo
    notificationList.classList.add("notification-list");
    document.body.appendChild(notificationList);

    let notificationCount = 0; // Đếm số thông báo chưa đọc

    // Kết nối SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7126/tableHub", {
            skipNegotiation: true, // 🛑 Bỏ qua bước mặc định, chỉ dùng WebSockets
            transport: signalR.HttpTransportType.WebSockets
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();
    // Bắt đầu kết nối SignalR
    connection.start()
        .then(() => console.log("✅ Kết nối SignalR thông báo ok thành công!"))
        .catch(err => console.error("❌ Lỗi kết nối SignalR:", err.toString()));
    connection.on("ReceiveOrderNotification", (data) => {
        console.log("📢 Nhận thông báo mới:", data); // Kiểm tra log trong console

        // Lấy thông báo và banId từ dữ liệu nhận được
        const message = data.message;
        const banId = data.banId;

        // Cập nhật số lượng thông báo
        notificationCount++;
        notificationBell.classList.add("has-notification");
        notificationBell.setAttribute("data-count", notificationCount);

        // Thêm thông báo vào danh sách
        let notificationItem = document.createElement("div");
        notificationItem.classList.add("notification-item");
        notificationItem.innerHTML = `
    <p>${message}</p>
    <button class="view-order" onclick="viewOrder(${banId})">Xem chi tiết</button>
`;


        // Thêm vào danh sách thông báo
        notificationList.appendChild(notificationItem);

        // Hiển thị danh sách nếu trước đó chưa có thông báo
        if (!notificationList.classList.contains("show")) {
            notificationList.classList.add("show");
        }
    });


    // Hiển thị danh sách thông báo khi bấm chuông
    notificationBell.addEventListener("click", function () {
        notificationList.classList.toggle("show");
        notificationCount = 0;
        notificationBell.classList.remove("has-notification"); // Xóa hiệu ứng chuông
        notificationBell.removeAttribute("data-count"); // Ẩn số đếm thông báo
    });


});
function viewOrder(banId) {
    console.log("🛠️ banId được nhận:", banId);
    if (!banId) {
        console.error("❌ banId bị undefined, không thể gọi API!");
        return;
    }
    fetch(`/Ban/GetOrderDetails?banId=${banId}`)
        .then(response => response.json())
        .then(data => {
         

            // Kiểm tra xem dữ liệu trả về có `orderDetails` không
            let orderDetails = data.orderDetails || data;
            let products = orderDetails.Products || orderDetails.products;

            if (!products || !Array.isArray(products)) {
                console.error("❌ Lỗi: Không tìm thấy danh sách sản phẩm (Products)!", data);
                return;
            }

            // 📝 Cập nhật thông tin cơ bản
            document.getElementById('orderBanId').innerText = orderDetails.BanId;
            document.getElementById('orderDate').innerText = new Date(orderDetails.NgayTao).toLocaleString();

            // 🖼 Kiểm tra & Hiển thị hóa đơn (ảnh check bank)
            if (orderDetails.ImageCheckBank) {
                document.getElementById('orderBill').style.display = 'block';
                document.getElementById('orderImage').src = 'data:image/jpeg;base64,' + orderDetails.ImageCheckBank;
            } else {
                document.getElementById('orderBill').style.display = 'none';
            }


            // Hiển thị danh sách món ăn
            const orderItemsContainer = document.getElementById('orderItems');
            orderItemsContainer.innerHTML = '';  // Xóa dữ liệu cũ trước khi thêm mới

            products.forEach(item => {
                const itemElement = document.createElement('div');
                itemElement.classList.add('order-item');
                itemElement.innerHTML = `
                <img src="data:image/jpeg;base64,${item.ImageBase64 || item.imageBase64}" alt="Ảnh món">
                <div class="item-info">
                    <h4>${item.Ten || item.ten}</h4>
                    <p>Số lượng: ${item.SoLuong || item.soLuong}</p>
                    <p>Tổng tiền: ${item.TongTien || item.tongTien} VND</p>
                </div>
            `;
                orderItemsContainer.appendChild(itemElement);
            });

            // Hiển thị popup
            document.getElementById('orderPopup').style.display = 'block';
        })
        .catch(err => console.error("❌ Lỗi khi gọi API:", err));

}

function closeOrderPopup() {
    document.getElementById('orderPopup').style.display = 'none';

    // Xóa nội dung đơn hàng
    document.getElementById('orderBanId').innerText = '';
    document.getElementById('orderDate').innerText = '';
    document.getElementById('orderItems').innerHTML = '';
    document.getElementById('orderBill').style.display = 'none';
    document.getElementById('orderImage').src = '';

    // 🟢 Ẩn danh sách thông báo khi đóng popup
    const notificationList = document.getElementById('notificationList');
    if (notificationList) {
        notificationList.classList.remove("show");
    }

    // 🟢 Xóa tất cả thông báo khỏi chuông
    document.getElementById('notificationList').innerHTML = '';

}
