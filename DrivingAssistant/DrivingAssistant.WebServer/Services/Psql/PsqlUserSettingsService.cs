﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Services.Generic;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlUserSettingsService : IUserSettingsService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlUserSettingsService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<UserSettings>> GetAsync()
        {
            throw new NotImplementedException();
        }


        //============================================================
        public async Task<long> SetAsync(UserSettings data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public async Task DeleteAsync(UserSettings data)
        {
            throw new NotImplementedException();
        }

        //============================================================
        public async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
