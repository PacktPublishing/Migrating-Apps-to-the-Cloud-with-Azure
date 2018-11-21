package com.packtpub.azure;

import java.util.HashMap;
import java.util.Map;

import com.lambdaworks.redis.RedisClient;
import com.lambdaworks.redis.RedisURI;
import com.lambdaworks.redis.api.StatefulRedisConnection;
import com.lambdaworks.redis.api.sync.RedisCommands;

public class AzureRedisConsoleApp {
	public static void main(String[] args) {
		String password = "your_password";
		String host = "your_host.redis.cache.windows.net";
		int port = 6380;

		RedisURI uri = RedisURI.create(host, port);
		uri.setPassword(password);
		uri.setSsl(true);

		RedisClient client = RedisClient.create(uri);
		try (StatefulRedisConnection<String, String> connection = client.connect()) {
			RedisCommands<String, String> sync = connection.sync();

			sync.set("counter", Integer.toString(1));
			System.out.println("The counter is at " + sync.get("counter"));
			sync.incr("counter");
			System.out.println("The counter is now at " + sync.get("counter"));
			sync.incrby("counter", 3);
			System.out.println("The counter is now at " + sync.get("counter"));
		}
		finally {
			client.shutdown();
		}

		client = RedisClient.create(uri);
		ObjectCodec codec = new ObjectCodec();
		try (StatefulRedisConnection<String, Object> connection = client.connect(codec)) {
			RedisCommands<String, Object> sync = connection.sync();

			Map<String, Object> people = new HashMap<String,Object>();
			people.put("1", new Person()
					.setId(1)
					.setFirstName("Bill")
					.setLastName("Gates")
					.addInterest("Microsoft")
					.addInterest("Philanthropy")
					.addInterest("Being Rich"));
			people.put("2", new Person()
					.setId(2)
					.setFirstName("Steve")
					.setLastName("Balmer")
					.addInterest("Basketball")
					.addInterest("Developers")
					.addInterest("Being Rich"));
			sync.mset(people);
			
			sync.set("3", new Person()
					.setId(3)
					.setFirstName("Satya")
					.setLastName("Nadella")
					.addInterest("Open Source")
					.addInterest("Microsoft"));

			for(Object result : sync.mget("1", "2", "3")) {
				Person person = (Person)result;
				System.out.println(String.format("%s - %s %s - %s",
					person.getId(),
					person.getFirstName(),
					person.getLastName(),
					person.getInterests()));
			}
		}
		finally {
			client.shutdown();
		}
	}
}
