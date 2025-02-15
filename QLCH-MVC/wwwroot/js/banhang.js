

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

    // Hiển thị bill
    function displayBill() {
        if (!currentTable) return;

        const bill = bills[currentTable] || { items: [], total: 0 };
        tableBody.innerHTML = ""; // Xóa danh sách cũ

        bill.items.forEach(item => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td class="delete-btn">x</td>
                <td>${item.name}</td>
                <td>${item.quantity}</td>
                <td>${item.totalPrice.toLocaleString()}đ</td>
            `;

            // Sự kiện xóa món
            row.querySelector(".delete-btn").addEventListener("click", function () {
                const index = bill.items.indexOf(item);
                if (index > -1) {
                    bill.items.splice(index, 1);
                    updateTotalAmount(); // Cập nhật tổng tiền
                    updateTableStatus();
                    saveBillsToLocalStorage(); // Lưu lại trạng thái
                    displayBill(); // Hiển thị lại bill
                }
            });

            tableBody.appendChild(row);
        });

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
        tableElements.forEach(table => {
            const tableId = table.dataset.tableId;
            if (bills[tableId] && bills[tableId].items.length > 0) {
                table.classList.add("in-use"); // Thêm class màu đỏ
                table.classList.remove("available"); // Xóa class màu xanh
            } else {
                table.classList.add("available"); // Thêm class màu xanh
                table.classList.remove("in-use"); // Xóa class màu đỏ
            }
        });
    }
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
            updateTableStatus(); // Cập nhật trạng thái bàn
        }
    });

    // Gắn sự kiện chọn bàn
    tableElements.forEach(table => {
        table.addEventListener("click", function () {
            currentTable = this.dataset.tableId; // Lấy ID bàn từ thuộc tính data-table-id
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
                QRCodeUrl: ''
            })
        })
            .then(response => response.json())
            .then(data => {
              //  console.log("Full Response:", data);
              //  console.log("Data type of transactionId:", typeof data.transactionId);
              //  console.log("Transaction ID:", data.transactionId);
                if (data.success) {
                    alert("Thanh toán thành công!");
                    var transactionId = data.transactionId;
                    delete bills[currentTable]; // Xóa hóa đơn sau khi thanh toán
                    fetch(`/Home/GenerateQrView?transactionId=${transactionId}`, {
                        method: "GET"
                    })
                        .then(response => response.text())
                        .then(html => {
                            let qrContainer = document.getElementById("qrViewContainer");

                            // Xóa nội dung cũ trước khi thêm mới
                            qrContainer.innerHTML = "";

                            // Thêm nút đóng (✖)
                            let closeButton = document.createElement("button");
                            closeButton.innerHTML = "✖";
                            closeButton.classList.add("qr-close-btn");
                            closeButton.onclick = function () {
                                qrContainer.style.display = "none";
                                qrContainer.innerHTML = ""; // Xóa nội dung QR khi đóng
                            };

                            // Hiển thị QR
                            qrContainer.style.display = "flex";
                            qrContainer.insertAdjacentHTML("beforeend", html);
                            qrContainer.appendChild(closeButton);
                        })
                        .catch(error => {
                            console.error("Error loading QR view:", error); 
                            alert("Không thể tải mã QR!");
                        });
                    saveBillsToLocalStorage(); // Lưu lại trạng thái
                    displayBill(); // Cập nhật giao diện
                    updateTableStatus(); // Cập nhật trạng thái bàn
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


