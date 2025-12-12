using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder;
using Rise.Shared.StudentCards;

namespace Rise.Client.Account.Components;

/// <summary>
/// Component that displays the user's digital student card with QR code.
/// Generates a QR code from student data for identification.
/// </summary>
public partial class StudentCard : ComponentBase
{
    /// <summary>Service for student card operations.</summary>
    [Inject] public required IStudentCardService StudentCardService { get; set; }
    
    /// <summary>JavaScript runtime for interop calls.</summary>
    [Inject] public required IJSRuntime JSRuntime { get; set; }
    
    /// <summary>The student card data to display.</summary>
    public required StudentCardDto? StudentCardDto { get; set; }
    
    /// <summary>
    /// Generates a QR code image from the student card data.
    /// </summary>
    /// <returns>PNG byte array of the QR code, or empty array if no data.</returns>
    public byte[] GetQRCode()
    {
        if (StudentCardDto == null)
        {
            return Array.Empty<byte>();
        }
        string studentData = StudentCardDto.ToJsonObject().ToJsonString();
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(studentData, QRCodeGenerator.ECCLevel.L);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(10, drawQuietZones: false);
        return qrCodeImage;
    }

    /// <summary>
    /// Loads the student card data on initialization.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        var result = await StudentCardService.GetByUserIdAsync();
        if (result.IsSuccess)
        {
            StudentCardDto = result.Value;
        }
        else
        {
            StudentCardDto = null;
        }
    }

    /// <summary>
    /// Scales the student card after rendering via JavaScript interop.
    /// </summary>
    /// <param name="firstRender">True if this is the first render.</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (StudentCardDto != null)
        {
            await JSRuntime.InvokeVoidAsync("scaleStudentCard");
        }
    }
}
