using MessyLabAdmin.Util.Sendgrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.OptionsModel;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace MessyLabAdmin.Services
{
    public class SendgrindEmailConfiguration
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool UseTestingEmailDestination { get; set; }
        public string TestingEmailDestination { get; set; }
    }

    public class SendgridEmailSender : IEmailSender
    {

        private readonly SendgrindEmailConfiguration _configuration;

        public SendgridEmailSender(IOptions<SendgrindEmailConfiguration> confOptions)
        {
            _configuration = confOptions.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            Email from = new Email(_configuration.FromEmail, _configuration.FromName);
            Email to = new Email(_configuration.UseTestingEmailDestination ? _configuration.TestingEmailDestination : email);
            Content content = new Content("text/html", message);
            Mail mail = new Mail(from, subject, to, content);

            var sg = new SendGridAPIClient(_configuration.ApiKey);
            dynamic response = sg.client.mail.send.post(requestBody: mail.Get());

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                // TODO: error reporting
            }

            return Task.FromResult(0);
        }
    }
}
