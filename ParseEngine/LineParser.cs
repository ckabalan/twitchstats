﻿/// <copyright file="LineParser.cs" company="SpectralCoding.com">
///     Copyright (c) 2015 SpectralCoding
/// </copyright>
/// <license>
/// This file is part of TwitchStats.
///
/// IncreBuild is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// IncreBuild is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
///
/// You should have received a copy of the GNU General Public License
/// along with TwitchStats.  If not, see <http://www.gnu.org/licenses/>.
/// </license>
/// <author>Caesar Kabalan</author>

namespace ParseEngine {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Math;
	using DataManager;
	using MySql.Data.MySqlClient;

	public static class LineParser {
		public static List<DBDelta> Message(DateTime date, String channelName, String username, String message) {
			List<DBDelta> returnList = new List<DBDelta>();
			returnList.AddRange(AddMessageChannel(date, "_global$1min_line_stats", 1));
			returnList.AddRange(AddMessageChannel(date, "_global$5min_line_stats", 5));
			returnList.AddRange(AddMessageChannel(date, "_global$15min_line_stats", 15));
			returnList.AddRange(AddMessageChannel(date, "_global$30min_line_stats", 30));
			returnList.AddRange(AddMessageChannel(date, "_global$1hr_line_stats", 60));
			return returnList;
		}

		public static List<DBDelta> Action(DateTime date, String channelName, String username, String message) {
			List<DBDelta> returnList = new List<DBDelta>();
			returnList.AddRange(AddActionChannel(date, "_global$1min_line_stats", 1));
			returnList.AddRange(AddActionChannel(date, "_global$5min_line_stats", 5));
			returnList.AddRange(AddActionChannel(date, "_global$15min_line_stats", 15));
			returnList.AddRange(AddActionChannel(date, "_global$30min_line_stats", 30));
			returnList.AddRange(AddActionChannel(date, "_global$1hr_line_stats", 60));
			return returnList;
		}

		private static List<DBDelta> AddMessageChannel(DateTime date, String tableName, Int32 accuracy) {
			List<DBDelta> returnList = new List<DBDelta>();
			Int32 timeID = GetTimeID(date, accuracy);
			returnList.Add(new DBDelta { TimeID = timeID, Table = tableName, Column = "messages", Delta = 1 });
			returnList.Add(new DBDelta { TimeID = timeID, Table = tableName, Column = "total", Delta = 1 });
			return returnList;
		}

		private static List<DBDelta> AddActionChannel(DateTime date, String tableName, Int32 accuracy) {
			List<DBDelta> returnList = new List<DBDelta>();
			Int32 timeID = GetTimeID(date, accuracy);
			returnList.Add(new DBDelta { TimeID = timeID, Table = tableName, Column = "actions", Delta = 1 });
			returnList.Add(new DBDelta { TimeID = timeID, Table = tableName, Column = "total", Delta = 1 });
			return returnList;
		}

		private static Int32 GetTimeID(DateTime date, Int32 accuracyMinutes) {
			DateTime fakeEpoch = new DateTime(2015, 1, 1, 0, 0, 0);
			return Convert.ToInt32(Math.Floor((date - fakeEpoch).TotalMinutes / accuracyMinutes));
		}

		private static Boolean TimeIDExists(Int32 timeID, String tableName) {
			MySqlCommand checkCmd = new MySqlCommand(
				"SELECT * FROM `" + tableName + @"` WHERE `time_id` = @time_id LIMIT 1;",
				DBManager.DbConnection);
			checkCmd.Parameters.AddWithValue("@time_id", timeID);
			if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0) {
				return true;
			}
			return false;
		}
	}
}
