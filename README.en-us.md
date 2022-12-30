# icUniAuth

### [Wiki](https://github.com/icSecLab/icUniAuth/wiki)

[简体中文](./README.md) | English

> Notice:The documents in other languages may not be updated in time. When inconsistent, the simplified Chinese documents shall prevail. 

----------

## A secure and reliable OAuth2.0 system

Project URL:
 - [http://uniauth.icseclab.org][1]
 

----------
### **icUniAuth release list**

#### ***icUniAuth V1.0***
released in *Dec 30, 2022*
> * Compatible with OAuth2.0
> * Support distributed deployment

----------

### **How to deploy**

#### **1.Clone the repository**

```bash
git clone https://github.com/icSecLab/icUniAuth.git
```

#### **2.Modify the config files**
\icUniAuth\icUniAuth\utils\Utils.vb This file contains the code for email sending. Put your SMTP server info here.
```vb
  Public Sub SendEmail(toAddr As String, bodyhtml As String)
        Dim addr As New Net.Mail.MailAddress(toAddr).
        Dim smtpClient As New Net.Mail.SmtpClient("host", 25) 'Please change the "host" to your SMTP server. Notice: some SMTP servers require authentication. Just add the code below
	'smtpClient.Credentials = New System.Net.NetworkCredential("username", "password")
        Dim mail As New Net.Mail.MailMessage()
        mail.Subject = "icUniAuth确认邮件"
        mail.From = New Net.Mail.MailAddress("emailaddress", "icUniAuth账号事务局")'Please change the "emailaddress" to your email address
        mail.Priority = Net.Mail.MailPriority.Normal
        mail.IsBodyHtml = True
        mail.Body = bodyhtml
        mail.To.Add(addr)
        smtpClient.Send(mail)
    End Sub
```

\icUniAuth\icUniAuth\server.conf This file contains the configuration of your web and database server.
```java
{
   "Server" : {
        "Configed": "True",
        "BaseURL": "http://auth.example.com", //Please change it to your project deployment url
   },
  "Database": {
    "Host": "localhost", //Change it to your MySQL server address
    "Port": "3306", //Change it to your MySQL port
    "Username": "icuniauth", //Change it to your MySQL username
    "Name": "icuniauth", //Change it to your MySQL password
    "Password": "icuniauth" //Change it to the name of your MySQL database that should be create first
  }
}
```
#### **3.Generate the final version by Visual Studio publish wizard**

#### **4.Deploy it to your server**

##### *This project support distributed deployment. To improve availability, you'd better deploy it on multiple servers and use a load balancer such as Nginx.
----------

### **Contributors**

 - [@HBSnail][2]
 - [@melody0123][3]
 - [@hzy0227][4]
 - [@Hshwhghs][5]


----------

[1]: http://uniauth.icseclab.org
[2]: https://github.com/HBSnail
[3]: https://github.com/melody0123
[4]: https://github.com/hzy0227
[5]: https://github.com/Hshwhghs