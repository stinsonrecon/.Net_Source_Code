using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Services;
using BCXN.Statics;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Data;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Epayment.Services;

namespace BCXN.Controllers
{
    [Route("api/mail")]
    public class SendMailController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<SendMailController> _logger;
        private readonly IUtilsService _service;
        public SendMailController(IConfiguration configuration, ILogger<SendMailController> logger, IUtilsService service)
        {
            this.Configuration = configuration;
            this._service = service;
            this._logger = logger;
        }
        [HttpPost("SendEx")]
        public ActionResult<Response> SendEx([FromBody] MailViewModel mail)
        {
            _logger.LogInformation("Data:", mail);
            try
            {
                this._service.SendMail(mail.toMail, null, mail.subject, mail.noiDung);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Send fail:", ex);
                return new Response(ex.Message, null, "01", false);
            }
            _logger.LogInformation("Send succes");
            return new Response("Gui email thanh cong", null, "00", true);
        }
        [HttpPost("Send")]
        public ActionResult<object> Send([FromBody] MailViewModel mail)
        {
            _logger.LogInformation("Data:", mail);
            try
            {
                string body = mail.noiDung;
                var strHost = Configuration["EmailSetting:server"];
                int strPort = Convert.ToInt32(Configuration["EmailSetting:port"]);
                string fromAdress = Configuration["EmailSetting:fromAddress"];
                string fromPassword = Configuration["EmailSetting:password"];
                string aliasName = Configuration["EmailSetting:aliasName"];
                var fromAddressMail = new MailAddress(fromAdress, aliasName);
                var toAddress = new MailAddress(mail.toMail[0], aliasName);
                if (mail.PathAttachFile != null && mail.PathAttachFile != "")
                {
                    Attachment attachment = new Attachment(mail.PathAttachFile);
                }
                var smtp = new SmtpClient
                {
                    Host = strHost,
                    Port = strPort,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddressMail.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddressMail, toAddress)
                {
                    Subject = mail.subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    if (mail.PathAttachFile != null && mail.PathAttachFile != "")
                    {
                        Attachment attachment = new Attachment(mail.PathAttachFile);
                        message.Attachments.Add(attachment);
                    }
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Send fail:", ex.Message);
                return false;
            }
            _logger.LogInformation("Send succes");
            return true;
        }
    }
}