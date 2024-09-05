namespace APICodigoEFC.Models
{
    public class Detail
    {
        public int DetailID { get; set; }
        public bool IsActive { get; set; }

        public int Amount { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public int ProductID { get; set; }
        public Product? Product { get; set; }

        public int InvoiceID { get; set; }
        public Invoice? Invoice { get; set; }

    }
}
