using Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEBtoSQL.Models
{
    internal class MVCSTORAGE
    {
        public string? Type { get; set; }
        public string? BoardID { get; set; }
        public string? CreatedBy { get; set; }
        public string? Failed { get; set; }
        public string? ProductTypeID { get; set; }
        public string? Flipped { get; set; }
        public string? TopBarcode { get; set; }
        public string? BottomBarcode { get; set; }
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? Thickness { get; set; }
        public string? ConveyorSpeed { get; set; }
        public string? TopClearanceHeight { get; set; }
        public string? BottomClearanceHeight { get; set; }
        public string? Weight { get; set; }
        public string? WorkOrderID { get; set; }
        public string? BatchID { get; set; }
        public string? Route { get; set; }
        public string? Action { get; set; }
        public string? SB_Pos { get; set; }
        public string? SB_Bc { get; set; }
        public string? SB_St { get; set; }

        public List<MVCSTORAGE> MVCstorage { get; internal set; }
    }
}
