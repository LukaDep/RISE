using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder;
using Rise.Shared.StudentCards;

namespace Rise.Client.Account.Components;

public partial class StudentCard : ComponentBase
{
    [Inject] public required IStudentCardService StudentCardService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }
    public required StudentCardDto? StudentCardDto { get; set; }
    public byte[]? GetQRCode()
    {
        if (StudentCardDto == null)
        {
            return null;
        }

        string studentData = StudentCardDto.ToJsonObject().ToJsonString();
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(studentData, QRCodeGenerator.ECCLevel.L);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(10, drawQuietZones: false);
        return qrCodeImage;
    }

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
            // Optionally, handle the error (e.g., log, show message)
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (StudentCardDto != null)
        {
            await JSRuntime.InvokeVoidAsync("scaleStudentCard");
        }
    }
}
