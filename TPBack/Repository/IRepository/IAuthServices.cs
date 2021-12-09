using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPBack.Models;

namespace TPBack.Repository.IRepository
{
    public interface IAuthServices
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
