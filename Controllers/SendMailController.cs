using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendMailAPI.Models;

namespace SendMailAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class SendMailController : Controller
    {       
        [HttpPost]
        public async Task<ActionResult<MessageCode>> Send(Message message)
        {
            MessageCode mc = SendMessage(message);
            return Ok(mc);
        }

        public MessageCode SendMessage(Message message)
        {            
            short puerto = 587;
            //short security = 3072; //TLS 1.2
            string asunto = "New Message";
            string host = "smtp.office365.com";
            bool isBodyHTML = true;

            MessageCode codeBack = new MessageCode();
            
            if (string.IsNullOrEmpty(message.senderEmail) 
                || string.IsNullOrEmpty(message.password) 
                || string.IsNullOrEmpty(message.TO))
            {
                codeBack.code = 0;
                codeBack.description = "Error. Data to send message is not complete";
                return codeBack;
            }
            if(message.tls > 0) 
            {
               System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)message.tls;
            }
            // System.Net.ServicePointManager.SecurityProtocol 
            //     |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12
            //     | System.Net.SecurityProtocolType.Tls13;

            //Creamos un nuevo Objeto de mensaje
            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

            //Direccion de correo electronico a la que queremos enviar el mensaje            
            if(message.TO.Contains(','))
            {
                string[] _to  = message.TO.Split(",");
                foreach (string l in _to)
                {
                    mmsg.To.Add(l);
                }
            }
            else if(message.TO.Contains(';'))
            {
                string[] _to  = message.TO.Split(";");
                foreach (string l in _to)
                {
                    mmsg.To.Add(l);
                }
            }
            else
            {
                mmsg.To.Add(message.TO);
            }
            
            //Direccion de correo electronico que queremos que reciba una copia del mensaje
            if(message.CCO.Contains(','))
            {
                string[] _cco  = message.CCO.Split(",");
                foreach (string l in _cco)
                {
                    mmsg.Bcc.Add(l);
                }
            }
            else if(message.CCO.Contains(';'))
            {
                string[] _cco  = message.CCO.Split(";");
                foreach (string l in _cco)
                {
                    mmsg.Bcc.Add(l);
                }
            }
            else
            {
                mmsg.Bcc.Add(message.CCO);
            }
            //Asunto
            mmsg.Subject = string.IsNullOrEmpty(message.subject) ? asunto :  message.subject;            
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            string attachment = string.IsNullOrEmpty(message.adjuntoURL) ? "" : message.adjuntoURL;
            if(attachment != "") mmsg.Attachments.Add( new System.Net.Mail.Attachment(message.adjuntoURL));

            //cuerpo del mensaje
            mmsg.Body = message.message;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = isBodyHTML; //Si queremos que se envíe como HTML
            
            //Correo electronico desde el que enviamos el mensaje
            mmsg.From = new System.Net.Mail.MailAddress(message.senderEmail);

            /*-------------------------CLIENTE DE CORREO----------------------*/
            //Creamos un objeto de cliente de correo
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();
            //Hay que crear las credenciales del correo emisor
            cliente.Credentials = new System.Net.NetworkCredential(message.senderEmail, message.password);
            cliente.Port = message.port > 0 ? message.port : puerto;
            cliente.EnableSsl = true;
            cliente.Host = string.IsNullOrEmpty(message.hostEmail) ? host : message.hostEmail;
            int waitTime = 15000;
            for(int i = 0; i <= 5; i++)
            {
                try
                {
                    cliente.Send(mmsg);
                    codeBack.code = 1;
                    codeBack.description = "Message sent successfully";
                    cliente.Dispose();                    
                    return codeBack;
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    codeBack.code = 2;
                    codeBack.description = "The message could not be sent. " + ex;                    
                }
                System.Threading.Thread.Sleep(waitTime);
            }
            return codeBack;
        }        
    }    
}