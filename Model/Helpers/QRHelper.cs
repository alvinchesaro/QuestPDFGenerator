using QRCoder;
using System.Drawing.Imaging;

namespace QuestPDFGenerator.Model.Helpers
{
    public static class QRHelper
    {
        public static byte[] GenerateQRCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(20);
        }
    }
}
