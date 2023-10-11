﻿using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using PeanutButter.TempDb.MySql.Base;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PeanutButter.TempDb.MySql.Data
{
    /// <summary>
    /// Provides the TempDB implementation for MySql, using
    /// MySql.Data as the connector library
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TempDBMySql : TempDBMySqlBase<MySqlConnection>
    {
        /// <summary>
        /// Construct a TempDbMySql with zero or more creation scripts and default options
        /// </summary>
        /// <param name="creationScripts"></param>
        public TempDBMySql(params string[] creationScripts)
            : base(new TempDbMySqlServerSettings(), creationScripts)
        {
        }


        /// <summary>
        /// Create a TempDbMySql instance with provided options and zero or more creation scripts
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="creationScripts"></param>
        public TempDBMySql(
            TempDbMySqlServerSettings settings,
            params string[] creationScripts
        )
            : base(
                settings,
                o =>
                {
                },
                creationScripts)
        {
        }

        /// <summary>
        /// Create a TempDbMySql instance with provided options, an action to run before initializing and
        /// zero or more creation scripts
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="beforeInit"></param>
        /// <param name="creationScripts"></param>
        public TempDBMySql(
            TempDbMySqlServerSettings settings,
            Action<object> beforeInit,
            params string[] creationScripts
        ) : base(
            settings,
            o => BeforeInit(o as TempDBMySqlBase<MySqlConnection>, beforeInit, settings),
            creationScripts
        )
        {
        }

        /// <summary>
        /// Generates the connection string to use for clients
        /// wishing to connect to this temp instance
        /// </summary>
        /// <returns></returns>
        protected override string GenerateConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Port = (uint) Port,
                UserID = "root",
                Password = RootPasswordSet
                    ? Settings.Options.RootUserPassword
                    : "",
                Server = "localhost",
                AllowUserVariables = true,
                SslMode = MySqlSslMode.Disabled,
                Database = SchemaName,
                ConnectionTimeout = DefaultTimeout,
                DefaultCommandTimeout = DefaultTimeout,
                CharacterSet = Settings.CharacterSetServer,
                // may be required for re-using snapshotted databases
                AllowPublicKeyRetrieval = true
            };
            return builder.ToString();
        }

        /// <summary>
        /// Fetches the current in-use connection count from
        /// mysql.data pool stats via reflection
        /// </summary>
        /// <returns></returns>
        protected override int FetchCurrentConnectionCount()
        {
            var stats = PoolStatsFetcher.GetPoolStatsViaReflection(
                ConnectionString
            );
            return stats.TotalInUse;
        }

        private MySqlPoolStatsFetcher PoolStatsFetcher
            => _poolStatsFetcher ??= new MySqlPoolStatsFetcher();

        private MySqlPoolStatsFetcher _poolStatsFetcher;
    }
}