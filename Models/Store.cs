using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Models
{
    internal class Store
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public uint Manager { get; set; }
        public int IsActive { get; set; } = 1;
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public string ManagerFullName { get; set; }
    }
}
