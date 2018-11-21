package com.packtpub.azure;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class Person implements Serializable {
	private static final long serialVersionUID = 1L;
	private int id;
	private String firstName;
	private String lastName;
	private List<String> interests = new ArrayList<String>();

	public int getId() {
		return id;
	}
	
	public Person setId(int id) {
		this.id = id;
		return this;
	}
	
	public String getFirstName() {
		return firstName;
	}
	
	public Person setFirstName(String firstName) {
		this.firstName = firstName;
		return this;
	}
	
	public String getLastName() {
		return lastName;
	}
	
	public Person setLastName(String lastName) {
		this.lastName = lastName;
		return this;
	}

	public List<String> getInterests() {
		return interests;
	}
	
	public Person addInterest(String interest) {
		this.interests.add(interest);
		return this;
	}
}
