using O7.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<LoginResponseViewModel> Login(LoginViewModel model);
        Task<LoginResponseViewModel> Register(RegisterViewModel model);
        Task<LoginResponseViewModel> RefreshToken(string token);
        Task<bool> RevokeToken(string token);
    }
}
