package com.packtpub.azure;

import static com.mongodb.client.model.Filters.eq;
import static org.bson.codecs.configuration.CodecRegistries.fromProviders;
import static org.bson.codecs.configuration.CodecRegistries.fromRegistries;

import java.io.IOException;
import java.util.Arrays;

import org.bson.codecs.configuration.CodecRegistry;
import org.bson.codecs.pojo.Conventions;
import org.bson.codecs.pojo.PojoCodecProvider;

import com.mongodb.MongoClient;
import com.mongodb.MongoClientURI;
import com.mongodb.client.FindIterable;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoCursor;
import com.mongodb.client.MongoDatabase;

public class AzureMongoDBConsoleApp {

	public static void main(String[] args) {
		int port = 10255;
		String userName = "your_username";
		String password = "your_password";
		String host = "your_host";
		String replicaSet = "your_replicaset";
		String databaseName = "your_database";
		String collectionName = "people";
		String url = String.format("mongodb://%s:%s@%s.documents.azure.com:%s/?ssl=true&replicaSet=%s",
				userName,
				password,
				host,
				port,
				replicaSet);
		
		MongoClientURI uri = new MongoClientURI(url);
		try (MongoClient client = new MongoClient(uri)) {
			CodecRegistry registry = fromRegistries(MongoClient.getDefaultCodecRegistry(),
					fromProviders(PojoCodecProvider
							.builder()
							.conventions(Arrays.asList(Conventions.USE_GETTERS_FOR_SETTERS, Conventions.ANNOTATION_CONVENTION))
							.automatic(true)
							.build()));
			MongoDatabase database = client.getDatabase(databaseName).withCodecRegistry(registry);
			MongoCollection<Person> collection = database.getCollection(collectionName, Person.class);
			
			collection.insertOne(new Person()
					.setFirstName("Bill")
					.setLastName("Gates")		
					.addInterest("Microsoft")
					.addInterest("Philanthropy")
					.addInterest("Being Rich"));
			
			FindIterable<Person> it = collection.find();
			try (MongoCursor<Person> cursor = it.iterator()) {
				while(cursor.hasNext()) {
					Person person = cursor.next();
					System.out.println(String.format("%s - %s %s - %s", 
							person.getId(),
							person.getFirstName(),
							person.getLastName(),
							person.getInterests()));
				}
				System.out.println("Press any key");
				System.in.read();
			}
			catch(IOException e) {
				e.printStackTrace();
			}
			
			collection.insertMany(Arrays.asList(
					new Person()
						.setFirstName("Steve")
						.setLastName("Balmer")		
						.addInterest("Basketball")
						.addInterest("Developers")
						.addInterest("Being Rich"),
					new Person()
						.setFirstName("Satya")
						.setLastName("Nadella")		
						.addInterest("Open Source")
						.addInterest("Microsoft")));
			
			it = collection.find(eq("interests", "Being Rich"));
			try (MongoCursor<Person> cursor = it.iterator()) {
				while(cursor.hasNext()) {
					Person person = cursor.next();
					System.out.println(String.format("%s - %s %s - %s", 
							person.getId(),
							person.getFirstName(),
							person.getLastName(),
							person.getInterests()));
				}
				System.out.println("Press any key");
				System.in.read();
			}
			catch(IOException e) {
				e.printStackTrace();
			}
		}
	}
}
