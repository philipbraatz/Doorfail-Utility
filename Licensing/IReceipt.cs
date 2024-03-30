using System.Globalization;

namespace Doorfail.ReceiptMailer.Models;

public interface IReceipt
{
    Brand Brand { get; set; }
    string Currency { get; set; }
    string Email { get; set; }
    string FirstName { get; set; }
    Uri ImageUrl { get; set; }
    string ItemName { get; set; }
    string LastName { get; set; }
    double? Price { get; set; }
    DateTime? PurchaseDate { get; set; }
    double? Shipping { get; set; }
    Uri StoreItemPage { get; set; }
    double? Tax { get; set; }

    NumberFormatInfo GetNumberFormat();
}