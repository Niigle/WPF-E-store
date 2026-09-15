using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class RoleCheckboxItem
    {
        public uint RoleId { get; set; }
        public string RoleName { get; set; }
        public bool isChecked { get; set; }
    }
}
