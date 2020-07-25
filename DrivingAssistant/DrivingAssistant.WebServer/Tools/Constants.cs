﻿using System.Runtime.InteropServices;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Constants
    {
        //============================================================
        public static class ServerConstants
        {
            private const string LinuxImageStoragePath = @"/mnt/hdd/CloudStorage/Images";
            private const string LinuxVideoStoragePath = @"/mnt/hdd/CloudStorage/Videos";
            private const string LinuxPsqlConnectionString = @"Host=127.0.0.1;Port=5432;Database=cloud;Username=pi;Password=1234";
            private const string LinuxMSSQLConnectionString = @"";

            private const string WindowsImageStoragePath = @"C:\CloudStorage\Images";
            private const string WindowsVideoStoragePath = @"C:\CloudStorage\Videos";
            private const string WindowsPsqlConnectionString = @"Host=127.0.0.1;Port=5432;Database=cloud;Username=postgres;Password=1234";
            private const string WindowsMSSQLConnectionString = @"Data Source=DESKTOP-21H2OBB;Initial Catalog=DrivingAssistant;Persist Security Info=True;User ID=sa;Password=pxd";

            //============================================================
            public static string GetImageStoragePath()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxImageStoragePath : WindowsImageStoragePath;
            }

            //============================================================
            public static string GetVideoStoragePath()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxVideoStoragePath : WindowsVideoStoragePath;
            }

            //============================================================
            public static string GetPsqlConnectionString()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? LinuxPsqlConnectionString
                    : WindowsPsqlConnectionString;
            }

            //============================================================
            public static string GetMssqlConnectionString()
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? LinuxMSSQLConnectionString
                    : WindowsMSSQLConnectionString;
            }
        }

        //============================================================
        public static class PsqlDatabaseConstants
        {
            public const string GetMediaCommand = @"select * from media;";
            public const string GetMediaByIdCommand = @"select * from media where id = @id;";
            public const string AddMediaCommand = @"insert into media(processed_id, session_id, user_id, type, filepath, source, description, date_added) values (@processed_id, @session_id, @user_id, @type, @filepath, @source, @description, @date_added) returning id;";
            public const string UpdateMediaCommand = @"update media set processed_id = @processed_id, session_id = @session_id, user_id = @user_id, type = @type, filepath = @filepath, source = @source, description = @description, date_added = @date_added where id = @id";
            public const string DeleteMediaCommand = @"delete from media where id = @id";

            public const string GetUsersCommand = @"select * from users;";
            public const string GetUserByIdCommand = @"select * from users where id = @id";
            public const string AddUserCommand = @"insert into users(username, password, first_name, last_name, email, role, join_date) values (@username, @password, @first_name, @last_name, @email, @role, @join_date) returning id;";
            public const string UpdateUserCommand = @"update users set username = @username, password = @password, first_name = @first_name, last_name = @last_name, email = @email, role = @role, join_date = @join_date where id = @id";
            public const string DeleteUserCommand = @"delete from users where id = @id";

            public const string GetSessionsCommand = @"select * from sessions";
            public const string GetSessionByIdCommand = @"select * from sessions where id = @id";
            public const string AddSessionCommand = @"insert into sessions(user_id, description, start_date_time, end_date_time, start_point, end_point, intermediate_points, processed) values (@user_id, @description, @start_date_time, @end_date_time, @start_point, @end_point, @intermediate_points, @processed) returning id;";
            public const string UpdateSessionCommand = @"update sessions set user_id = @user_id, description = @description, start_date_time = @start_date_time, end_date_time = @end_date_time, start_point = @start_point, end_point = @end_point, intermediate_points = @intermediate_points, processed = @processed where id = @id";
            public const string DeleteSessionCommand = @"delete from sessions where id = @id";
        }
    }
}