using System.IO;
using System.Threading.Tasks;
using Godot;
using MailKit.Net.Smtp;
using MimeKit;

public static class EmailSender
{
    private static readonly string attachementDir = Path.Join(Manager.SaveDir, "stundenzettel");

    public static Task SendEmailWithAttachement(string fileName)
    {
        return Task.Run(() =>
        {
            MimeMessage message = new();
            
            message.From.Add(new MailboxAddress(Settings.UserSettings.User, Settings.UserSettings.MailAddress));
            message.To.Add(MailboxAddress.Parse(Settings.UserSettings.TargetAddress));
            message.Subject = fileName;

            BodyBuilder builder = new();
            var attachementPath = Path.Join(attachementDir, $"{fileName}.pdf");
            var fileBytes = Godot.FileAccess.GetFileAsBytes(attachementPath);
            if (Godot.FileAccess.GetOpenError() != Error.Ok)
            {
                GD.Print(Godot.FileAccess.GetOpenError());
                return;
            }
            builder.Attachments.Add($"{fileName}.pdf", fileBytes);
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(Settings.UserSettings.MailAddress, Settings.UserSettings.AppPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        });
    }
}