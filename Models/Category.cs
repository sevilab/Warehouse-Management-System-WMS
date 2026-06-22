namespace WarehouseManagementSystem.Models
{
    public enum Category
    {
        Üst_Giyim,
        Alt_Giyim,
        Dış_Giyim,
        İç_Giyim,
        Spor_Giyim,
        Aksesuar,
        Ayakkabı,
        Çanta,
        Diğer
    }

    public enum TransactionType
    {
        StokGirisi,
        StokCikisi,
        Transfer,
        Iade
    }

    public enum AlertLevel
    {
        Normal,
        Düşük,
        Kritik
    }
}