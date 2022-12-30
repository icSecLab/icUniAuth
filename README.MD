# icUniAuth

### [使用帮助](https://github.com/icSecLab/icUniAuth/wiki)

简体中文 | [English](./README.en-us.md)

> 注:其他语言版本的文档可能存在更新不及时的情况，当出现不一致时，以简体中文版文档为准。

> Notice:The documents in other languages may not be updated in time. When inconsistent, the simplified Chinese documents shall prevail. 

----------

## 一款安全可靠兼容OAuth2.0的统一认证系统

项目部署地址:
 - [http://uniauth.icseclab.org][1]
 

----------
### **icUniAuth 版本列表**

#### ***icUniAuth V1.0***
released in *Dec 30, 2022*
> * 兼容OAuth2.0标准
> * 支持分布式部署

----------

### **部署指南**

#### **1.克隆仓库代码**

```bash
git clone https://github.com/icSecLab/icUniAuth.git
```

#### **2.修改代码中的服务器配置信息**
\icUniAuth\icUniAuth\utils\Utils.vb中包发送激活邮件的代码需要适当修改
```vb
  Public Sub SendEmail(toAddr As String, bodyhtml As String)
        Dim addr As New Net.Mail.MailAddress(toAddr)
        Dim smtpClient As New Net.Mail.SmtpClient("host", 25) '请将此处的host改为您的SMTP服务器地址，注意有些SMTP服务器要求用户认证，可以在此处增加以下语句
		'smtpClient.Credentials = New System.Net.NetworkCredential("username", "password")
        Dim mail As New Net.Mail.MailMessage()
        mail.Subject = "icUniAuth确认邮件"
        mail.From = New Net.Mail.MailAddress("emailaddress", "icUniAuth账号事务局")'请将此处的emailaddress改为您的邮箱地址
        mail.Priority = Net.Mail.MailPriority.Normal
        mail.IsBodyHtml = True
        mail.Body = bodyhtml
        mail.To.Add(addr)
        smtpClient.Send(mail)
    End Sub
```

\icUniAuth\icUniAuth\server.conf包含基本的服务器配置信息需要调整
```java
{
   "Server" : {
        "Configed": "True",
        "BaseURL": "http://auth.example.com", //请将此改为您项目部署的url地址
   },
  "Database": {
    "Host": "localhost", //请将此改为您项目使用的MySQL服务器地址
    "Port": "3306", //请将此改为您项目使用的MySQL服务器端口
    "Username": "icuniauth", //请将此改为您项目使用的MySQL服务器用户名
    "Name": "icuniauth", //请将此改为您项目使用的MySQL服务器密码
    "Password": "icuniauth" //请将此改为您项目使用的MySQL数据库名，该数据库应当提前成功创建
  }
}
```
#### **3.通过visual studio的发布向导发布项目**

#### **4.将项目部署到云**

##### *本项支持分布式部署，可以多服务器部署，通过Nginx等软件实现均衡负载，提高可用性

----------

### **icUniAuth项目开发者**

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