package com.packtpub.azure;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.nio.ByteBuffer;
import java.nio.charset.Charset;

import com.lambdaworks.redis.codec.RedisCodec;


public class ObjectCodec implements 
		RedisCodec<String, Object> {
	private Charset charset = Charset.forName("UTF-8");
	
	@Override
	public String decodeKey(ByteBuffer bytes) {
		return charset.decode(bytes).toString();
	}

	@Override
	public Object decodeValue(ByteBuffer bytes) {
		try {
			byte[] array = new byte[bytes.remaining()];
			bytes.get(array);
			ByteArrayInputStream bis = new ByteArrayInputStream(array);
			ObjectInputStream is = new ObjectInputStream(bis);
			return is.readObject();
		}
		catch (IOException | ClassNotFoundException e) {
			return null;
		}
	}

	@Override
	public ByteBuffer encodeKey(String key) {
		return charset.encode(key);
	}

	@Override
	public ByteBuffer encodeValue(Object value) {
		try {
			ByteArrayOutputStream bytes = new ByteArrayOutputStream();
			ObjectOutputStream os = new ObjectOutputStream(bytes);
			os.writeObject(value);
			return ByteBuffer.wrap(bytes.toByteArray());
		}
		catch (IOException e) {
			return null;
		}
	}

}
