using System;
using System.Collections.Generic;

public interface ISendSMS
{
    bool IsSupportSimCard { get; }
    bool SendSMS(string phoneNumber, string message);
    bool SendMail(string email, string subject, string message);
}
