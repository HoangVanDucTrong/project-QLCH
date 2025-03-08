function toggleForm() {
    var form = document.getElementById("Tableform");
    if (form.style.display === "block") {
        form.style.display = "none";
    } else {
        form.style.display = "block";
    }
}

window.onclick = function (event) {
    var form = document.getElementById("Tableform");
    if (event.target != form && !form.contains(event.target) && event.target.className != "add-btn") {
        form.style.display = "none";
    }
}

document.addEventListener("DOMContentLoaded", function () {
    let tables = document.querySelectorAll(".tablee");

    tables.forEach(table => {
        table.addEventListener("click", async function () {
            let banId = this.dataset.tableId;
            let soban = this.dataset.soban;
            console.log("Bàn được chọn:", banId);

            try {
                let response = await fetch(`/Ban/GetQR?banId=${banId}`);

                if (!response.ok) {
                    alert("Không thể lấy mã QR cho bàn này!");
                    return;
                }

                // Nhận dữ liệu ảnh dưới dạng blob
                let blob = await response.blob();
                let qrImageUrl = URL.createObjectURL(blob);

                // Hiển thị ảnh QR
                showQRCode(qrImageUrl, soban);
            } catch (error) {
                console.error("Lỗi khi gọi API QR:", error);
                alert("Có lỗi xảy ra khi tải mã QR.");
            }
        });
    });

    function showQRCode(qrImageUrl, soban) {
        let qrContainer = document.getElementById("qrContainer");
        qrContainer.innerHTML = ""; // Xóa nội dung cũ trước khi thêm mới

        // Tạo nút đóng (X)
        let closeButton = document.createElement("span");
        closeButton.innerHTML = "&times;";
        closeButton.style.position = "absolute";
        closeButton.style.top = "20px";
        closeButton.style.right = "30px";
        closeButton.style.fontSize = "40px";
        closeButton.style.color = "#fff";
        closeButton.style.cursor = "pointer";
        closeButton.onclick = function () {
            qrContainer.style.display = "none";
        };
        qrContainer.appendChild(closeButton);

        // Thêm tiêu đề
        let title = document.createElement("h3");
        title.textContent = `Mã QR của bàn ${soban}`;
        title.style.color = "#fff";
        qrContainer.appendChild(title);

        // Thêm ảnh QR
        let qrImage = document.createElement("img");
        qrImage.id = "qrImage";
        qrImage.style.width = "400px";
        qrImage.style.height = "400px";
        qrImage.src = qrImageUrl;
        qrImage.style.margin = "20px";
        qrContainer.appendChild(qrImage);

        // Thêm nút in
        let printButton = document.createElement("button");
        printButton.textContent = "🖨️ In mã QR";
        printButton.style.padding = "10px 20px";
        printButton.style.fontSize = "16px";
        printButton.style.cursor = "pointer";
        printButton.onclick = function () {
            let printWindow = window.open('', '_blank');
            printWindow.document.write(`<html><head><title>In mã QR</title></head><body>`);
            printWindow.document.write(`<h3>Mã QR của bàn ${soban}</h3>`);
            printWindow.document.write(`<img src="${qrImageUrl}" style="width:400px;height:400px;">`);
            printWindow.document.write(`</body></html>`);
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
            printWindow.close();
        };
        qrContainer.appendChild(printButton);

        qrContainer.style.display = "flex"; // Hiển thị QR
    }


});
