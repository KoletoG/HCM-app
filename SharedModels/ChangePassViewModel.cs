using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class ChangePassViewModel
    {
        public string NewPassword { get; set; }
        public string Id { get; set; }
        public ChangePassViewModel(string id, string newPassword)
        {
            Id = id;
            NewPassword = newPassword;
        }
    }
}
