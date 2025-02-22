


    //Hành động kéo thả
    function onDragStart(event) {
        // Lưu ID của nhân viên khi bắt đầu kéo
        event.dataTransfer.setData("NVid", event.target.id);
    event.dataTransfer.effectAllowed = "move"; // Đảm bảo hiệu ứng kéo là "move"
    }

    // Cho phép thả vào các ô
    function onDragOver(event) {
        event.preventDefault(); // Cho phép thả vào ô khi kéo qua
    }

    // Hàm xử lý khi thả nhân viên vào ô
function onDrop(event) {
    event.preventDefault();

    // Lấy ID của nhân viên được kéo
    var employeeId = event.dataTransfer.getData("NVid");
    var employeeElement = document.getElementById(employeeId);

    // Xác định ô mục tiêu (td)
    var dropTarget = event.target.closest("td");

    // Chỉ xử lý khi mục tiêu là một ô td hợp lệ
    if (dropTarget && dropTarget.tagName.toLowerCase() === "td") {
        // Kiểm tra xem nhân viên đã tồn tại trong ô chưa
        var existingEmployees = dropTarget.querySelectorAll(".employee-item");
        for (var i = 0; i < existingEmployees.length; i++) {
            if (existingEmployees[i].getAttribute("data-id") === employeeElement.id.split('-')[1]) {
                alert("Nhân viên này đã được thêm vào ô!");
                return; // Dừng việc thêm nếu trùng lặp
            }
        }

        // Tạo container cho nhân viên
        var employeeDiv = document.createElement("div");
        employeeDiv.className = "employee-item";
        employeeDiv.setAttribute("data-id", employeeElement.id.split('-')[1]);

        // Tạo span hiển thị tên nhân viên
        var employeeSpan = document.createElement("span");
        employeeSpan.textContent = employeeElement.textContent; // Lấy tên từ phần tử nhân viên
        employeeSpan.className = "employee-name";

        // Tạo nút xóa
        var deleteButton = document.createElement("button");
        deleteButton.textContent = "X"; // Hiển thị "X" làm nút xóa
        deleteButton.className = "delete-btn";
        deleteButton.onclick = function () {
            dropTarget.removeChild(employeeDiv); // Xóa container khỏi ô
        };

        // Thêm tên và nút xóa vào container
        employeeDiv.appendChild(employeeSpan);
        employeeDiv.appendChild(deleteButton);

        // Thêm container vào ô
        dropTarget.appendChild(employeeDiv);
    } else {
        console.log("Không thể thả vào mục tiêu này");
    }
}



        function toggleCustomInput(shift) {
        const selectElement = document.getElementById(`${shift}-shift`);
        const customInputElement = document.getElementById(`${shift}-custom`);

        if (selectElement.value === 'custom') {
            customInputElement.style.display = 'inline-block'; // Hiển thị ô nhập
        customInputElement.focus();
        } else {
            customInputElement.style.display = 'none'; // Ẩn ô nhập
        customInputElement.value = ''; // Xóa giá trị đã nhập
        }
    }

