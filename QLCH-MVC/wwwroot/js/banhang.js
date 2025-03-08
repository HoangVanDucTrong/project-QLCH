
function dongModal() {
    document.getElementById("qrModal").style.display = "none";
}
// Lấy các phần tử
const menuButton = document.getElementById('menuButton');
const closeButton = document.getElementById('closeButton');
const menu = document.getElementById('menu');

// Khi nhấn nút "Thực đơn"
menuButton.addEventListener('click', () => {
    menu.classList.add('active'); // Thêm class active để hiển thị menu
});

// Khi nhấn nút "✖" để đóng menu
closeButton.addEventListener('click', () => {
    menu.classList.remove('active'); // Xóa class active để ẩn menu
});


document.addEventListener("DOMContentLoaded", function () {
    const menuList = document.querySelector(".menu ul"); // Lấy danh sách <ul>

    // Lắng nghe sự kiện click cho tăng/giảm số lượng
    menuList.addEventListener("click", function (event) {
        // Xử lý nút tăng số lượng
        if (event.target && event.target.classList.contains("increase-button")) {
            const input = event.target.previousElementSibling;
            input.value = parseInt(input.value) + 1;
        }

        // Xử lý nút giảm số lượng
        if (event.target && event.target.classList.contains("decrease-button")) {
            const input = event.target.nextElementSibling;
            if (parseInt(input.value) > 0) {
                input.value = parseInt(input.value) - 1;
            }
        }
    });
});
/*
document.addEventListener("DOMContentLoaded", function () {
    const menuList = document.querySelector(".menu ul"); // Lấy danh sách <ul>

    // Lắng nghe sự kiện click trên phần tử cha <ul>
    menuList.addEventListener("click", function (event) {
        if (event.target && event.target.id === "addproduct") {
            // Lấy thông tin sản phẩm từ phần tử cha
            const parentLi = event.target.closest("li");
            const itemName = parentLi.querySelector(".item-name").textContent;
            const itemPrice = parentLi.querySelector(".item-price").textContent;
            const quantityInput = parentLi.querySelector(".quantity-input").value;

            // Tính thành tiền
            const totalPrice = parseInt(quantityInput) * parseInt(itemPrice.replace(/,/g, "").replace("đ", ""));

            // Tìm bảng và thêm hàng mới
            const tableBody = document.querySelector(".table-wrapper table tbody");

            // Kiểm tra sản phẩm đã tồn tại trong bảng chưa
            let existingRow = Array.from(tableBody.querySelectorAll("tr")).find(row => {
                return row.querySelector("td:nth-child(2)").textContent === itemName;
            });

            if (existingRow) {
                // Nếu đã tồn tại, cập nhật số lượng và thành tiền
                const existingQuantityCell = existingRow.querySelector("td:nth-child(3)");
                const existingTotalPriceCell = existingRow.querySelector("td:nth-child(4)");

                const updatedQuantity = parseInt(existingQuantityCell.textContent) + parseInt(quantityInput);
                const updatedTotalPrice = updatedQuantity * parseInt(itemPrice.replace(/,/g, "").replace("đ", ""));

                existingQuantityCell.textContent = updatedQuantity;
                existingTotalPriceCell.textContent = updatedTotalPrice.toLocaleString() + "đ";
            } else {
                // Nếu chưa tồn tại, thêm hàng mới
                const newRow = document.createElement("tr");

                newRow.innerHTML = `
                        <td class="delete-btn">x</td>
                        <td>${itemName}</td>
                        <td>${quantityInput}</td>
                        <td>${totalPrice.toLocaleString()}đ</td>
                    `;

                // Thêm sự kiện xóa cho nút "x"
                newRow.querySelector(".delete-btn").addEventListener("click", function () {
                    this.closest("tr").remove();
                    updateTotalAmount();
                });

                tableBody.appendChild(newRow);
            }

            // Cập nhật tổng cộng
            updateTotalAmount();
        }
    });

    function formatCurrency(value) {
        return value.toLocaleString() + "đ"; // Thêm dấu phân cách và "đ"
    }

    // Hàm cập nhật tổng cộng
    function updateTotalAmount() {
        const tableBody = document.querySelector(".table-wrapper table tbody");
        const totalAmountCell = document.querySelector(".table-wrapper table tfoot td:last-child");
        const btnthanhtoan = document.querySelector(".btnthanhtoan");
        let totalAmount = 0;

        tableBody.querySelectorAll("tr").forEach(row => {
            const rowTotalPriceText = row.querySelector("td:nth-child(4)").textContent;
            if (rowTotalPriceText) {
                const rowTotalPrice = parseInt(rowTotalPriceText.replace(/\./g, "").replace("đ", ""));
                totalAmount += rowTotalPrice;
            }
        });
        totalAmountCell.textContent = formatCurrency(totalAmount);
        btnthanhtoan.textContent = "Thanh toán: " + formatCurrency(totalAmount);
    }
});

*/
document.addEventListener("DOMContentLoaded", function () {
    const bills = JSON.parse(localStorage.getItem("bills")) || {};  // Lưu trữ bill cho từng bàn
    let currentTable = null; // Bàn hiện tại đang được chọn

    const tableElements = document.querySelectorAll(".tables-section .table");
    const currentTableDisplay = document.getElementById("currentTableDisplay");
    const tableBody = document.querySelector(".table-wrapper tbody");
    const totalAmountCell = document.querySelector(".table-wrapper tfoot td:last-child");
    const btnThanhToan = document.querySelector(".btnthanhtoan");
    //lưu trữ diệu vào local
    function saveBillsToLocalStorage() {
        localStorage.setItem("bills", JSON.stringify(bills));
    }

        function printInvoice() {
            if (!currentTable || !bills[currentTable] || bills[currentTable].items.length === 0) {
                alert("Không có hóa đơn để in!");
                return;
            }

            let bill = bills[currentTable]; // Lấy hóa đơn hiện tại
            let billContent = document.getElementById("billContent");
            let totalBillAmount = document.getElementById("totalBillAmount");
            let qrContainer = document.getElementById("qrCodeContainer");
            let printableBill = document.getElementById("printableBill");
            printableBill.style.display = "block";
          
            // Xóa nội dung cũ của hóa đơn trước khi in
            billContent.innerHTML = "";

            // **Duyệt qua danh sách món ăn và thêm vào bảng in**
            bill.items.forEach(item => {
                let row = document.createElement("tr");
                row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.quantity}</td>
                <td>${item.totalPrice.toLocaleString()}đ</td>
            `;
                billContent.appendChild(row);
            });

            // Cập nhật tổng tiền
            totalBillAmount.textContent = bill.total.toLocaleString() + "đ";

            // 🚀 Nếu QR đã được tạo, hiển thị QR trong hóa đơn in
            if (savedQRCode.trim() !== "") {
                qrContainer.innerHTML = savedQRCode;
            } else {
                qrContainer.innerHTML = "<p>Không có QR Code!</p>";
            }

            // 🚀 In ngay lập tức
            setTimeout(() => {
                window.print();

                // 🚀 **Xóa hóa đơn sau khi in xong**
                setTimeout(() => {
                    delete bills[currentTable];
                    saveBillsToLocalStorage();
                    displayBill();
                    updateTableStatus();
                }, 1000); // Chờ 1 giây để đảm bảo in hoàn tất trước khi xóa
            }, 500);
        }


    // Hiển thị bill
    function displayBill() {
        if (!currentTable) return;

        const bill = bills[currentTable] || { items: [], total: 0 };
        tableBody.innerHTML = ""; // Xóa danh sách cũ

        let billContent = document.getElementById("billContent");
        billContent.innerHTML = ""; // Xóa nội dung cũ

        // Hiển thị số bàn
        document.getElementById("tableNumber").textContent = currentTable;

        bill.items.forEach(item => {
            const row = document.createElement("tr");
            row.innerHTML = `
            <td>${item.name}</td>
            <td>${item.quantity}</td>
            <td>${item.totalPrice.toLocaleString()}đ</td>
        `;

            // Thêm vào bảng hiển thị
            billContent.appendChild(row);

            // Cập nhật bảng hiển thị gốc
            const mainRow = document.createElement("tr");
            mainRow.innerHTML = `
            <td class="delete-btn">x</td>
            <td>${item.name}</td>
            <td>${item.quantity}</td>
            <td>${item.totalPrice.toLocaleString()}đ</td>
        `;

            // Sự kiện xóa món
            mainRow.querySelector(".delete-btn").addEventListener("click", function () {
                const index = bill.items.indexOf(item);
                if (index > -1) {
                    bill.items.splice(index, 1);
                    updateTotalAmount();
                    saveBillsToLocalStorage();
                    displayBill(); // Hiển thị lại hóa đơn mới
                    updateTableStatuslocal(); // 🛑 Cập nhật trạng thái bàn sau khi xóa
                }
            });

            tableBody.appendChild(mainRow);
        });

        document.getElementById("totalBillAmount").textContent = bill.total.toLocaleString() + "đ";
        totalAmountCell.textContent = bill.total.toLocaleString() + "đ";
        btnThanhToan.textContent = `Thanh toán: ${bill.total.toLocaleString()}đ`;
    }


    // Cập nhật tổng tiền
    function updateTotalAmount() {
        if (!currentTable) return;

        const bill = bills[currentTable] || { items: [], total: 0 };
        bill.total = bill.items.reduce((sum, item) => sum + item.totalPrice, 0);
        bills[currentTable] = bill;
    }
    function updateTableStatus() {
        document.querySelectorAll(".table").forEach(table => {
            const isInUse = table.dataset.isInUse === "true"; // Lấy trạng thái từ backend
            if (isInUse) {
                table.classList.add("in-use-table");
                table.classList.remove("available");
            } else {
                table.classList.add("available");
                table.classList.remove("in-use-table");
            }
        });

        // Sau khi cập nhật trạng thái từ backend, cập nhật từ `bills` trong localStorage
        setTimeout(updateTableStatuslocal, 200); // Đảm bảo gọi sau khi `updateTableStatus()` chạy
    }

    function updateTableStatuslocal() {
        document.querySelectorAll(".table").forEach(table => {
            const tableId = table.dataset.soban;
            const isInUseFromServer = table.dataset.isInUse === "true"; // Lấy trạng thái từ backend

            // Nếu bàn có món trong `bills`, giữ trạng thái "in-use"
            if (bills[tableId] && bills[tableId].items.length > 0) {
                table.classList.add("in-use");
                table.classList.remove("available");
            } else {
                // 🛑 Chỉ đặt về "available" nếu backend KHÔNG đánh dấu bàn là đang sử dụng
                if (!isInUseFromServer) {
                    table.classList.add("available");
                    table.classList.remove("in-use");
                }
            }
        });
    }
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7126/tableHub", {
            skipNegotiation: true, // 🛑 Bỏ qua bước mặc định, chỉ dùng WebSockets
            transport: signalR.HttpTransportType.WebSockets
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // Bắt đầu kết nối
    connection.start()
        .then(() => console.log("✅ Kết nối SignalR thành công!"))
        .catch(err => console.error("❌ Lỗi kết nối SignalR:", err.toString()));

    // Nhận tín hiệu từ server khi trạng thái bàn thay đổi
    connection.on("ReceiveTableUpdate", (banId, isInUse) => {
        console.log(`🚀 Bàn ${banId} cập nhật trạng thái: ${isInUse}`);

        // Cập nhật UI bàn tương ứng
        const tableElement = document.querySelector(`.table[data-table-id="${banId}"]`);
        if (tableElement) {
            tableElement.dataset.isInUse = isInUse;
            updateTableStatus();
        }
    });

    // Thêm món vào bill
    document.querySelector(".menu ul").addEventListener("click", function (event) {
        if (event.target && event.target.id === "addproduct") {
            if (!currentTable) {
                alert("Vui lòng chọn bàn trước!");
                return;
            }

            const parentLi = event.target.closest("li");
            const itemName = parentLi.querySelector(".item-name").textContent;
            const itemPrice = parseInt(parentLi.querySelector(".item-price").textContent.replace(/,/g, "").replace("đ", ""));
            const quantity = parseInt(parentLi.querySelector(".quantity-input").value);
            const totalPrice = itemPrice * quantity;

            const bill = bills[currentTable] || { items: [], total: 0 };

            const existingItem = bill.items.find(item => item.name === itemName);
            if (existingItem) {
                existingItem.quantity += quantity;
                existingItem.totalPrice += totalPrice;
            } else {
                bill.items.push({ name: itemName, quantity: quantity, totalPrice: totalPrice });
            }

            bills[currentTable] = bill;

            updateTotalAmount(); // Cập nhật tổng cộng
            saveBillsToLocalStorage();
            displayBill(); // Hiển thị lại bill
        
            updateTableStatuslocal();
        }
    });

    // Gắn sự kiện chọn bàn
    tableElements.forEach(table => {
        table.addEventListener("click", function () {
            currentTable = this.dataset.soban; // Lấy ID bàn từ thuộc tính data-table-id
            currentTableDisplay.textContent = `Bàn: ${currentTable}`; // Hiển thị bàn
            displayBill(); // Hiển thị bill của bàn
        });
    });
  
    const thanhtoan = document.getElementById("thanhtoan");
    thanhtoan.addEventListener("click", function () {
        if (!currentTable || !bills[currentTable] || bills[currentTable].items.length === 0) {
            alert("Không có hóa đơn để thanh toán!");
            return;
        }
        const banid = parseInt(document.getElementById("ban").dataset.tableId, 10);

        const bill = bills[currentTable];

        // Gửi dữ liệu hóa đơn đến MVC
        fetch("/Home/Create", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
            
                Amount: bill.total,    // Tổng số tiền cần thanh toán
                Note: `Thanh toán bàn ${currentTable}`,
                BanId: banid, 
                QRCodeUrl: ''
            })
        })
            .then(response => response.json())
            .then(data => {
              //  console.log("Full Response:", data);
              //  console.log("Data type of transactionId:", typeof data.transactionId);
              //  console.log("Transaction ID:", data.transactionId);
                if (data.success) {
                  
                    var transactionId = data.transactionId;
                
                    fetch(`/Home/GenerateQrView?transactionId=${transactionId}`, {
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

                            let printButton = document.createElement("button");
                            printButton.innerHTML = "🖨️ In hóa đơn";
                            printButton.classList.add("prinbutton"); // Thêm class cho CSS
                            printButton.onclick = function () {
                                printInvoice();
                            };
                            qrWrapper.appendChild(printButton);

                            // Thêm nút đóng (✖)
                            let closeButton = document.createElement("button");
                            closeButton.innerHTML = "✖";
                            closeButton.classList.add("qr-close-btn");
                            closeButton.onclick = function () {
                                qrContainer.style.display = "none";
                                qrContainer.innerHTML = ""; // Xóa nội dung QR khi đóng
                                delete bills[currentTable]; // Xóa hóa đơn sau khi thanh toán
                                let printableBill = document.getElementById("printableBill");
                                printableBill.style.display = "none";
                                saveBillsToLocalStorage(); // Lưu lại trạng thái
                                displayBill(); // Cập nhật giao diện
                                updateTableStatus(); // Cập nhật trạng thái bàn
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
                console.error("Error:", error);
                alert("Đã xảy ra lỗi khi thanh toán!");
            });
    });

    updateTableStatus();
});


//Mã tìm kiếm
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("searchInput"); // Lấy ô input
    const menuList = document.querySelector(".menu ul"); // Lấy thẻ <ul> chứa danh sách sản phẩm

    function loadAllProducts() {
        fetch(`/Home/Getsanpham`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json(); // Parse dữ liệu JSON từ phản hồi
            })
            .then(data => {
                menuList.innerHTML = ""; // Xóa danh sách cũ

                // Duyệt qua danh sách sản phẩm và hiển thị
                data.forEach(product => {
                    const li = document.createElement("li");
                    li.innerHTML = `
                                            <img src="data:image/jpeg;base64,${product.ImageBase64}" alt="${product.Ten}" class="menu-item-image">
                                    <span class="item-name">${product.Ten}</span>
                                        <span class="item-price">${product.Gia}đ</span>
                                    <div class="quantity-controls">
                                        <div>
                                            <button class="decrease-button">-</button>
                                            <input type="number" value="1" min="0" class="quantity-input">
                                            <button class="increase-button">+</button>
                                        </div>
                                            <div id="addproduct">+</div>
                                    </div>
                                `;
                    menuList.appendChild(li); // Thêm sản phẩm vào danh sách
                });
            })
            .catch(error => console.error("Error fetching all products:", error));
    }



    searchInput.addEventListener("input", function () {
        const query = searchInput.value.trim();

        if (!query) {
            loadAllProducts();
            return;
        }

        // Gửi yêu cầu đến Action SearchSanPham
        fetch(`/Home/SearchSanPham?query=${encodeURIComponent(query)}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => {

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json(); // Parse dữ liệu JSON từ Action
            })
            .then(data => {

                menuList.innerHTML = ""; // Xóa danh sách cũ

                if (data.length === 0) {
                    // Nếu không có sản phẩm nào
                    menuList.innerHTML = "<p>Không tìm thấy sản phẩm nào!</p>";
                } else {
                    // Hiển thị danh sách sản phẩm gợi ý
                    data.forEach(product => {

                        const li = document.createElement("li");
                        li.innerHTML = `
                                        <img src="data:image/jpeg;base64,${product.ImageBase64}" alt="${product.Ten}" class="menu-item-image">
                                <span class="item-name">${product.Ten}</span>
                                    <span class="item-price">${product.Gia}đ</span>
                                <div class="quantity-controls">
                                    <div>
                                        <button class="decrease-button">-</button>
                                        <input type="number" value="1" min="0" class="quantity-input">
                                        <button class="increase-button">+</button>
                                    </div>
                                        <div id="addproduct">+</div>
                                </div>
                            `;
                        menuList.appendChild(li); // Thêm sản phẩm vào danh sách
                    });
                }
            })
            .catch(error => {
                console.error("Error fetching data:", error);
                menuList.innerHTML = "<p>Có lỗi xảy ra khi tải dữ liệu!</p>";
            });
    });
});

