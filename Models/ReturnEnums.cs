namespace WarehouseManagementSystem.Models
{
    public enum ReturnReason
    {
        Kusurlu,
        YanlisUrun,
        Vazgecildi,
        BedenUyumsuzlugu,
        Diger
    }

    public enum ReturnAction
    {
        StokaGeriAl,
        HurdayaAyir
    }
}
