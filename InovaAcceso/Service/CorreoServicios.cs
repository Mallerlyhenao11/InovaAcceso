using InovaAcceso.Models;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;


namespace InovaAcceso.Service
{
    public static class CorreoServicio
    {
        private static string _Host = "smtp.gmail.com";
        private static int _Puerto = 587;
        private static string _NombreEnvia = "Soporte InovaAcceso";
        private static string _Correo = "gisela1993jc1208@gmail.com";
        private static string _Clave = "vhle yirp fcrx pzdw";
       
        public static bool Enviar(EmailSettings EmailSettings)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(_NombreEnvia, _Correo));
                email.To.Add(MailboxAddress.Parse(EmailSettings.To));
                email.Subject = EmailSettings.Subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = EmailSettings.Body
                };

                var smtp = new SmtpClient();
                smtp.Connect(_Host, _Puerto, SecureSocketOptions.StartTls);

                smtp.Authenticate(_Correo, _Clave);
                smtp.Send(email);
                smtp.Disconnect(true);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}