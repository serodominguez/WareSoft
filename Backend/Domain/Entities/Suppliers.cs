namespace Domain.Entities
{
    public class Suppliers
    {
        public int PK_SUPPLIER { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? CONTACT { get; set; }
        public int PHONE_NUMBER { get; set; }
        public string? EMAIL { get; set; }
        public int AUDIT_CREATE_USER { get; set; }
        public DateTime AUDIT_CREATE_DATE { get; set; }
        public int AUDIT_UPDATE_USER { get; set; }
        public DateTime AUDIT_UPDATE_DATE { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public virtual ICollection<GoodsReceipt> GoodsReceipt { get; set; } = new List<GoodsReceipt>();
    }
}
