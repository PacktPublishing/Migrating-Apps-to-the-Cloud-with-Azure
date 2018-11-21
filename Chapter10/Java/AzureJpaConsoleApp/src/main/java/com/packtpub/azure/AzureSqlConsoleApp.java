package com.packtpub.azure;

import java.io.IOException;
import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.Persistence;
import javax.persistence.TypedQuery;

public class AzureSqlConsoleApp {
	public static void main(String args[]) {
		EntityManagerFactory factory = Persistence.createEntityManagerFactory("jpa-example");
		EntityManager manager = factory.createEntityManager();
		
		manager.getTransaction().begin();
		
		TypedQuery<Person> query = manager.createQuery("select p from Person p", Person.class);

	
		List<Person> list = query.getResultList();
		
		for(Person person : list) {
			System.out.println(String.format("%s = %s %s",
					person.getId(),
					person.getFirstName(),
					person.getLastName()));
		}
		
		manager.getTransaction().commit();
		manager.close();
		factory.close();

		System.out.println("Press any key to continue...");
		try {
			System.in.read();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
