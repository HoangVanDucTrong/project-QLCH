
﻿namespace QLCH_MVC.Models
{
    public class PutCalamViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<NhanVien> NhanViens { get; set; }
        public IEnumerable<Calam> CaLams { get; set; }
    }

}
