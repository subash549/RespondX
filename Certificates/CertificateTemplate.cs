using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Web;
using RespondX.Models;

namespace RespondX.Certificates
{
    public static class CertificateTemplate
    {
        private static readonly string TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "Templates");
        private static readonly string OutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "Generated");

        static CertificateTemplate()
        {
            // Ensure directories exist
            if (!Directory.Exists(TemplatePath))
                Directory.CreateDirectory(TemplatePath);

            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);
        }

        /// <summary>
        /// Generates a certificate image for a learner
        /// </summary>
        public static byte[] GenerateCertificateImage(Certificate certificate)
        {
            try
            {
                // Create a new bitmap for the certificate
                int width = 1200;
                int height = 850;
                using (var bitmap = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // Draw background
                    using (var background = new SolidBrush(Color.FromArgb(250, 248, 242)))
                    {
                        graphics.FillRectangle(background, 0, 0, width, height);
                    }

                    // Draw border
                    using (var borderPen = new Pen(Color.FromArgb(25, 42, 70), 8))
                    {
                        graphics.DrawRectangle(borderPen, 20, 20, width - 40, height - 40);
                    }

                    // Draw inner border
                    using (var innerPen = new Pen(Color.FromArgb(201, 168, 76), 2))
                    {
                        graphics.DrawRectangle(innerPen, 40, 40, width - 80, height - 80);
                    }

                    // Draw decorative corners
                    DrawCornerDecoration(graphics, 40, 40, true, true);
                    DrawCornerDecoration(graphics, width - 40, 40, false, true);
                    DrawCornerDecoration(graphics, 40, height - 40, true, false);
                    DrawCornerDecoration(graphics, width - 40, height - 40, false, false);

                    // Draw seal
                    DrawSeal(graphics, width / 2, 180, 100);

                    // Draw title
                    using (var titleFont = new Font("Georgia", 42, FontStyle.Bold))
                    using (var titleBrush = new SolidBrush(Color.FromArgb(25, 42, 70)))
                    {
                        var titleText = "Certificate of Completion";
                        var titleSize = graphics.MeasureString(titleText, titleFont);
                        var titleX = (width - titleSize.Width) / 2;
                        graphics.DrawString(titleText, titleFont, titleBrush, titleX, 260);
                    }

                    // Draw subtitle
                    using (var subFont = new Font("Georgia", 16, FontStyle.Regular))
                    using (var subBrush = new SolidBrush(Color.FromArgb(166, 128, 45)))
                    {
                        var subText = "RespondX Emergency Response Training";
                        var subSize = graphics.MeasureString(subText, subFont);
                        var subX = (width - subSize.Width) / 2;
                        graphics.DrawString(subText, subFont, subBrush, subX, 315);
                    }

                    // Draw recipient name with a smaller font when the name is long.
                    using (var nameBrush = new SolidBrush(Color.FromArgb(25, 42, 70)))
                    {
                        DrawFittedCenteredText(graphics, certificate.LearnerName, "Georgia", FontStyle.Bold,
                            36, 24, width - 180, 390, nameBrush, width);
                    }

                    // Draw completion text
                    using (var textFont = new Font("Georgia", 14, FontStyle.Regular))
                    using (var textBrush = new SolidBrush(Color.FromArgb(74, 85, 104)))
                    {
                        var yOffset = 450;
                        var lineHeight = 30;

                        var line1 = $"has successfully completed the training module:";
                        var line1Size = graphics.MeasureString(line1, textFont);
                        graphics.DrawString(line1, textFont, textBrush, (width - line1Size.Width) / 2, yOffset);
                        yOffset += lineHeight;

                        // Module name
                        using (var moduleBrush = new SolidBrush(Color.FromArgb(166, 128, 45)))
                        {
                            DrawFittedCenteredText(graphics, certificate.ModuleTitle, "Georgia", FontStyle.Bold,
                                20, 16, width - 180, yOffset, moduleBrush, width);
                            yOffset += lineHeight + 10;
                        }

                        var line3 = $"with a score of {certificate.Score:F1}%";
                        var line3Size = graphics.MeasureString(line3, textFont);
                        graphics.DrawString(line3, textFont, textBrush, (width - line3Size.Width) / 2, yOffset);
                        yOffset += lineHeight;

                        var line4 = $"Awarded on {certificate.IssueDate:MMMM dd, yyyy}";
                        var line4Size = graphics.MeasureString(line4, textFont);
                        graphics.DrawString(line4, textFont, textBrush, (width - line4Size.Width) / 2, yOffset);
                    }

                    // Draw signature line
                    using (var signFont = new Font("Georgia", 12, FontStyle.Regular))
                    using (var signBrush = new SolidBrush(Color.FromArgb(160, 174, 192)))
                    {
                        var signX = 120;
                        var signY = height - 140;
                        graphics.DrawLine(new Pen(Color.FromArgb(160, 174, 192), 2), signX, signY, signX + 250, signY);
                        graphics.DrawString("Authorized Signature", signFont, signBrush, signX, signY + 10);
                        graphics.DrawString(certificate.Issuer, new Font("Georgia", 11, FontStyle.Regular), signBrush, signX + 70, signY + 30);
                    }

                    // Draw certificate number
                    using (var codeFont = new Font("Georgia", 10, FontStyle.Regular))
                    using (var codeBrush = new SolidBrush(Color.FromArgb(160, 174, 192)))
                    {
                        var codeX = width - 350;
                        var codeY = height - 140;
                        graphics.DrawString($"Certificate Number:", codeFont, codeBrush, codeX, codeY);
                        graphics.DrawString(certificate.CertificateNumber, new Font("Georgia", 10, FontStyle.Bold), codeBrush, codeX + 130, codeY);

                        var codeY2 = codeY + 25;
                        graphics.DrawString($"Verification Code:", codeFont, codeBrush, codeX, codeY2);
                        graphics.DrawString(certificate.VerificationCode, new Font("Georgia", 10, FontStyle.Bold), codeBrush, codeX + 120, codeY2);
                    }

                    // Draw footer line
                    using (var footerPen = new Pen(Color.FromArgb(201, 168, 76), 1))
                    {
                        graphics.DrawLine(footerPen, 80, height - 80, width - 80, height - 80);
                    }

                    // Save to memory stream
                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                RespondX.Helpers.DatabaseHelper.LogError("Certificate Generation Error", ex.Message);
                return null;
            }
        }

        private static void DrawCornerDecoration(Graphics graphics, int x, int y, bool left, bool top)
        {
            int size = 30;
            int margin = 40;

            using (var pen = new Pen(Color.FromArgb(201, 168, 76), 3))
            {
                int startX = left ? x + margin : x - margin;
                int endX = left ? x + margin + size : x - margin - size;
                int startY = top ? y + margin : y - margin;
                int endY = top ? y + margin + size : y - margin - size;

                graphics.DrawLine(pen, startX, startY, left ? startX + size : startX - size, startY);
                graphics.DrawLine(pen, startX, startY, startX, top ? startY + size : startY - size);
            }
        }

        private static void DrawFittedCenteredText(Graphics graphics, string text, string fontFamily,
            FontStyle fontStyle, float maxFontSize, float minFontSize, float maxWidth, float y, Brush brush, int canvasWidth)
        {
            text = text ?? string.Empty;
            for (float fontSize = maxFontSize; fontSize >= minFontSize; fontSize -= 1)
            {
                using (var font = new Font(fontFamily, fontSize, fontStyle))
                {
                    var size = graphics.MeasureString(text, font);
                    if (size.Width <= maxWidth || fontSize <= minFontSize)
                    {
                        graphics.DrawString(text, font, brush, (canvasWidth - size.Width) / 2, y);
                        return;
                    }
                }
            }
        }

        private static void DrawSeal(Graphics graphics, int x, int y, int radius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(x - radius, y - radius, radius * 2, radius * 2);

                using (var sealBrush = new LinearGradientBrush(
                    new Point(x - radius, y - radius),
                    new Point(x + radius, y + radius),
                    Color.FromArgb(25, 42, 70),
                    Color.FromArgb(201, 168, 76)))
                {
                    graphics.FillPath(sealBrush, path);
                }

                using (var sealPen = new Pen(Color.White, 4))
                {
                    graphics.DrawPath(sealPen, path);
                }

                // Draw inner circle
                using (var innerPen = new Pen(Color.FromArgb(250, 248, 242), 2))
                {
                    graphics.DrawEllipse(innerPen, x - radius + 15, y - radius + 15, (radius - 15) * 2, (radius - 15) * 2);
                }

                // Draw a check mark as vector lines (not a font glyph, which depends on installed fonts).
                using (var checkPen = new Pen(Color.FromArgb(250, 248, 242), Math.Max(3f, radius / 6f)))
                {
                    checkPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    checkPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    checkPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    float unit = radius / 3f;
                    graphics.DrawLines(checkPen, new[]
                    {
                        new PointF(x - unit, y),
                        new PointF(x - unit / 4f, y + unit * 0.8f),
                        new PointF(x + unit * 1.1f, y - unit * 0.8f)
                    });
                }
            }
        }

        /// <summary>
        /// Saves the certificate image to file
        /// </summary>
        public static string SaveCertificateImage(Certificate certificate)
        {
            var imageBytes = GenerateCertificateImage(certificate);
            if (imageBytes == null)
                return null;

            var fileName = $"Certificate_{certificate.CertificateNumber}.png";
            var filePath = Path.Combine(OutputPath, fileName);

            File.WriteAllBytes(filePath, imageBytes);
            return filePath;
        }

        /// <summary>
        /// Gets a certificate image as base64 string for display
        /// </summary>
        public static string GetCertificateAsBase64(Certificate certificate)
        {
            var imageBytes = GenerateCertificateImage(certificate);
            if (imageBytes == null)
                return null;

            return Convert.ToBase64String(imageBytes);
        }

        /// <summary>
        /// Generates a verification badge for the certificate
        /// </summary>
        public static string GenerateVerificationBadge(string verificationCode)
        {
            return $@"
            <div style='display:inline-block;padding:10px 20px;background:#48bb78;color:white;border-radius:5px;font-weight:bold;'>
                <i class='fas fa-check-circle'></i> Verified
                <span style='font-size:12px;margin-left:10px;opacity:0.8;'>Code: {verificationCode}</span>
            </div>";
        }

        /// <summary>
        /// Generates a simple HTML certificate (for preview)
        /// </summary>
        public static string GenerateCertificateHtml(Certificate certificate)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{
                        margin: 0;
                        padding: 20px;
                        font-family: 'Georgia', serif;
                        background: #e9edf2;
                    }}
                    .certificate {{
                        border: 10px double #192a46;
                        outline: 2px solid #c9a84c;
                        outline-offset: -22px;
                        padding: 48px;
                        background: #faf8f2;
                        max-width: 1000px;
                        margin: 0 auto;
                        box-shadow: 0 10px 30px rgba(0,0,0,0.2);
                    }}
                    .certificate-header {{
                        text-align: center;
                        border-bottom: 2px solid #c9a84c;
                        padding-bottom: 24px;
                        margin-bottom: 24px;
                    }}
                    .certificate-title {{
                        font-size: 42px;
                        color: #192a46;
                        margin: 0;
                    }}
                    .certificate-subtitle {{
                        font-size: 18px;
                        color: #a6802d;
                        margin-top: 5px;
                    }}
                    .certificate-body {{
                        text-align: center;
                        padding: 20px 0;
                    }}
                    .certificate-body h2 {{
                        font-size: 28px;
                        color: #192a46;
                        margin-bottom: 10px;
                    }}
                    .certificate-body p {{
                        font-size: 16px;
                        color: #394a62;
                        line-height: 1.6;
                    }}
                    .certificate-body .module-name {{
                        font-size: 24px;
                        color: #a6802d;
                        font-weight: bold;
                    }}
                    .certificate-footer {{
                        margin-top: 30px;
                        padding-top: 20px;
                        border-top: 2px solid #c9a84c;
                        display: flex;
                        justify-content: space-between;
                    }}
                    .certificate-footer .signature {{
                        text-align: left;
                    }}
                    .certificate-footer .verification {{
                        text-align: right;
                    }}
                    .certificate-footer .label {{
                        font-size: 12px;
                        color: #68778a;
                    }}
                    .certificate-footer .value {{
                        font-size: 14px;
                        color: #192a46;
                    }}
                    .seal {{
                        display: inline-block;
                        width: 80px;
                        height: 80px;
                        border-radius: 50%;
                        background: linear-gradient(135deg, #192a46, #c9a84c);
                        color: #faf8f2;
                        border: 2px solid #faf8f2;
                        box-shadow: 0 0 0 2px #c9a84c;
                        line-height: 0;
                        text-align: center;
                        padding-top: 20px;
                        box-sizing: border-box;
                        margin: 20px auto;
                    }}
                </style>
            </head>
            <body>
                <div class='certificate'>
                    <div class='certificate-header'>
                        <h1 class='certificate-title'>Certificate of Completion</h1>
                        <p class='certificate-subtitle'>RespondX Emergency Response Training</p>
                    </div>
                    
                    <div class='certificate-body'>
                        <div class='seal'><svg viewBox='0 0 24 24' width='40' height='40' aria-hidden='true'><path d='M5 12.5l4.5 4.5L19 7.5' fill='none' stroke='#faf8f2' stroke-width='3' stroke-linecap='round' stroke-linejoin='round'/></svg></div>
                        <h2>This is to certify that</h2>
                        <h2 style='color:#192a46;font-size:32px;'>{HttpUtility.HtmlEncode(certificate.LearnerName ?? string.Empty)}</h2>
                        <p>has successfully completed the training module:</p>
                        <p class='module-name'>{HttpUtility.HtmlEncode(certificate.ModuleTitle ?? string.Empty)}</p>
                        <p>with a score of {certificate.Score:F1}%</p>
                        <p>Awarded on {certificate.IssueDate:MMMM dd, yyyy}</p>
                    </div>
                    
                    <div class='certificate-footer'>
                        <div class='signature'>
                            <div class='label'>Authorized Signature</div>
                            <div class='value'>_________________</div>
                            <div class='value'>{HttpUtility.HtmlEncode(certificate.Issuer ?? string.Empty)}</div>
                        </div>
                        <div class='verification'>
                            <div class='label'>Certificate Number</div>
                            <div class='value'>{HttpUtility.HtmlEncode(certificate.CertificateNumber ?? string.Empty)}</div>
                            <div class='label'>Verification Code</div>
                            <div class='value'>{HttpUtility.HtmlEncode(certificate.VerificationCode ?? string.Empty)}</div>
                        </div>
                    </div>
                </div>
            </body>
            </html>";
        }

        /// <summary>
        /// Saves certificate as HTML file
        /// </summary>
        public static string SaveCertificateHtml(Certificate certificate)
        {
            var html = GenerateCertificateHtml(certificate);
            if (string.IsNullOrEmpty(html))
                return null;

            var fileName = $"Certificate_{certificate.CertificateNumber}.html";
            var filePath = Path.Combine(OutputPath, fileName);

            File.WriteAllText(filePath, html);
            return filePath;
        }
    }
}
