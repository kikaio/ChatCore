using System;
using System.Data.Entity;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DataBases.Common
{
    [Serializable]
    [Table(Account)]
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, Index(IsUnique = true)]
        public string NickName { get; set; }
        [Required]
        public string Pw { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime RegistDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedDate { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsSignOut { get; set; }

        public static async Task<bool> SignUp(string _nickName, string _plainPW)
        {
            return false;
        }
        public static async Task<bool> SignIn(string _nickName, string _plainPW)
        {
            return false;
        }
        public static async Task<bool> SignOut(string _nickName, string _plainPW)
        {
            return false;
        }
    }
}
