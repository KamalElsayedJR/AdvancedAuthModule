using CORE.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto email);
    }
}
