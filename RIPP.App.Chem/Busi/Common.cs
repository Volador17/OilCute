using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.App.Chem.Roles;

namespace RIPP.App.Chem.Busi
{
    public class Common
    {
        private static UserEntity _logonuser;
        private static Config _config;

        public static Config Configuration
        {
            get
            {
                if (_config == null)
                    _config = new Config();
                return _config;
            }
        }


        public static UserEntity LogonUser
        {
            set { _logonuser = value; }
            get
            {
                if (_logonuser == null)
                    _logonuser = new UserEntity();
                if (_logonuser.Role == null)
                    _logonuser.Role = new RoleEntity();
                return _logonuser;
            }
        }

        public static void Reload()
        {
            _config = null;
        }

    }
}
