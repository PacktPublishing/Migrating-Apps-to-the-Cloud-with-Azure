package com.packtpub.azure;

import java.util.ArrayList;
import java.util.List;

import org.bson.codecs.pojo.annotations.BsonId;
import org.bson.types.ObjectId;


public class Person {
	@BsonId
	private ObjectId id;
	private String firstName;
	private String lastName;
	private List<String> interests = new ArrayList<String>();

	public ObjectId getId() {
		return id;
	}

	public Person setId(ObjectId id) {
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
