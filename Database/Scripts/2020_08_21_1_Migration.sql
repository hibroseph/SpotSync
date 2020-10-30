CREATE TABLE Exceptions (
	id SERIAL PRIMARY KEY  NOT NULL,
	record_time TIMESTAMP NOT NULL,
	exception_message VARCHAR(500),
	stack_trace VARCHAR(3000),
	custom_message VARCHAR(2000),
	reference_id VARCHAR(50)
);

CREATE TABLE ActivityLog (
	id SERIAL PRIMARY KEY NOT NULL,
	record_time TIMESTAMP NOT NULL,
	user_action VARCHAR(200) NOT NULL,
	username VARCHAR(50) NOT NULL
);