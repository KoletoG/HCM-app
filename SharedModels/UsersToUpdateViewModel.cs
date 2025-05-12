using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class UsersToUpdateViewModel
    {
        public List<UserDataModel> Users { get; set; }
        public List<string>? Errors { get; set; }

        public UsersToUpdateViewModel(List<UserDataModel> users) 
        {
            Users = users;
        }
        public UsersToUpdateViewModel(List<UserDataModel> users,List<string> errors)
        {
            Users = users;
            Errors = errors;
        }
    }
}
