using ComputerAccessories.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComputerAccessories.Helpers
{
    public static class CustomRepository
    {
        public static bool CreateUser(string email, string phoneNumber, string password, string fullname, int provinceId, int districtId, int wardId, string place)
        {
            using (ComputerAccessoriesContext db = new ComputerAccessoriesContext())
            {
                try
                {
                    var checkUser = db.TblUsers.Where(x => x.UserName.Equals(email)).FirstOrDefault();
                    if(checkUser != null)
                    {
                        return false;
                    }
                    TblUsers user = new TblUsers();
                    user.Email = email;
                    user.DisplayName = fullname;
                    user.PasswordSalt = CreateSalt();
                    user.PasswordHash = CreatePasswordHash(password, user.PasswordSalt);
                    user.CreatedDate = DateTime.Now;
                    user.IsActivated = true;
                    user.LockoutEnabled = false;
                    user.UserName = email;
                    user.PhoneNumber = phoneNumber;
                    user.EmailConfirmed = false;
                    user.PhoneNumberConfirmed = false;
                    user.TwoFactorEnabled = false;
                    user.LockoutEnabled = false;
                    user.AccessFailedCount = 0;
                    Random rand = new Random();
                    var code = rand.Next(100000, 999999).ToString();
                  
                    user.CodeConfirm = code;

                    user.SecurityStamp = Guid.NewGuid().ToString();

                    db.TblUsers.Add(user);
                    db.SaveChanges();
                    var result = GetUser(user.Email);
                    if(result == null)
                    {
                        return false;
                    }

                    TblUserAddress tblUserAddress = new TblUserAddress();
                    tblUserAddress.UserId = result.Id;
                    tblUserAddress.ProvinceId = provinceId;
                    tblUserAddress.DistrictId = districtId;
                    tblUserAddress.WardId = wardId;
                    tblUserAddress.PlaceDetail = place;

                    db.TblUserAddress.Add(tblUserAddress);
                    db.SaveChanges();
                    return true;
                }
                catch(Exception e)
                {
                    return false;
                    throw;
                }
            }
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            return GetSwcSH1(saltAndPwd);
        }

        public static string GetSwcSH1(string value)
        {
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
            //byte[] salt;
            //byte[] buffer2;
            //if (value == null)
            //{
            //    throw new ArgumentNullException("password");
            //}
            //using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(value, 0x10, 0x3e8))
            //{
            //    salt = bytes.Salt;
            //    buffer2 = bytes.GetBytes(0x20);
            //}
            //byte[] dst = new byte[0x31];
            //Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            //Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            //return Convert.ToBase64String(dst);
        }
        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }
        private static TblUsers GetUser(string email)
        {
            using(ComputerAccessoriesContext db = new ComputerAccessoriesContext())
            {
                return db.TblUsers.Where(x => x.Email.Equals(email)).FirstOrDefault();
            }
        }

        public static bool ValidateUser(string email, string password)
        {
            using (ComputerAccessoriesContext db = new ComputerAccessoriesContext())
            {
                var result = db.TblUsers.SingleOrDefault(m => m.Email.Equals(email));
                if (result != null)
                {
                    var dbuser = result;
                    if (dbuser.Password ==
                        CreatePasswordHash(password, dbuser.PasswordSalt))
                        return true;
                    return false;
                }
                else
                    return false;
            }
        }

        private static string GenerateKey()
        {
            Guid emailKey = Guid.NewGuid();

            return emailKey.ToString();
        }

        public static bool ChangePassword(string email, string newpassword)
        {
            using (ComputerAccessoriesContext db = new ComputerAccessoriesContext())
            {
                var result = db.TblUsers.SingleOrDefault(m => m.Email.Equals(email));
                if (result != null)
                {
                    var dbuser = result;
                    dbuser.Password = CreatePasswordHash(newpassword, dbuser.PasswordSalt);
                    //db.Entry(dbuser).State = System.Data.Entity.EntityState.Modified;
                    if (db.SaveChanges() == 1)
                        return true;
                    return false;
                }
                else
                    return false;
            }
        }
        public static async Task<bool> AddUserToRoleAsync(TblUsers user, int roleType, string description =null)
        {
            using(ComputerAccessoriesContext _db = new ComputerAccessoriesContext())
            {
                try
                {
                    var role = _db.TblRoles.Where(x => x.Id == roleType).FirstOrDefault();
                    if (role == null)
                    {
                        TblRoles newRole = new TblRoles
                        {
                            Name = description,
                            NormalizedName = "Default",
                        };
                        _db.TblRoles.Add(newRole);
                        await _db.SaveChangesAsync();

                        TblUserRole userRole = new TblUserRole()
                        {
                            RoleId = newRole.Id,
                            UserId = user.Id
                        };
                        _db.TblUserRole.Add(userRole);
                        await _db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {

                        TblUserRole userRole = new TblUserRole()
                        {
                            RoleId = roleType,
                            UserId = user.Id
                        };
                        _db.TblUserRole.Add(userRole);
                        await _db.SaveChangesAsync();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    return false;
                    throw;
                }
                
                
            }
        }

        public static bool VerifyHashedPassword(TblUsers user, string password)
        {
            //byte[] buffer4;
            //if (hashedPassword == null)
            //{
            //    return false;
            //}
            //if (password == null)
            //{
            //    throw new ArgumentNullException("password");
            //}
            //byte[] src = Convert.FromBase64String(hashedPassword);
            //if ((src.Length != 0x31) || (src[0] != 0))
            //{
            //    return false;
            //}
            //byte[] dst = new byte[0x10];
            //Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            //byte[] buffer3 = new byte[0x20];
            //Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            //using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            //{
            //    buffer4 = bytes.GetBytes(0x20);
            //}
            ////return ByteArraysEqual(buffer3, buffer4);
            //return buffer3.ToString().Equals(buffer4);
            return user.PasswordHash.Equals(CreatePasswordHash(password, user.PasswordSalt));
        }

    }
}
