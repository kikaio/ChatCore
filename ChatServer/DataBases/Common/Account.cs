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
using CoreNet.Utils;
using CoreNet.Utils.Loggers;

namespace ChatServer.DataBases.Common
{
    [Serializable]
    [Table("Account")]
    public class Account
    {
        [Key]
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

        private static CoreLogger logger = new ConsoleLogger();

        public static async Task<bool> SignUp(string _nickName, string _plainPW)
        {
            if (string.IsNullOrWhiteSpace(_nickName) || string.IsNullOrWhiteSpace(_plainPW))
                return false;
            try
            {
                using (var c = new CommonContext())
                {
                    var ret = await c.Accounts.Where(x => x.NickName == _nickName).SingleOrDefaultAsync();
                    if (ret == default(Account))
                    {
                        var newAcc = new Account();
                        newAcc.NickName = _nickName;
                        newAcc.Pw = CryptHelper.PlainStrToBase64WithSha256(_plainPW);
                        c.Accounts.Add(newAcc);
                        await c.SaveChangesAsync();
                        logger.WriteDebug($"signup complete-{_nickName}");
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                return false;
            }
            return false;
        }
        public static async Task<Account> SignIn(string _nickName, string _plainPW)
        {
            if (string.IsNullOrWhiteSpace(_nickName) || string.IsNullOrWhiteSpace(_plainPW))
                return default(Account);
            using (var c = new CommonContext())
            {
                var sha256dPw = CryptHelper.PlainStrToBase64WithSha256(_plainPW);
                var acc = await c.Accounts.AsNoTracking().Where(x => x.NickName == _nickName && x.Pw == sha256dPw).SingleOrDefaultAsync();
                return acc;
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
