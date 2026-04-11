using HotelBooking.Configurations;
using HotelBooking.DTOs;
using HotelBooking.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HotelBooking.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(EmailSettings settings, ILogger<EmailService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string toName, BookingResponseDto booking)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = $"Booking Confirmed – {booking.ReservationNumber} | LuxeStay";

                var nights = (booking.CheckOutDate - booking.CheckInDate).Days;

                message.Body = new TextPart("html")
                {
                    Text = BuildEmailHtml(toName, booking, nights)
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Confirmation email sent to {Email} for booking {ReservationNumber}",
                    toEmail, booking.ReservationNumber);
            }
            catch (Exception ex)
            {
                // Log but don't throw — email failure should not break the booking
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", toEmail);
            }
        }

        private static string BuildEmailHtml(string name, BookingResponseDto b, int nights)
        {
            var discount = b.DiscountAmount > 0
                ? $@"<tr>
                        <td style='padding:10px 0;color:#6b7280;border-bottom:1px solid #2a2a3a;'>Discount Applied</td>
                        <td style='padding:10px 0;color:#6ee7b7;text-align:right;border-bottom:1px solid #2a2a3a;'>
                            −₹{b.DiscountAmount:N0}
                        </td>
                     </tr>"
                : "";

            var specialRequests = !string.IsNullOrWhiteSpace(b.SpecialRequests)
                ? $@"<tr>
                        <td style='padding:10px 0;color:#6b7280;border-bottom:1px solid #2a2a3a;'>Special Requests</td>
                        <td style='padding:10px 0;color:#e8e8f0;text-align:right;border-bottom:1px solid #2a2a3a;'>
                            {b.SpecialRequests}
                        </td>
                     </tr>"
                : "";

            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
</head>
<body style='margin:0;padding:0;background:#0a0a14;font-family:Georgia,serif;'>

  <table width='100%' cellpadding='0' cellspacing='0' style='background:#0a0a14;padding:40px 20px;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0'
               style='background:#0f0f22;border-radius:16px;border:1px solid rgba(212,175,55,0.25);overflow:hidden;max-width:600px;'>

          <!-- Header -->
          <tr>
            <td style='background:linear-gradient(135deg,#1a1a2e,#0f0f22);padding:36px 40px;text-align:center;
                        border-bottom:1px solid rgba(212,175,55,0.2);'>
              <div style='font-size:22px;color:#d4af37;letter-spacing:2px;margin-bottom:8px;'>✦ LuxeStay</div>
              <div style='font-size:13px;color:#6b7280;letter-spacing:1px;text-transform:uppercase;'>
                Booking Confirmation
              </div>
            </td>
          </tr>

          <!-- Greeting -->
          <tr>
            <td style='padding:36px 40px 0;'>
              <h1 style='margin:0 0 8px;font-size:24px;color:#e8e8f0;font-weight:700;'>
                Your stay is confirmed, {name}!
              </h1>
              <p style='margin:0;font-size:15px;color:#6b7280;line-height:1.6;'>
                Thank you for choosing LuxeStay. We look forward to welcoming you.
              </p>
            </td>
          </tr>

          <!-- Reservation Number Badge -->
          <tr>
            <td style='padding:24px 40px;'>
              <div style='background:rgba(212,175,55,0.08);border:1px solid rgba(212,175,55,0.25);
                          border-radius:10px;padding:20px;text-align:center;'>
                <div style='font-size:12px;color:#6b7280;letter-spacing:1.5px;
                            text-transform:uppercase;margin-bottom:8px;'>
                  Reservation Number
                </div>
                <div style='font-size:28px;font-weight:700;color:#d4af37;letter-spacing:2px;'>
                  {b.ReservationNumber}
                </div>
                <div style='margin-top:10px;'>
                  <span style='background:rgba(16,185,129,0.12);color:#6ee7b7;font-size:12px;
                               font-weight:700;padding:4px 14px;border-radius:20px;
                               text-transform:uppercase;letter-spacing:1px;'>
                    ✓ {b.Status}
                  </span>
                </div>
              </div>
            </td>
          </tr>

          <!-- Hotel & Room Info -->
          <tr>
            <td style='padding:0 40px 24px;'>
              <table width='100%' cellpadding='0' cellspacing='0'
                     style='background:rgba(255,255,255,0.03);border-radius:10px;
                             border:1px solid rgba(255,255,255,0.07);overflow:hidden;'>
                <tr>
                  <td style='padding:16px 20px;border-bottom:1px solid rgba(255,255,255,0.07);'>
                    <div style='font-size:11px;color:#6b7280;text-transform:uppercase;
                                letter-spacing:1px;margin-bottom:4px;'>Hotel</div>
                    <div style='font-size:18px;font-weight:700;color:#e8e8f0;'>{b.HotelName}</div>
                  </td>
                </tr>
                <tr>
                  <td style='padding:16px 20px;'>
                    <div style='font-size:11px;color:#6b7280;text-transform:uppercase;
                                letter-spacing:1px;margin-bottom:4px;'>Room</div>
                    <div style='font-size:16px;color:#e8e8f0;'>
                      {b.RoomType} &nbsp;·&nbsp; Room {b.RoomNumber}
                    </div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Check-in / Check-out -->
          <tr>
            <td style='padding:0 40px 24px;'>
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td width='48%'
                      style='background:rgba(255,255,255,0.03);border-radius:10px;
                              border:1px solid rgba(255,255,255,0.07);padding:16px 20px;
                              text-align:center;vertical-align:top;'>
                    <div style='font-size:11px;color:#6b7280;text-transform:uppercase;
                                letter-spacing:1px;margin-bottom:6px;'>Check-in</div>
                    <div style='font-size:18px;font-weight:700;color:#d4af37;'>
                      {b.CheckInDate:dd MMM yyyy}
                    </div>
                    <div style='font-size:12px;color:#6b7280;margin-top:4px;'>From 2:00 PM</div>
                  </td>
                  <td width='4%' style='text-align:center;color:#6b7280;font-size:18px;'>→</td>
                  <td width='48%'
                      style='background:rgba(255,255,255,0.03);border-radius:10px;
                              border:1px solid rgba(255,255,255,0.07);padding:16px 20px;
                              text-align:center;vertical-align:top;'>
                    <div style='font-size:11px;color:#6b7280;text-transform:uppercase;
                                letter-spacing:1px;margin-bottom:6px;'>Check-out</div>
                    <div style='font-size:18px;font-weight:700;color:#d4af37;'>
                      {b.CheckOutDate:dd MMM yyyy}
                    </div>
                    <div style='font-size:12px;color:#6b7280;margin-top:4px;'>Until 11:00 AM</div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Booking Details Table -->
          <tr>
            <td style='padding:0 40px 24px;'>
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td style='padding:10px 0;color:#6b7280;border-bottom:1px solid #2a2a3a;'>
                    Duration
                  </td>
                  <td style='padding:10px 0;color:#e8e8f0;text-align:right;border-bottom:1px solid #2a2a3a;'>
                    {nights} night{(nights > 1 ? "s" : "")}
                  </td>
                </tr>
                <tr>
                  <td style='padding:10px 0;color:#6b7280;border-bottom:1px solid #2a2a3a;'>Guests</td>
                  <td style='padding:10px 0;color:#e8e8f0;text-align:right;border-bottom:1px solid #2a2a3a;'>
                    {b.NumberOfGuests}
                  </td>
                </tr>
                {discount}
                {specialRequests}
                <tr>
                  <td style='padding:14px 0 0;color:#e8e8f0;font-size:16px;font-weight:700;'>
                    Total Amount
                  </td>
                  <td style='padding:14px 0 0;color:#d4af37;font-size:20px;font-weight:700;text-align:right;'>
                    ₹{b.TotalAmount:N0}
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Divider -->
          <tr>
            <td style='padding:0 40px 24px;'>
              <div style='height:1px;background:rgba(212,175,55,0.15);'></div>
            </td>
          </tr>

          <!-- Important Info -->
          <tr>
            <td style='padding:0 40px 24px;'>
              <div style='background:rgba(212,175,55,0.05);border-left:3px solid #d4af37;
                          border-radius:0 8px 8px 0;padding:16px 20px;'>
                <div style='font-size:13px;font-weight:700;color:#d4af37;margin-bottom:8px;
                            text-transform:uppercase;letter-spacing:0.5px;'>
                  Important Information
                </div>
                <ul style='margin:0;padding-left:18px;color:#6b7280;font-size:13px;line-height:1.9;'>
                  <li>Please carry a valid photo ID at check-in</li>
                  <li>Present this reservation number at the front desk</li>
                  <li>Free cancellation up to 24 hours before check-in</li>
                  <li>Contact the hotel directly for early check-in requests</li>
                </ul>
              </div>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='padding:24px 40px;text-align:center;
                        border-top:1px solid rgba(212,175,55,0.15);'>
              <div style='font-size:13px;color:#d4af37;margin-bottom:6px;'>✦ LuxeStay Hotels</div>
              <div style='font-size:12px;color:#4b5563;'>
                This is an automated confirmation email. Please do not reply.
              </div>
              <div style='font-size:12px;color:#4b5563;margin-top:4px;'>
                © {DateTime.UtcNow.Year} LuxeStay. All rights reserved.
              </div>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>

</body>
</html>";
        }
    }
}