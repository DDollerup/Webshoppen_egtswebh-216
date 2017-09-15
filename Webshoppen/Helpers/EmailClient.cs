using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.IO;
using Webshoppen.Models;
using System.Web.Hosting;

public class EmailClient
{
    private string _smtp;
    private int _port;
    private string _systemMail;
    private string _password;
    private bool _ssl;

    private SmtpClient mailClient;

    public EmailClient(string smtp, int port, string email, string password, bool ssl)
    {
        _smtp = smtp;
        _port = port;
        _systemMail = email;
        _password = password;
        _ssl = ssl;

        mailClient = new SmtpClient(_smtp, _port);
        mailClient.EnableSsl = _ssl;
        mailClient.UseDefaultCredentials = true;
        mailClient.Credentials = new System.Net.NetworkCredential(email, password);
    }

    public void SendEmail(string to)
    {
        MailMessage mail = new MailMessage(_systemMail, to);
        mail.IsBodyHtml = true;

        mail.Subject = "Hej med dig";
        mail.From = new MailAddress(_systemMail);
        mail.To.Add(new MailAddress(to));

        mail.Body = "This is an email";

        mailClient.Send(mail);
    }

    public string ReadHtml(string path)
    {
        string htmlBody = "";

        using (StreamReader reader = new StreamReader(path))
        {
            htmlBody += reader.ReadToEnd();
        }
        return htmlBody;
    }

    public void SendNotification(string customerName, string email, string subject, string message)
    {
        #region To Client
        string htmlBody = ReadHtml(HostingEnvironment.ApplicationPhysicalPath + @"/EmailTemplates/Notification.html");

        htmlBody = htmlBody.Replace("{customerName}", customerName);
        htmlBody = htmlBody.Replace("{date}", DateTime.Today.ToShortDateString());

        MailMessage mail = new MailMessage();
        mail.IsBodyHtml = true;

        mail.Subject = "Din Besked er blevet modtaget - Webshoppen ApS";
        mail.From = new MailAddress(_systemMail);
        mail.To.Add(new MailAddress(email));

        mail.Body = htmlBody;

        mailClient.Send(mail);
        #endregion

        #region To Administrator

        htmlBody = "";
        htmlBody = ReadHtml(HostingEnvironment.ApplicationPhysicalPath + @"/EmailTemplates/NotificationToAdmin.html");

        htmlBody = htmlBody.Replace("{customerName}", customerName);
        htmlBody = htmlBody.Replace("{subject}", subject);
        htmlBody = htmlBody.Replace("{email}", email);
        htmlBody = htmlBody.Replace("{message}", message);
        htmlBody = htmlBody.Replace("{date}", DateTime.Today.ToShortDateString());

        MailMessage mailToAdministrator = new MailMessage();
        mailToAdministrator.IsBodyHtml = true;
        mailToAdministrator.Subject = "Der er kommet en mail fra: " + customerName + " via kontaktformen";
        mailToAdministrator.Body = htmlBody;
        mailToAdministrator.From = new MailAddress(_systemMail);
        mailToAdministrator.To.Add(new MailAddress(_systemMail));

        mailClient.Send(mailToAdministrator);
        #endregion
    }


    public string TableRow(string data)
    {
        return "<tr>" + data + "<tr>";
    }

    public string TableData(string info)
    {
        return "<td>" + info + "</td>";
    }

    public void SendInvoice(Order order, List<ProductAmountVM> products)
    {
        string htmlBody = ReadHtml(HostingEnvironment.ApplicationPhysicalPath + @"/EmailTemplates/Invoice_top.html");

        htmlBody = htmlBody.Replace("{orderNumber}", order.ID.ToString());

        htmlBody = htmlBody.Replace("{fullName}", order.Fullname);
        htmlBody = htmlBody.Replace("{phone}", order.Phone);
        htmlBody = htmlBody.Replace("{email}", order.Email);
        htmlBody = htmlBody.Replace("{address}", order.Address);
        htmlBody = htmlBody.Replace("{postal}", order.Postal);
        htmlBody = htmlBody.Replace("{city}", order.City);

        if (order.Fullname_Delivery != null)
        {
            htmlBody = htmlBody.Replace("{fullName-delivery}", order.Fullname_Delivery);
            htmlBody = htmlBody.Replace("{phone-delivery}", order.Phone_Delivery);
            htmlBody = htmlBody.Replace("{email-delivery}", order.Email_Delivery);
            htmlBody = htmlBody.Replace("{address-delivery}", order.Address_Delivery);
            htmlBody = htmlBody.Replace("{postal-delivery}", order.Postal_Delivery);
            htmlBody = htmlBody.Replace("{city-delivery}", order.City_Delivery);
        }
        else
        {
            htmlBody = htmlBody.Replace("{fullName-delivery}", order.Fullname);
            htmlBody = htmlBody.Replace("{phone-delivery}", order.Phone);
            htmlBody = htmlBody.Replace("{email-delivery}", order.Email);
            htmlBody = htmlBody.Replace("{address-delivery}", order.Address);
            htmlBody = htmlBody.Replace("{postal-delivery}", order.Postal);
            htmlBody = htmlBody.Replace("{city-delivery}", order.City);
        }

        string productBody = "";
        double total = 0;

        foreach (ProductAmountVM vm in products)
        {
            productBody += TableRow(
                TableData(vm.ProductVM.Product.Name) +
                TableData(vm.ProductVM.Product.Description) +
                TableData(vm.Amount.ToString()) +
                TableData(vm.ProductVM.Product.Price.ToString())
                );
            total += vm.Total();
        }

        productBody += TableRow(
            "<td colspan='3'>Total</td>" +
            "<td>" + total + "</td>"
            );

        htmlBody += productBody;

        htmlBody += ReadHtml(HostingEnvironment.ApplicationPhysicalPath + @"/EmailTemplates/Invoice_bottom.html");

        htmlBody = htmlBody.Replace("{year}", DateTime.Now.Year.ToString());

        MailMessage mail = new MailMessage();
        mail.IsBodyHtml = true;
        mail.Body = htmlBody;
        mail.From = new MailAddress(_systemMail);
        mail.To.Add(new MailAddress(order.Email));
        mail.Subject = "Invoice for Ordernumber: " + order.ID;

        mailClient.Send(mail);
    }
}