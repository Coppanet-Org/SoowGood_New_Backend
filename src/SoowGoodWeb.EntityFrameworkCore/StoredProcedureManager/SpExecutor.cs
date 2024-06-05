﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SoowGoodWeb.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace SoowGoodWeb.StoredProcedureManager
{
    //[ConnectionStringName("Default")]
    public static class SpExecutor
    {
        static IConfigurationRoot configuration = BuildConfiguration();
        static string? conStr = configuration != null ? configuration.GetConnectionString("Default") : "";

        public static List<T> ToList<T>(DataTable dt)
        {
            List<T> objects = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    T item = Activator.CreateInstance<T>();
                    foreach (PropertyInfo propInfo in typeof(T).GetProperties())
                    {
                        if (propInfo.CanWrite == false) continue;

                        string fieldname = propInfo.Name;

                        if (dr.Table.Columns.Contains(fieldname))
                        {
                            object propertyValue = (dr[fieldname]);
                            if (propertyValue != DBNull.Value)
                            {

                                if (propInfo.PropertyType.FullName.Contains("Nullable"))
                                {
                                    propInfo.SetValue(item, System.Convert.ChangeType(propertyValue, Nullable.GetUnderlyingType(propInfo.PropertyType)));
                                }
                                else
                                {
                                    propInfo.SetValue(item, System.Convert.ChangeType(propertyValue, propInfo.PropertyType), null);
                                }
                            }
                        }
                    }
                    T ctpe = (T)Convert.ChangeType(item, typeof(T));
                    objects.Add(ctpe);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return objects;
        }
        public static List<T> ExecuteStoredProcedure<T>(string SPName, List<SqlParams>? Params)
        {
            using (SqlConnection objConn = new SqlConnection(conStr))
            {
                SqlCommand objCmd = new SqlCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = SPName;
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandTimeout = 600;
                foreach (var Param in Params)
                {
                    objCmd.Parameters.Add(Param.ParamName, Param.SqlType).Value = Param.Value;
                    objCmd.Parameters[Param.ParamName].Direction = ParameterDirection.Input;
                }

                DataTable dt = new DataTable();

                try
                {
                    objConn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(objCmd);
                    da.SelectCommand = objCmd;
                    da.Fill(dt);

                    objConn.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ToList<T>(dt);

            }
        }
        public static DataTable ExecuteStoredProcedure(string SPName, List<SqlParams> Params)
        {
            using (SqlConnection objConn = new SqlConnection(conStr))
            {
                SqlCommand objCmd = new SqlCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = SPName;
                objCmd.CommandTimeout = 1200;
                objCmd.CommandType = CommandType.StoredProcedure;
                foreach (var Param in Params)
                {
                    objCmd.Parameters.Add(Param.ParamName, Param.SqlType).Value = Param.Value;
                    objCmd.Parameters[Param.ParamName].Direction = ParameterDirection.Input;
                }

                DataTable dt = new DataTable();

                try
                {
                    objConn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(objCmd);

                    da.Fill(dt);

                    objConn.Close();

                }
                catch
                {
                    //throw ex;
                }
                return dt;

            }
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SoowGoodWeb.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
