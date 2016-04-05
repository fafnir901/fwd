using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FWD.Services
{
	public class EmailService
	{
		//http://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
		private readonly string _smtpServerDefault = "smtp.gmail.com";
		private readonly string _fromDefault = "shkorodenok@gmail.com";
		private readonly string _passwordDefault = "DotNetZip901";

		private string _smtpServer;
		private string _from;
		private string _password;

		public string SmptServer
		{
			get
			{
				if (string.IsNullOrEmpty(_smtpServer))
				{
					return _smtpServerDefault;
				}
				else
				{
					return _smtpServer;
				}
			}
			set
			{
				_smtpServer = value;
			}
		}

		public string From
		{
			get
			{
				if (string.IsNullOrEmpty(_from))
				{
					return _fromDefault;
				}
				else
				{
					return _from;
				}
			}
			set
			{
				_from = value;
			}
		}

		public string Password
		{
			get
			{
				if (string.IsNullOrEmpty(_password))
				{
					return _passwordDefault;
				}
				else
				{
					return _password;
				}
			}
			set
			{
				_password = value;
			}
		}

		private EmailService()
		{

		}

		private static EmailService _instance;

		public static EmailService Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new EmailService();
				}
				return _instance;
			}
		}

		public void SendMail(string mailto, string caption, string message, string mailTitle =  "Список статей: ", string attachFile = null)
		{
			try
			{
				var buidedMessage = BuildMessage(message, mailTitle);
				SendMail(SmptServer, From, Password, mailto, caption, buidedMessage, attachFile);
			}
			catch (Exception)
			{
				throw;
			}

		}

		private string BuildMessage(string message, string mailTitle = "Список статей: ")
		{
			var builder = new StringBuilder();
			builder.Append("<html xmlns=\"" + "http://www.w3.org/1999/xhtml" + "\"><head><title>Статьевод |" +
						   " Уведомление</title></head><body style=\"margin: 0px; padding: 0px; background: #f1f2f3;\">" +
						   "<div style=\"margin: 10px\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"background: #fff; padding: 15px; " +
						   "margin: 0px 0px 5px 0px; border: 1px solid #dddddd; font-family: Arial;\"><tr><td><h2>{0}</h2><h4>".Fmt(mailTitle));
			builder.Append(message);
			builder.Append("</h4><h4><span style=\"font-weight: normal; color: #666666\">Добавлено:</span><br>  Статьеводом в " + DateTime.Now.ToString());
			builder.Append("</h4></td></tr></table><div style=\"margin: 0px 35px; padding: 0px 0px 40px 0px\">" +
						   "<span style=\"color: #666666; font-weight: normal; font-size: 11px;\">" +
						   "Это сообщение сгенерированно автоматически с помощью \"Статьевод\".</span></div></div></body></html>");
			return builder.ToString();
		}

		private void SendMail(string smtpServer, string from, string password, string mailto, string caption, string message, string attachFile = null)
		{
			var mail = new MailMessage
			{
				From = new MailAddress(from, "Статьевод"),
				Subject = caption,
				Body = message,
				IsBodyHtml = true
			};

			mail.To.Add(new MailAddress(mailto));
			if (!string.IsNullOrEmpty(attachFile))
				mail.Attachments.Add(new Attachment(attachFile));
			var client = new SmtpClient
			{
				Host = smtpServer,
				Port = 587,
				EnableSsl = true,
				Credentials = new NetworkCredential(from, password),
				DeliveryMethod = SmtpDeliveryMethod.Network
			};
			try
			{
				client.Send(mail);
				mail.Dispose();

			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				client.Dispose();
				mail.Dispose();
			}
		}
	}
}
