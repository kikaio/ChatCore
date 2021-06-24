using System;
using System.Data.Entity;
using MySql.Data.EntityFramework;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CoreNet.Cryptor;

namespace ChatServer.DataBases.Common
{
    [Serializable]
    [Table("Account")]
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
            if (string.IsNullOrWhiteSpace(_nickName) || string.IsNullOrWhiteSpace(_plainPW))
                return false;
            using (var c = new CommonContext())
            {
                var ret = await c.Accounts.Where(x=>x.NickName == _nickName).SingleOrDefaultAsync();
                if (ret == default(Account))
                {
                    var newAcc = new Account();
                    newAcc.NickName = _nickName;
                    newAcc.Pw = CryptHelper.PlainStrToBase64WithSha256(_plainPW);
                    c.Accounts.Add(newAcc);
                    await c.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        public static async Task<bool> SignIn(string _nickName, string _plainPW)
        {
            if (string.IsNullOrWhiteSpace(_nickName) || string.IsNullOrWhiteSpace(_plainPW))
                return false;
            using (var c = new CommonContext())
            {
                var acc = await c.Accounts.Where(x => x.NickName == _nickName).SingleOrDefaultAsync();
                if (acc == default(Account))
                    return false;
                return acc.Pw == CryptHelper.PlainStrToBase64WithSha256(_plainPW);
            }
        }
        public static async Task<bool> SignOut(string _nickName, string _plainPW)
        {
            if (string.IsNullOrWhiteSpace(_nickName) || string.IsNullOrWhiteSpace(_plainPW))
                return false;
            using (var c = new CommonContext())
            {
                var shaedPw = CryptHelper.PlainStrToBase64WithSha256(_plainPW);
                var target = await c.Accounts.Where(x => x.NickName == _nickName && x.Pw == shaedPw).SingleOrDefaultAsync();
                if (target == default(Account))
                    return false;
                target.UpdatedDate = DateTime.UtcNow;
                target.IsSignOut = true;
                await c.SaveChangesAsync();
            }
            return true;
        }
    }
}
