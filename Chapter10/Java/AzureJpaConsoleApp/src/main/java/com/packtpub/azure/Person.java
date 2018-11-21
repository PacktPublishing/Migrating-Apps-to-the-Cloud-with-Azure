package com.packtpub.azure;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "Person")
public class Person {
	
	@Id
	@Column(name = "Id")
	private Integer id;
	
	@Column(name = "FirstName", columnDefinition = "NVARCHAR(255)")
	private String firstName;
	
	
	@Column(name = "LastName", columnDefinition = "NVARCHAR(255)")
	private String lastName;

	public Integer getId() {
		return id;
	}

	public Person setId(Integer id) {
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
}
