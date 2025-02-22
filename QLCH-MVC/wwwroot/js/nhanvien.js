<<<<<<< HEAD
﻿//phần nhân viên 
function toggleForm() {
    var form = document.getElementById("employeeForm");
    if (form.style.display === "block") {
        form.style.display = "none";
    } else {
        form.style.display = "block";
    }
}

window.onclick = function (event) {
    var form = document.getElementById("employeeForm");
    if (event.target != form && !form.contains(event.target) && event.target.className != "add-btn") {
        form.style.display = "none";
    }
}


=======
﻿//phần nhân viên 
function toggleForm() {
    var form = document.getElementById("employeeForm");
    if (form.style.display === "block") {
        form.style.display = "none";
    } else {
        form.style.display = "block";
    }
}

window.onclick = function (event) {
    var form = document.getElementById("employeeForm");
    if (event.target != form && !form.contains(event.target) && event.target.className != "add-btn") {
        form.style.display = "none";
    }
}


>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
//kết thúc phần nhân viên 