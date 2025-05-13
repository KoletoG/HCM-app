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

        public bool IsLastPage { get; set; }
        public bool IsFirstPage { get; set; }
        public int Page {  get; set; }
        public UsersToUpdateViewModel(List<UserDataModel> users, bool isLastPage, bool isFirstPage, int page) 
        {
            Users = users;
            IsFirstPage = isFirstPage;
            IsLastPage = isLastPage;
            Page = page;
        }
        public UsersToUpdateViewModel(List<UserDataModel> users,List<string> errors, bool isLastPage, bool isFirstPage,int page)
        {
            Users = users;
            Errors = errors;
            IsFirstPage = isFirstPage;
            IsLastPage = isLastPage;
            Page = page;
        }
    }
}
