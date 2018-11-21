package com.packtpub.azure;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;

public class AzureSqlConsoleApp {
	public static void main(String args[]) {
		String hostName = "your_server.database.windows.net";
		String databaseName = "your_database";
		String userName = "your_username";
		String password = "your_password";
		String url = String.format("jdbc:sqlserver://%s:1433;database=%s;user=%s;password=%s;encrypt=true;hostNameInCertificate=*.database.windows.net;loginTimeout=30;",
				hostName,
				databaseName,
				userName,
				password);

		try (Connection con = DriverManager.getConnection(url)) {
			String sql = "SELECT Id, FirstName, LastName FROM Person";
			
			try (PreparedStatement stmt = con.prepareStatement(sql)) {
				ResultSet rs = stmt.executeQuery();
				
				while(rs.next()) {
					System.out.println(String.format("%s = %s %s",
							rs.getString("Id"),
							rs.getString("FirstName"),
							rs.getString("LastName")));
				}
				System.out.println("Press any key to continue...");
				System.in.read();
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		
	}
}
