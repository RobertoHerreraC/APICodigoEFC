using NuGet.Protocol.Core.Types;

namespace APICodigoEFC.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
